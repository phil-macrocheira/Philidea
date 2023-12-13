using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TOTK.Website.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace TOTK.Website.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ImportData _importData;
        private readonly CalculateDamage _calculateDamage;
        public InputModel Input { get; set; } = new InputModel();
        public int DamageOutput { get; set; }
        public IEnumerable<Weapon>? Weapons { get; private set; } = Enumerable.Empty<Weapon>();
        public IEnumerable<Fuse>? Fuses { get; private set; } = Enumerable.Empty<Fuse>();
        public IEnumerable<Enemy>? Enemies { get; private set; } = Enumerable.Empty<Enemy>();
        public Weapon? SelectedWeapon { get; set; } = new Weapon();
        public Fuse? SelectedFuse { get; set; } = new Fuse();
        public Enemy? SelectedEnemy { get; set; } = new Enemy();
        public string SelectedWeaponName { get; set; } = "Master Sword";
        public string SelectedFuseName { get; set; } = "Stone Talus Heart";
        public string SelectedEnemyName { get; set; } = "Chuchu (Small)";
        public string? WeaponIconURL { get; set; } = "";
        public string? FuseIconURL { get; set; } = "";
        public string? EnemyIconURL { get; set; } = "";
        public IndexModel(ILogger<IndexModel> logger, ImportData importData, CalculateDamage calculateDamage)
        {
            _logger = logger;
            _importData = importData;
            _calculateDamage = calculateDamage;
        }

        public IActionResult OnGet()
        {
            LoadData();
            DamageOutput = _calculateDamage.Calculate(this);
            return Page();
        }
        public IActionResult OnPost(string source)
        {
            _logger.LogInformation("OnPost() Called");

            var methodName = $"OnPost{source}";
            var methodInfo = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (methodInfo != null)
            {
                var result = (JsonResult)methodInfo.Invoke(this, null);
                return result;
            }

            return new JsonResult(new { success = true, message = "Success", });
        }
        public IActionResult OnPostUpdateInput()
        {
            Input.AttackUpMod = Convert.ToInt32(Request.Form["AttackUpMod"]);
            Input.AttackType = Request.Form["AttackType"];
            Input.LowDurability = Request.Form["LowDurability"].Count > 0;
            Input.Multishot = Request.Form["Multishot"].Count > 0;
            Input.Wet = Request.Form["Wet"].Count > 0;
            Input.EnemyCondition = Request.Form["EnemyCondition"];
            Input.HP = Convert.ToSingle(Request.Form["HP"]);
            Input.Buff1 = Request.Form["Buff1"];
            Input.Buff2 = Request.Form["Buff2"];

            DamageOutput = _calculateDamage.Calculate(this);

            return new JsonResult(new { success = true, message = "Success",
                AttackUpMod = Input.AttackUpMod, AttackType = Input.AttackType, LowDurability = Input.LowDurability, Multishot = Input.Multishot, 
                Wet = Input.Wet, EnemyCondition = Input.EnemyCondition, HP = Input.HP, Buff1 = Input.Buff1, Buff2 = Input.Buff2, DamageOutput = DamageOutput,
            });
        }
        public IActionResult OnPostUpdateWeapon()
        {
            SelectedWeapon.ActorID = Request.Form["ActorID"];
            SelectedWeapon.Name = Request.Form["Name"];
            SelectedWeapon.Type = Convert.ToByte(Request.Form["Type"]);
            if (bool.TryParse(Request.Form["CanCut"], out bool CanCutValue)) { SelectedWeapon.CanCut = CanCutValue; }
            SelectedWeapon.BaseAttack = Convert.ToByte(Request.Form["BaseAttack"]);
            SelectedWeapon.Durability = Convert.ToByte(Request.Form["Durability"]);
            SelectedWeapon.Property = Request.Form["Property"];
            if (bool.TryParse(Request.Form["CanFuseTo"], out bool CanFuseToValue)) { SelectedWeapon.CanFuseTo = CanFuseToValue; }
            SelectedWeapon.FuseExtraDurability = Convert.ToByte(Request.Form["FuseExtraDurability"]);
            SelectedWeapon.FuseBaseName = Request.Form["FuseBaseName"];
            SelectedWeapon.IconURL = Request.Form["IconURL"];

            DamageOutput = _calculateDamage.Calculate(this);

            return new JsonResult(new { success = true, message = "Success",
                Name = SelectedWeapon.Name, DamageOutput = DamageOutput,
            });
        }
        public IActionResult OnPostFuse() 
        {
            SelectedFuse.ActorID = Request.Form["ActorID"];
            SelectedFuse.Name = Request.Form["Name"];
            SelectedFuse.BaseAttack = Convert.ToByte(Request.Form["BaseAttack"]);
            if (bool.TryParse(Request.Form["CanCut"], out bool CanCutValue)) { SelectedFuse.CanCut = CanCutValue; }
            if (bool.TryParse(Request.Form["AddsShieldAttack"], out bool AddsShieldAttackValue)) { SelectedFuse.AddsShieldAttack = AddsShieldAttackValue; }
            if (bool.TryParse(Request.Form["ReplaceProperties"], out bool ReplacePropertiesValue)) { SelectedFuse.ReplaceProperties = ReplacePropertiesValue; }
            SelectedFuse.Property1 = Request.Form["Property1"];
            SelectedFuse.Property2 = Request.Form["Property2"];
            SelectedFuse.Property3 = Request.Form["Property3"];
            SelectedFuse.Property4 = Request.Form["Property4"];
            SelectedFuse.IconURL = Request.Form["IconURL"];

            DamageOutput = _calculateDamage.Calculate(this);

            return new JsonResult(new { success = true, message = "Success",
                Name = SelectedFuse.Name, DamageOutput = DamageOutput,
            });
        }
        public IActionResult OnPostEnemy()
        {
            SelectedEnemy.ActorID = Request.Form["ActorID"];
            SelectedEnemy.Name = Request.Form["Name"];
            if (bool.TryParse(Request.Form["HasGloomVariant"], out bool HasGloomVariantValue)) { SelectedEnemy.HasGloomVariant = HasGloomVariantValue; }
            SelectedEnemy.HP = Convert.ToInt16(Request.Form["HP"]);
            SelectedEnemy.FireDamage = Convert.ToInt16(Request.Form["FireDamage"]);
            SelectedEnemy.FireDamageContinuous = Convert.ToInt16(Request.Form["FireDamageContinuous"]);
            SelectedEnemy.IceDamage = Convert.ToInt16(Request.Form["IceDamage"]);
            SelectedEnemy.ShockDamage = Convert.ToInt16(Request.Form["ShockDamage"]);
            SelectedEnemy.WaterDamage = Convert.ToByte(Request.Form["WaterDamage"]);
            if (bool.TryParse(Request.Form["WeakToWater"], out bool WeakToWaterValue)) { SelectedEnemy.WeakToWater = WeakToWaterValue; }
            SelectedEnemy.RijuDamage = Convert.ToInt16(Request.Form["RijuDamage"]);
            if (bool.TryParse(Request.Form["AncientBladeDefeat"], out bool AncientBladeDefeatValue)) { SelectedEnemy.AncientBladeDefeat = AncientBladeDefeatValue; }
            if (bool.TryParse(Request.Form["IsRock"], out bool IsRockValue)) { SelectedEnemy.IsRock = IsRockValue; }
            if (bool.TryParse(Request.Form["CanSneakstrike"], out bool CanSneakstrikeValue)) { SelectedEnemy.CanSneakstrike = CanSneakstrikeValue; }
            SelectedEnemy.ArrowMultiplier = Convert.ToSingle(Request.Form["ArrowMultiplier"]);
            SelectedEnemy.BeamMultiplier = Convert.ToSingle(Request.Form["BeamMultiplier"]);
            SelectedEnemy.BombMultiplier = Convert.ToSingle(Request.Form["BombMultiplier"]);
            SelectedEnemy.IconURL = Request.Form["IconURL"];

            DamageOutput = _calculateDamage.Calculate(this);

            return new JsonResult(new { success = true, message = "Success",
                Name = SelectedEnemy.Name, DamageOutput = DamageOutput,
            });
        }
        public void LoadData()
        {
            Weapons = _importData.LoadWeapons();
            SelectedWeapon = Weapons?.FirstOrDefault(w => w.Name == SelectedWeaponName);
            WeaponIconURL = SelectedWeapon?.IconURL;

            Fuses = _importData.LoadFuse();
            SelectedFuse = Fuses?.FirstOrDefault(w => w.Name == SelectedFuseName);
            FuseIconURL = SelectedFuse?.IconURL;

            Enemies = _importData.LoadEnemies();
            SelectedEnemy = Enemies?.FirstOrDefault(w => w.Name == SelectedEnemyName);
            EnemyIconURL = SelectedEnemy?.IconURL;
        }
    }
}