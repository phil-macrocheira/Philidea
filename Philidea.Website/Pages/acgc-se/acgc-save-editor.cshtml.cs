using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using Philidea.Website.Models;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace Philidea.Website.Pages
{
    public class Item
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string DropdownName { get; set; }
        public string ImageURL { get; set; }
        public string IconURL { get; set; }
        public string IconURLUpscaled { get; set; }
        public string TextureURL { get; set; }
        public string TextureURLUpscaled { get; set; }
        public string SourceGroup { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public short HRABasePoints { get; set; }
        public string HRASeries { get; set; }
        public byte HRASeriesType { get; set; }
        public byte HRAEssentialType { get; set; }
        public byte HRALucky { get; set; }
        public byte HRAFace { get; set; }
        public byte FengShui { get; set; }
        public ushort CatalogBit { get; set; }
        public byte Obtainable { get; set; }
        public string Category { get; set; }
        public string TextColor { get; set; }
        public byte CanRotate { get; set; }
        public short Order { get; set; }
    }
    public class Object
    {
        public string ID { get; set; }
        public string DropdownName { get; set; }
        public string ImageURL { get; set; }
    }
    public class Villager
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public byte Type { get; set; }
        public string ImageURL { get; set; }
    }
    public class acgcModel : PageModel
    {
        private readonly ILogger<acgcModel> _logger;
        public acgcSaveModel acgcSaveModelJson { get; private set; }
        public JsonDocument JsonData { get; }
        public string SaveJsonText { get; }
        public byte[] SaveFile { get; set; }
        public string SaveFileString { get; set; }
        public IEnumerable<Item> Items { get; private set; }
        public IEnumerable<Object> Objects { get; private set; }
        public IEnumerable<Villager> Villagers { get; private set; }
        public List<Item> SelectedItems { get; set; } = new List<Item> { new Item(), new Item(), new Item(), new Item() };
        public List<Item> SelectedHandhelds { get; set; } = new List<Item> { new Item(), new Item(), new Item(), new Item() };
        public List<Item> SelectedClothings { get; set; } = new List<Item> { new Item(), new Item(), new Item(), new Item() };
        public List<Villager> SelectedVillagers { get; set; } = new List<Villager>();
        public List<string> ItemIconURLs { get; set; } = new List<string> { "acgc-se/ui-icons/None.png", "acgc-se/ui-icons/None.png", "acgc-se/ui-icons/None.png", "acgc-se/ui-icons/None.png" };
        public List<string> HandheldIconURLs { get; set; } = new List<string> { "acgc-se/ui-icons/None.png", "acgc-se/ui-icons/None.png", "acgc-se/ui-icons/None.png", "acgc-se/ui-icons/None.png" };
        public List<string> ClothingIconURLs { get; set; } = new List<string> { "acgc-se/ui-icons/None.png", "acgc-se/ui-icons/None.png", "acgc-se/ui-icons/None.png", "acgc-se/ui-icons/None.png" };
        public List<string> InventoryIconURLs { get; set; } = new List<string> { "", "", "", "" };
        public string VillagerIconURL { get; set; }
        public acgcModel(ILogger<acgcModel> logger)
        {
            _logger = logger;
            SaveJsonText = System.IO.File.ReadAllText("acgc-se/data/save_data.json");
            JsonData = JsonDocument.Parse(SaveJsonText);

            string ItemJsonText = System.IO.File.ReadAllText("acgc-se/data/item_data.json");
            Items = JsonSerializer.Deserialize<List<Item>>(ItemJsonText);

            string ObjectJsonText = System.IO.File.ReadAllText("acgc-se/data/object_data.json");
            Objects = JsonSerializer.Deserialize<List<Object>>(ObjectJsonText);

            string VillagerJsonText = System.IO.File.ReadAllText("acgc-se/data/villager_data.json");
            Villagers = JsonSerializer.Deserialize<List<Villager>>(VillagerJsonText);
        }
        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (Request.Form["func"] == "updateItem") {
                byte player = byte.Parse(Request.Form["player"]);
                bool success = UpdateItem(Request.Form["selectedItemID"], player);

                return new JsonResult(new {
                    success = success, message = "Success",
                    itemIconURL = ItemIconURLs[player],
                    inventoryIconURL = InventoryIconURLs[player],
                });
            }
            else if (Request.Form["func"] == "updateHandheld") {
                byte player = byte.Parse(Request.Form["player"]);
                bool success = UpdateHandheld(Request.Form["selectedItemID"], player);

                return new JsonResult(new {
                    success = success, message = "Success",
                    handheldIconURL = HandheldIconURLs[player],
                });
            }
            else if (Request.Form["func"] == "updateClothing") {
                byte player = byte.Parse(Request.Form["player"]);
                bool success = UpdateClothing(Request.Form["selectedItemID"], player);

                return new JsonResult(new {
                    success = success, message = "Success",
                    clothingIconURL = ClothingIconURLs[player],
                });
            }
            else if (Request.Form["func"] == "updateVillagers") {
                bool success = UpdateVillagers(Request.Form["selectedVillagerIDs"]);
                return new JsonResult(new {
                    success = success, message = "Success",
                    villagerIconURL = VillagerIconURL,
                });
            }
            else {
                IFormFile fileUpload = Request.Form.Files.FirstOrDefault();

                if (fileUpload != null && fileUpload.Length > 0) {
                    using (var memoryStream = new MemoryStream()) {
                        await fileUpload.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;
                        SaveFile = memoryStream.ToArray();
                    }
                }
                else {
                    string testsave_filepath = "acgc-se/test-saves/chubby_bub.gci";
                    SaveFile = System.IO.File.ReadAllBytes(testsave_filepath);
                }
                string JsonDatastr = JsonData.RootElement.ToString();
                return new JsonResult(new {
                    success = true, message = "Success",
                    save_file = SaveFile,
                    json = SaveJsonText,
                });
            }
        }
        public bool UpdateItem(string selectedItemID, int player)
        {
            var SelectedItem = new Item();

            foreach (var item in Items) {
                if (item.ID == selectedItemID) {
                    SelectedItems[player] = item;
                    SelectedItem = item;
                    break;
                }
            }
            ItemIconURLs[player] = SelectedItem?.ImageURL ?? "acgc-se/ui-icons/None.png";
            InventoryIconURLs[player] = SelectedItem?.IconURLUpscaled ?? "";
            return true;
        }
        public bool UpdateHandheld(string selectedItemID, int player)
        {
            var SelectedHandheld = new Item();

            foreach (var item in Items) {
                if (item.ID == selectedItemID) {
                    SelectedHandhelds[player] = item;
                    SelectedHandheld = item;
                    break;
                }
            }
            HandheldIconURLs[player] = SelectedHandheld?.ImageURL ?? "acgc-se/ui-icons/None.png";
            return true;
        }
        public bool UpdateClothing(string selectedItemID, int player)
        {
            var SelectedClothing = new Item();

            foreach (var item in Items) {
                if (item.ID == selectedItemID) {
                    SelectedClothings.Add(item);
                    SelectedClothing = item;
                    break;
                }
            }
            ClothingIconURLs[player] = SelectedClothing?.ImageURL ?? "acgc-se/ui-icons/None.png";
            return true;
        }
        public bool UpdateVillagers(string selectedVillagerIDs)
        {
            string[] IDs = selectedVillagerIDs.Split(',');

            foreach (var villagerID in IDs) {
                foreach (var villager in Villagers) {
                    if (villager.ID == villagerID) {
                        SelectedVillagers.Add(villager);
                    }
                }
            }
            return true;
        }
        public (int index,int size) GetIndex(string name)
        {
            JsonElement saveInfoElement = JsonData.RootElement.GetProperty(name);
            string offsetHex = saveInfoElement.GetProperty("Global Byte Offset").GetString();
            int offset = Convert.ToInt32(offsetHex, 16);
            int size = saveInfoElement.GetProperty("Byte Size").GetInt32();
            return (offset,size);
        }
    }
}
