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
        public bool BlueDamageNum { get; set; } = false;
        public string FusedName { get; set; } = "Master Sword";
        public int AttackPowerUI { get; set; } = 30;
        public bool Defeated { get; set; } = true;
        public string Formula { get; set; }
        public List<string> Properties { get; set; } = new List<string>();
        public List<int> damageNumList { get; set; } = new List<int>();
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
            Input.CriticalHitMod = Request.Form["CriticalHitMod"] == "true";
            Input.Multishot = Request.Form["Multishot"] == "true";
            Input.Zonaite = Request.Form["Zonaite"] == "true";
            Input.SageWill = Request.Form["SageWill"] == "true";
            Input.AttackType = Request.Form["AttackType"];
            Input.Wet = Request.Form["Wet"] == "true";
            Input.Headshot = Request.Form["Headshot"] == "true";
            Input.Frozen = Request.Form["Frozen"] == "true";
            Input.Weakened = Request.Form["Weakened"] == "true";
            Input.Fence = Request.Form["Fence"] == "true";
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
            SelectedFuse.MineruDurability = Convert.ToByte(Request.Form["MineruDurability"]);
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
            BlueDamageNum = (_calculateDamage.GerudoBonus > 1 || _calculateDamage.ZonaiBonus > 0 || _calculateDamage.LowHealth > 1 || _calculateDamage.LowDurability > 1 || _calculateDamage.WetPlayer > 1);
            DamageOutput = _calculateDamage.Calculate(this);

            // Clamp Frox HP
            if (SelectedEnemy.Name == "Frox" && DamageOutput > 140)
                DamageOutput = 140;
            if (SelectedEnemy.Name == "Obsidian Frox" && DamageOutput > 270)
                DamageOutput = 270;
            if (SelectedEnemy.Name == "Blue-White Frox" && DamageOutput > 420)
                DamageOutput = 420;

            // Electric Chuchu Charged Instakill
            if (SelectedEnemy.Name.IndexOf("Electric Chuchu") != -1 && Input.Weakened && DamageOutput >= 1) {
                DamageOutput = (float)SelectedEnemy.HP;
            }

            // Defeated Check
            if (DamageOutput >= SelectedEnemy.HP)
                Defeated = true;
            else
                Defeated = false;

            float TotalBaseAttack = _calculateDamage.BaseAttack + _calculateDamage.AttackUpMod;
            float TotalFuseAttack = _calculateDamage.FuseBaseAttack * _calculateDamage.GerudoBonus + _calculateDamage.ZonaiBonus;
            float TotalAttack = TotalBaseAttack + TotalFuseAttack;

            // FORMULA
            Formula = $"Formula: BaseAttack({_calculateDamage.BaseAttack})";
            if (SelectedFuse.Name != "None") {
                Formula += $" + FuseUIAdjust(FuseBaseAttack({_calculateDamage.FuseBaseAttack})";
                if (_calculateDamage.GerudoBonus > 1) { Formula += $" * GerudoBonus({_calculateDamage.GerudoBonus})"; }
                if (_calculateDamage.AttackUpMod > 0) { Formula += $" + AttackUpMod({_calculateDamage.AttackUpMod})"; }
                if (_calculateDamage.ZonaiBonus > 0) { Formula += $" + ZonaiBonus({_calculateDamage.ZonaiBonus})"; }
                Formula += ")";
            }
            else if (_calculateDamage.AttackUpMod > 0) { Formula += $" + AttackUpMod({_calculateDamage.AttackUpMod})"; }
            if (_calculateDamage.LowHealth > 1) { Formula += $" * LowHealth({_calculateDamage.LowHealth})"; }
            if (_calculateDamage.WetPlayer > 1) { Formula += $" * WetPlayer({_calculateDamage.WetPlayer})"; }
            if (_calculateDamage.Sneakstrike > 1) { Formula += $" * Sneakstrike({_calculateDamage.Sneakstrike})"; }
            if (_calculateDamage.LowDurability > 1) { Formula += $" * LowDurability({_calculateDamage.LowDurability})"; }
            if (_calculateDamage.Bone > 1) { Formula += $" * Bone({_calculateDamage.Bone})"; }
            if (_calculateDamage.FlurryRush > 1) { Formula += $" * FlurryRush({_calculateDamage.FlurryRush})"; }
            if (_calculateDamage.Shatter > 1) { Formula += $" * Shatter({_calculateDamage.Shatter})"; }
            if (_calculateDamage.AttackUp > 1) { Formula += $" * AttackUp({_calculateDamage.AttackUp})"; }
            if (_calculateDamage.Headshot > 1) { Formula += $" * Headshot({_calculateDamage.Headshot})"; }
            if (_calculateDamage.Throw > 1) { Formula += $" * Throw({_calculateDamage.Throw})"; }
            if (_calculateDamage.OneDurability > 1) { Formula += $" * OneDurability({_calculateDamage.OneDurability})"; }
            if (_calculateDamage.Frozen > 1) { Formula += $" * Frozen({_calculateDamage.Frozen})"; }
            if (_calculateDamage.TreeCutter > 1) { Formula += $" * TreeCutter({_calculateDamage.TreeCutter}))"; }
            if (_calculateDamage.ArrowEnemyMult > 1) { Formula += $" * ArrowEnemyMult({_calculateDamage.ArrowEnemyMult})"; }
            if (_calculateDamage.CriticalHit > 1) { Formula += $" * CriticalHit({_calculateDamage.CriticalHit})"; }
            if (_calculateDamage.DemonDragon > 1) { Formula += $" * DemonDragon({_calculateDamage.DemonDragon})"; }
            if (_calculateDamage.ElementalDamage > 0) { Formula += $" + ElementalDamage({_calculateDamage.ElementalDamage})"; }
            if (_calculateDamage.ElementalMult > 1) { Formula += $"; Multiply result by ElementalMult({_calculateDamage.ElementalMult})"; }
            if (_calculateDamage.ContinuousFire > 0) { Formula += $" + ContinuousFire({_calculateDamage.ContinuousFire})"; }

            //$"Base({TotalBaseAttack}) + Fuse({TotalFuseAttack}) = {TotalAttack}";

            return new JsonResult(new {
                success = true,
                message = "Success",
                attackPowerUI = AttackPowerUI,
                DamageOutput = DamageOutput,
                Properties = Properties,
                FusedName = FusedName,
                Formula = Formula,
                damageNumList = damageNumList,
                blueDamageNum = BlueDamageNum,
                defeated = Defeated,
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

            if (SelectedWeapon?.Type != 2 && SelectedWeapon?.Type != 3) {
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

            // Don't add Bone or Melee Projectile from fuse if weapon is bow
            if (SelectedWeapon.Type == 3) {
                if (SelectedFuse.Property1 == "Bone" || SelectedFuse.Property1 == "Melee Projectile") {
                    SelectedFuse.Property1 = "-";
                }
                if (SelectedFuse.Property2 == "Bone" || SelectedFuse.Property2 == "Melee Projectile") {
                    SelectedFuse.Property2 = "-";
                }
                if (SelectedFuse.Property3 == "Bone" || SelectedFuse.Property3 == "Melee Projectile") {
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