using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TOTK.Website.Models;
using TOTK.Website.Services;

namespace TOTK.Website.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public JsonService WeaponService;
        public IEnumerable<Weapon> Weapons { get; private set; }
        public string SelectedWeapon { get; set; }

        // All services needed are arguments in this constructor
        public IndexModel(
            ILogger<IndexModel> logger,
            JsonService weaponService)
        {
            _logger = logger;
            WeaponService = weaponService;
        }

        public void OnGet()
        {
            Weapons = WeaponService.GetWeapons();
        }
        public IActionResult OnPostUpdateSelectedWeapon(string selectedWeapon)
        {
            SelectedWeapon = selectedWeapon;
            return new JsonResult("success");
        }
    }
}