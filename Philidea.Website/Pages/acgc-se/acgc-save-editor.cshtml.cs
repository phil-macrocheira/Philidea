using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Philidea.Website.Models;
using System.Text.Json;

namespace Philidea.Website.Pages
{
    public class Item
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string DropdownName { get; set; }
        public string ImageURL { get; set; }
        public string IconURL { get; set; }
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
    }
    public class Object
    {
        public string ID { get; set; }
        public string DropdownName { get; set; }
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
        public Item SelectedItem { get; set; }
        public string ItemIconURL { get; set; } = "";
        public string InventoryIconURL { get; set; } = "";
        public acgcModel(ILogger<acgcModel> logger)
        {
            _logger = logger;
            SaveJsonText = System.IO.File.ReadAllText("acgc-se/data/save_data.json");
            JsonData = JsonDocument.Parse(SaveJsonText);

            string ItemJsonText = System.IO.File.ReadAllText("acgc-se/data/item_data.json");
            Items = JsonSerializer.Deserialize<List<Item>>(ItemJsonText);
        }
        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (Request.Form["func"] == "updateItem") {
                bool success = UpdateItem(Request.Form["selectedItemID"]);
                return new JsonResult(new {
                    success = success, message = "Success", 
                    itemIconURL = ItemIconURL,
                    inventoryIconURL = InventoryIconURL,
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
        public bool UpdateItem(string selectedItemID)
        {
            foreach (var item in Items) {
                if (item.ID == selectedItemID) {
                    SelectedItem = item;
                    break;
                }
            }
            ItemIconURL = SelectedItem?.ImageURL ?? "";
            InventoryIconURL = SelectedItem?.IconURL ?? "";
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
