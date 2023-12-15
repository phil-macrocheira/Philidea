using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TOTK.Website.Models;

namespace TOTK.Website.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ImportData _importData;
        private readonly CalculateDamage _calculateDamage;
        public InputModel Input { get; set; } = new InputModel();
        public float DamageOutput { get; set; }
        public List<string> Properties { get; set; } = new List<string>();
        public IEnumerable<Weapon>? Weapons { get; private set; } = Enumerable.Empty<Weapon>();
        public IEnumerable<Fuse>? Fuses { get; private set; } = Enumerable.Empty<Fuse>();
        public IEnumerable<Fuse>? FusesArrow { get; private set; } = Enumerable.Empty<Fuse>();
        public IEnumerable<Enemy>? Enemies { get; private set; } = Enumerable.Empty<Enemy>();
        public Weapon? SelectedWeapon { get; set; } = new Weapon();
        public Fuse? SelectedFuse { get; set; } = new Fuse();
        public Enemy? SelectedEnemy { get; set; } = new Enemy();
        public string SelectedWeaponName { get; set; } = "Master Sword";
        public string SelectedFuseName { get; set; } = "None";
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
        public IActionResult OnPost()
        {
            Input.AttackUpMod = Convert.ToInt32(Request.Form["AttackUpMod"]);
            Input.AttackType = Request.Form["AttackType"];
            Input.LowDurability = Request.Form["LowDurability"] == "true";
            Input.Multishot = Request.Form["Multishot"] == "true";
            Input.Wet = Request.Form["Wet"] == "true";
            Input.Frozen = Request.Form["Frozen"] == "true";
            Input.HP = Convert.ToSingle(Request.Form["PlayerHP"]);
            Input.Buff1 = Request.Form["Buff1"];
            Input.Buff2 = Request.Form["Buff2"];
            
            SelectedWeapon.Name = Request.Form["NameWeapon"];
            SelectedWeapon.Type = Convert.ToByte(Request.Form["Type"]);
            if (bool.TryParse(Request.Form["CanCutWeapon"], out bool CanCutValueWeapon)) { SelectedWeapon.CanCut = CanCutValueWeapon; }
            SelectedWeapon.BaseAttack = Convert.ToByte(Request.Form["BaseAttackWeapon"]);
            SelectedWeapon.ProjectileAttack = Convert.ToByte(Request.Form["ProjectileAttackWeapon"]);
            SelectedWeapon.Durability = Convert.ToByte(Request.Form["Durability"]);
            SelectedWeapon.Property = Request.Form["Property"];
            if (bool.TryParse(Request.Form["CanHaveAttackUpMod"], out bool CanHaveAttackUpModValue)) { SelectedWeapon.CanHaveAttackUpMod = CanHaveAttackUpModValue; }
            if (bool.TryParse(Request.Form["CanFuseTo"], out bool CanFuseToValue)) { SelectedWeapon.CanFuseTo = CanFuseToValue; }
            SelectedWeapon.FuseExtraDurability = Convert.ToByte(Request.Form["FuseExtraDurability"]);
            SelectedWeapon.FuseBaseName = Request.Form["FuseBaseName"];

            SelectedFuse.Name = Request.Form["NameFuse"];
            SelectedFuse.BaseAttack = Convert.ToByte(Request.Form["BaseAttackFuse"]);
            SelectedFuse.ProjectileAttack = Convert.ToByte(Request.Form["ProjectileAttackFuse"]);
            SelectedFuse.ElementPower = Convert.ToByte(Request.Form["ElementPower"]);
            if (bool.TryParse(Request.Form["CanFuseToArrow"], out bool CanFuseToArrowValue)) { SelectedFuse.CanFuseToArrow = CanFuseToArrowValue; }
            SelectedFuse.ArrowMultiplier = Convert.ToByte(Request.Form["ArrowMultiplierFuse"]);
            if (bool.TryParse(Request.Form["CanCutFuse"], out bool CanCutValueFuse)) { SelectedFuse.CanCut = CanCutValueFuse; }
            if (bool.TryParse(Request.Form["AddsShieldAttack"], out bool AddsShieldAttackValue)) { SelectedFuse.AddsShieldAttack = AddsShieldAttackValue; }
            if (bool.TryParse(Request.Form["ReplaceProperties"], out bool ReplacePropertiesValue)) { SelectedFuse.ReplaceProperties = ReplacePropertiesValue; }
            SelectedFuse.Property1 = Request.Form["Property1"];
            SelectedFuse.Property2 = Request.Form["Property2"];
            SelectedFuse.Property3 = Request.Form["Property3"];
            SelectedFuse.Property4 = Request.Form["Property4"];

            SelectedEnemy.Name = Request.Form["NameEnemy"];
            if (bool.TryParse(Request.Form["HasGloomVariant"], out bool HasGloomVariantValue)) { SelectedEnemy.HasGloomVariant = HasGloomVariantValue; }
            SelectedEnemy.HP = Convert.ToInt16(Request.Form["EnemyHP"]);
            SelectedEnemy.Element = Request.Form["Element"];
            SelectedEnemy.FireDamage = Convert.ToInt16(Request.Form["FireDamage"]);
            SelectedEnemy.FireDamageContinuous = Convert.ToInt16(Request.Form["FireDamageContinuous"]);
            if (bool.TryParse(Request.Form["CanFreeze"], out bool CanFreezeValue)) { SelectedEnemy.CanFreeze = CanFreezeValue; }
            SelectedEnemy.IceDamage = Convert.ToInt16(Request.Form["IceDamage"]);
            SelectedEnemy.ShockDamage = Convert.ToInt16(Request.Form["ShockDamage"]);
            SelectedEnemy.WaterDamage = Convert.ToByte(Request.Form["WaterDamage"]);
            SelectedEnemy.RijuDamage = Convert.ToInt16(Request.Form["RijuDamage"]);
            if (bool.TryParse(Request.Form["AncientBladeDefeat"], out bool AncientBladeDefeatValue)) { SelectedEnemy.AncientBladeDefeat = AncientBladeDefeatValue; }
            if (bool.TryParse(Request.Form["IsRock"], out bool IsRockValue)) { SelectedEnemy.IsRock = IsRockValue; }
            if (bool.TryParse(Request.Form["CanSneakstrike"], out bool CanSneakstrikeValue)) { SelectedEnemy.CanSneakstrike = CanSneakstrikeValue; }
            SelectedEnemy.ArrowMultiplier = Convert.ToSingle(Request.Form["ArrowMultiplierEnemy"]);
            SelectedEnemy.BeamMultiplier = Convert.ToSingle(Request.Form["BeamMultiplier"]);
            SelectedEnemy.BombMultiplier = Convert.ToSingle(Request.Form["BombMultiplier"]);

            GetProperties();
            DamageOutput = _calculateDamage.Calculate(this);

            //_logger.LogInformation($"{SelectedWeapon.Property} {SelectedFuse.Property1}");

            return new JsonResult(new { success = true, message = "Success",
                DamageOutput = DamageOutput, Properties = Properties,
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

            FusesArrow = _importData.LoadFuseArrow();
            SelectedFuse = FusesArrow?.FirstOrDefault(w => w.Name == SelectedFuseName);
            FuseIconURL = SelectedFuse?.IconURL;
        }
        public void GetProperties()
        {
            Properties.Clear();
            if (SelectedFuse?.ReplaceProperties.GetValueOrDefault() == true)
            {
                if (SelectedFuse?.CanCut.GetValueOrDefault() == true)
                {
                    Properties.Add("Cut");
                }
            }
            else
            {
                if (SelectedWeapon?.CanCut.GetValueOrDefault() == true)
                {
                    Properties.Add("Cut");
                }
            }
            if (SelectedWeapon.Property != "-")
            {
                if ((SelectedWeapon.Property == "Shatter Rock" && SelectedFuse?.ReplaceProperties.GetValueOrDefault() == true) &&
                   (SelectedFuse.Property1 != "Shatter Rock" && SelectedFuse.Property2 != "Shatter Rock" &&
                    SelectedFuse.Property3 != "Shatter Rock" && SelectedFuse.Property4 != "Shatter Rock")) {
                    // Don't add Shatter property if the fuse replaces properties and doesn't have Shatter
                }
                else {
                    Properties.Add(SelectedWeapon.Property);
                }
            }

            if (SelectedFuse.Property1 != "-" && SelectedFuse.Property1 != SelectedWeapon.Property) { Properties.Add(SelectedFuse.Property1); }
            if (SelectedFuse.Property2 != "-" && SelectedFuse.Property2 != SelectedWeapon.Property) { Properties.Add(SelectedFuse.Property2); }
            if (SelectedFuse.Property3 != "-" && SelectedFuse.Property3 != SelectedWeapon.Property) { Properties.Add(SelectedFuse.Property3); }
            if (SelectedFuse.Property4 != "-" && SelectedFuse.Property4 != SelectedWeapon.Property) { Properties.Add(SelectedFuse.Property4); }

            if (Properties.Count == 0)
            {
                Properties.Add("None");
            }
        }
    }
}