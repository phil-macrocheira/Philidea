using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.Metrics;
using TOTK.Website.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TOTK.Website.Pages
{
    public class totk_calculatorModel : PageModel
    {
        private readonly ILogger<totk_calculatorModel> _logger;
        private readonly ImportData _importData;
        private readonly CalculateDamage _calculateDamage;
        private readonly GetFusedName _getFusedName;
        public InputModel Input { get; set; } = new InputModel();
        public float DamageOutput { get; set; } = 30;
        public string FusedName { get; set; } = "Master Sword";
        public int AttackPowerUI { get; set; } = 30;
        public string Formula { get; set; }
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
        public totk_calculatorModel(ILogger<totk_calculatorModel> logger, ImportData importData, CalculateDamage calculateDamage, GetFusedName getFusedName)
        {
            _logger = logger;
            _importData = importData;
            _calculateDamage = calculateDamage;
            _getFusedName = getFusedName;
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
            Input.Durability = Convert.ToInt32(Request.Form["DurabilityInput"]);
            Input.AttackType = Request.Form["AttackType"];
            Input.Wet = Request.Form["Wet"] == "true";
            Input.Headshot = Request.Form["Headshot"] == "true";
            Input.Frozen = Request.Form["Frozen"] == "true";
            Input.HP = Convert.ToSingle(Request.Form["PlayerHP"]);
            Input.Buff1 = Request.Form["Buff1"];
            Input.Buff2 = Request.Form["Buff2"];

            SelectedWeapon.Name = Request.Form["NameWeapon"];
            SelectedWeapon.Type = Convert.ToByte(Request.Form["Type"]);
            if (bool.TryParse(Request.Form["CanCutWeapon"], out bool CanCutValueWeapon)) { SelectedWeapon.CanCut = CanCutValueWeapon; }
            SelectedWeapon.BaseAttack = Convert.ToByte(Request.Form["BaseAttackWeapon"]);
            SelectedWeapon.ProjectileAttack = Convert.ToByte(Request.Form["ProjectileAttackWeapon"]);
            SelectedWeapon.Durability = Convert.ToInt16(Request.Form["Durability"]);
            SelectedWeapon.Property = Request.Form["Property"];
            if (bool.TryParse(Request.Form["CanHaveAttackUpMod"], out bool CanHaveAttackUpModValue)) { SelectedWeapon.CanHaveAttackUpMod = CanHaveAttackUpModValue; }
            SelectedWeapon.FuseExtraDurability = Convert.ToByte(Request.Form["FuseExtraDurability"]);
            SelectedWeapon.FuseBaseName = Request.Form["FuseBaseName"];
            SelectedWeapon.NamingRule = Request.Form["NamingRuleWeapon"];

            SelectedFuse.Name = Request.Form["NameFuse"];
            SelectedFuse.BaseAttack = Convert.ToByte(Request.Form["BaseAttackFuse"]);
            SelectedFuse.ProjectileAttack = Convert.ToByte(Request.Form["ProjectileAttackFuse"]);
            SelectedFuse.ElementPower = Convert.ToByte(Request.Form["ElementPower"]);
            SelectedFuse.WeaponDurability = Convert.ToInt16(Request.Form["WeaponDurability"]);
            if (bool.TryParse(Request.Form["CanFuseToArrow"], out bool CanFuseToArrowValue)) { SelectedFuse.CanFuseToArrow = CanFuseToArrowValue; }
            SelectedFuse.ArrowMultiplier = Convert.ToByte(Request.Form["ArrowMultiplierFuse"]);
            if (bool.TryParse(Request.Form["CanCutFuse"], out bool CanCutValueFuse)) { SelectedFuse.CanCut = CanCutValueFuse; }
            if (bool.TryParse(Request.Form["AddsShieldAttack"], out bool AddsShieldAttackValue)) { SelectedFuse.AddsShieldAttack = AddsShieldAttackValue; }
            if (bool.TryParse(Request.Form["ReplaceProperties"], out bool ReplacePropertiesValue)) { SelectedFuse.ReplaceProperties = ReplacePropertiesValue; }
            SelectedFuse.Property1 = Request.Form["Property1"];
            SelectedFuse.Property2 = Request.Form["Property2"];
            SelectedFuse.Property3 = Request.Form["Property3"];
            SelectedFuse.NamingRule = Request.Form["NamingRuleFuse"];
            SelectedFuse.Adjective = Request.Form["Adjective"];
            SelectedFuse.BindTypeSword = Request.Form["BindTypeSword"];
            SelectedFuse.BindTypeSpear = Request.Form["BindTypeSpear"];

            SelectedEnemy.Name = Request.Form["NameEnemy"];
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
            if (bool.TryParse(Request.Form["CanMeleeHeadshot"], out bool CanMeleeHeadshotValue)) { SelectedEnemy.CanMeleeHeadshot = CanMeleeHeadshotValue; }
            SelectedEnemy.HeadshotMultiplier = Convert.ToSingle(Request.Form["HeadshotMultiplier"]);
            SelectedEnemy.ArrowMultiplier = Convert.ToSingle(Request.Form["ArrowMultiplierEnemy"]);
            SelectedEnemy.BeamMultiplier = Convert.ToSingle(Request.Form["BeamMultiplier"]);
            SelectedEnemy.BombMultiplier = Convert.ToSingle(Request.Form["BombMultiplier"]);

            GetProperties();
            FusedName = _getFusedName.GetName(this);

            AttackPowerUI = _calculateDamage.CalculateAttackPowerUI(this);
            DamageOutput = _calculateDamage.Calculate(this);

            float TotalBaseAttack = _calculateDamage.BaseAttack + _calculateDamage.AttackUpMod;
            float TotalFuseAttack = _calculateDamage.FuseBaseAttack * _calculateDamage.GerudoBonus + _calculateDamage.ZonaiBonus;
            float TotalAttack = TotalBaseAttack + TotalFuseAttack;

            Formula = $"(BaseAttack({_calculateDamage.BaseAttack}) + FuseUIAdjust((FuseBaseAttack({_calculateDamage.FuseBaseAttack}) * GerudoBonus({_calculateDamage.GerudoBonus})) + AttackUpMod({_calculateDamage.AttackUpMod}) + ZonaiBonus({_calculateDamage.ZonaiBonus})) * " +
                $"LowHealth({_calculateDamage.LowHealth}) * WetPlayer({_calculateDamage.WetPlayer}) * Sneakstrike({_calculateDamage.Sneakstrike}) * LowDurability({_calculateDamage.LowDurability}) * Bone({_calculateDamage.Bone}) * FlurryRush({_calculateDamage.FlurryRush}) * " +
                $"Shatter({_calculateDamage.Shatter}) * AttackUp({_calculateDamage.AttackUp}) * Headshot({_calculateDamage.Headshot}) * Throw({_calculateDamage.Throw}) * OneDurability({_calculateDamage.OneDurability}) * Frozen({_calculateDamage.Frozen}) * TreeCutter({_calculateDamage.TreeCutter}) * " +
                $"ArrowEnemyMult({_calculateDamage.ArrowEnemyMult}) * ComboFinisher({_calculateDamage.ComboFinisher}) * DemonDragon({_calculateDamage.DemonDragon}) + ElementalDamage({_calculateDamage.ElementalDamage}); " +
                $"Multiply result by ElementalMult({_calculateDamage.ElementalMult}) + ContinuousFire({_calculateDamage.ContinuousFire})" +
                $" ................................................... Base({TotalBaseAttack}) + Fuse({TotalFuseAttack}) = {TotalAttack}";

            return new JsonResult(new {
                success = true,
                message = "Success",
                attackPowerUI = AttackPowerUI,
                DamageOutput = DamageOutput,
                Properties = Properties,
                FusedName = FusedName,
                Formula = Formula,
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

            if (SelectedWeapon.Type == 5) {
                Properties.Add("None");
                return;
            }

            if (SelectedFuse?.ReplaceProperties.GetValueOrDefault() == true) {
                if (SelectedFuse?.CanCut.GetValueOrDefault() == true) {
                    Properties.Add("Cut");
                }
            }
            else {
                if (SelectedWeapon?.CanCut.GetValueOrDefault() == true) {
                    Properties.Add("Cut");
                }
            }

            // Don't add Wind Razor if not cut
            if (SelectedWeapon.Property == "Wind Razor" && Properties.Count == 0) {
                SelectedWeapon.Property = "-";
            }

            // Add weapon property
            if (SelectedWeapon.Property != "-") {
                if ((SelectedWeapon.Property == "Shatter Rock" && SelectedFuse?.ReplaceProperties.GetValueOrDefault() == true) &&
                   (SelectedFuse.Property1 != "Shatter Rock" && SelectedFuse.Property2 != "Shatter Rock" && SelectedFuse.Property3 != "Shatter Rock")) {
                    // Don't add Shatter property if the fuse replaces properties and doesn't have Shatter
                }
                else {
                    Properties.Add(SelectedWeapon.Property);
                }
            }

            // Don't add Bone from fuse if weapon is bow
            if (SelectedWeapon.Type == 3) {
                if (SelectedFuse.Property1 == "Bone") {
                    SelectedFuse.Property1 = "-";
                }
                if (SelectedFuse.Property2 == "Bone") {
                    SelectedFuse.Property2 = "-";
                }
                if (SelectedFuse.Property3 == "Bone") {
                    SelectedFuse.Property3 = "-";
                }
            }

            // Add fuse properties
            if (SelectedFuse.Property1 != "-" && SelectedFuse.Property1 != SelectedWeapon.Property) { Properties.Add(SelectedFuse.Property1); }
            if (SelectedFuse.Property2 != "-" && SelectedFuse.Property2 != SelectedWeapon.Property) { Properties.Add(SelectedFuse.Property2); }
            if (SelectedFuse.Property3 != "-" && SelectedFuse.Property3 != SelectedWeapon.Property) { Properties.Add(SelectedFuse.Property3); }

            if (Properties.Count == 0) {
                Properties.Add("None");
            }
        }
    }
}