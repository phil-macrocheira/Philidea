using System.Data.SqlTypes;
using TOTK.Website.Pages;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TOTK.Website
{
    public class CalculateDamage
    {
        private readonly ILogger<CalculateDamage> _logger;
        public totk_calculatorModel Data;
        private List<int> damageNumList { get; set; } = new List<int>();

        public float DamageOutput = 0;
        public string AttackType;
        public byte WeaponType;
        public string WeaponProperty;
        public float AttackPower;
        public float AttackUp;
        public float BaseAttack;
        public float AttackUpMod;
        public float FuseBaseAttack;
        public float ZonaiBonus;
        public float GerudoBonus;
        public float LowHealth;
        public float WetPlayer;
        public float Sneakstrike;
        public float LowDurability;
        public float OneDurability;
        public float Bone;
        public float FlurryRush;
        public float Shatter;
        public float Throw;
        public float Headshot;
        public float Frozen;
        public float TreeCutter;
        public float ArrowEnemyMult;
        public float ElementalDamage;
        public float ElementalMult;
        public float ContinuousFire;
        public float ComboFinisher;
        public float DemonDragon;
        public bool UsingFire;
        public bool UsingIce;
        public bool UsingShock;
        public bool UsingBomb;
        public bool UsingWater;
        public bool UsingBeam;
        public bool WindRazor;
        public float DamageBeforeElement;
        public CalculateDamage(ILogger<CalculateDamage> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public int CalculateAttackPowerUI(totk_calculatorModel data)
        {
            Data = data;
            BaseAttack = GetBaseAttack();
            float FuseAttackUI = GetFuseBaseAttack();
            AttackUpMod = (float)Data.Input.AttackUpMod;
            GerudoBonus = GetGerudoBonus();
            float ZonaiBonusUI = GetZonaiBonusUI();
            LowHealth = GetLowHealth();
            LowDurability = GetLowDurability();
            WetPlayer = GetWetPlayer();

            float BaseAttackUI = (WeaponUIAdjust(BaseAttack) + (FuseAttackUI * GerudoBonus) + AttackUpMod + ZonaiBonusUI) * LowHealth * LowDurability * WetPlayer;

            if (Data.SelectedWeapon.Type == 5) {
                return (int)BaseAttack;
            }
            return (int)Math.Floor(BaseAttackUI);
        }
        public float WeaponUIAdjust(float input)
        {
            switch (Data.SelectedWeapon.Type) {
                case 1:
                    return (float)Math.Floor(input * 0.95f);
                case 2:
                    return (float)Math.Ceiling(input * 1.326856f);
                default:
                    return (float)Math.Floor(input);
            }
        }
        public float GetZonaiBonusUI()
        {
            var SelectedWeapon = Data.SelectedWeapon.Name;
            bool ZonaiFuseProperty = ScanProperties("Zonai Fuse");

            if (ZonaiFuseProperty == false) {
                return 0;
            }

            switch (Data.SelectedWeapon.Property) {
                case "Zonai Lv1":
                    return 3;
                case "Zonai Lv2":
                    return 5;
                case "Zonai Lv3":
                    return 10;
                default:
                    return 0;
            }
        }
        public float Calculate(totk_calculatorModel data)
        {
            Data = data;
            AttackType = Data.Input.AttackType;
            WeaponType = (byte)Data.SelectedWeapon.Type;
            WeaponProperty = Data.SelectedWeapon.Property;
            UsingFire = GetUsingFire();
            UsingIce = GetUsingIce();
            UsingShock = GetUsingShock();
            UsingBomb = GetUsingBomb();
            UsingWater = GetUsingWater();
            UsingBeam = GetUsingBeam();
            AttackUp = GetAttackUp();
            FuseBaseAttack = GetFuseBaseAttack();
            ZonaiBonus = GetZonaiBonus();
            Sneakstrike = GetSneakstrike();
            OneDurability = GetOneDurability();
            Bone = GetBone();
            FlurryRush = GetFlurryRush();
            Shatter = GetShatter();
            Throw = GetThrow();
            Headshot = GetHeadshot();
            WindRazor = ScanProperties("Wind Razor");
            Frozen = GetFrozen();
            TreeCutter = GetTreeCutter();
            ArrowEnemyMult = GetArrowEnemyMult();
            ElementalDamage = GetElementalDamage();
            ElementalMult = GetElementalMult();
            ContinuousFire = GetContinuousFire();
            ComboFinisher = GetComboFinisher();
            DemonDragon = GetDemonDragon();
            bool IsBomb = ScanProperties("Bomb");
            bool IsChuchu = Data.SelectedEnemy.Name.IndexOf("Chuchu") != -1;
            bool IsPebblit = Data.SelectedEnemy.Name.IndexOf("Pebblit") != -1;
            bool IsGibdo = Data.SelectedEnemy.Name.IndexOf("Gibdo") != -1;
            float ProjectileDamage;

            AttackPower = (BaseAttack + (FuseBaseAttack * GerudoBonus) + AttackUpMod + ZonaiBonus);

            // Return enemy's HP if ancient blade or wind razor chuchu
            if ((Data.SelectedFuse.Name == "Ancient Blade" && Data.SelectedEnemy.AncientBladeDefeat == true) || (IsChuchu && WindRazor)) {
                return (float)Data.SelectedEnemy.HP;
            }

            // Pebblit Damage
            if (IsPebblit) {
                if (AttackType == "Master Sword Beam" || AttackType == "Sidon's Water") {
                    return 0;
                }
                if (Shatter > 1 && AttackType == "Throw") {
                    if (WeaponProperty == "Boomerang" && WeaponType == 0) {
                        return (float)Data.SelectedEnemy.HP / 2;
                    }
                    return (float)Data.SelectedEnemy.HP;
                }
                if (Shatter == 1.5f || IsBomb || AttackType == "Riju's Lightning") {
                    return (float)Data.SelectedEnemy.HP;
                }
                if (Shatter == 1.25f) {
                    return (float)Data.SelectedEnemy.HP / 2;
                }
                return 0;
            }

            if (IsGibdo) {
                if (!UsingFire && !UsingIce && !UsingShock && !UsingWater && Data.SelectedFuse.Property1 != "Dazzle" && !Data.Input.WeakenedGibdo) {
                    return 1;
                }
            }

            // Fire Chuchu Water Instakill
            if (Data.SelectedEnemy.Name.IndexOf("Fire Chuchu") != -1) {
                if (ScanProperties("Water") || AttackType == "Sidon's Water") {
                    return (float)Data.SelectedEnemy.HP;
                }
            }

            // MASTER SWORD BEAM
            if (AttackType == "Master Sword Beam") {
                float MasterSwordBeamUp = 1.0f;

                if (Data.Input.Buff1 == "Master Sword Beam Up" || Data.Input.Buff2 == "Master Sword Beam Up") {
                    MasterSwordBeamUp = 1.5f;
                }
                return (float)Data.SelectedWeapon.ProjectileAttack * MasterSwordBeamUp * AttackUp;
            }

            // Sidon's Water
            if (AttackType == "Sidon's Water") {
                float WaterMult = 1;
                if (Data.SelectedEnemy.Element == "Fire") {
                    WaterMult = 1.5f;
                }

                return (float)Math.Floor((BaseAttack + FuseUIAdjust((FuseBaseAttack * GerudoBonus) + AttackUpMod + ZonaiBonus)) * AttackUp * Frozen * WaterMult);
            }

            // Earthwake Technique / Throwing Materials
            if (Data.SelectedWeapon.Name == "None (Earthwake Technique)") {
                if (AttackType == "Throw") {
                    if (Data.SelectedEnemy.Name == "Evermean") {
                        return 0;
                    }
                    DamageOutput = FuseUIAdjust(FuseBaseAttack);
                    if (ElementalMult != 0) {
                        DamageOutput += ElementalDamage;
                        DamageOutput *= ElementalMult;
                    }
                    DamageOutput += ContinuousFire;
                    return DamageOutput;
                }
                return (float)(BaseAttack * AttackUp);
            }

            // MAIN DAMAGE FORMULA
            DamageOutput = BaseAttack + FuseUIAdjust((FuseBaseAttack * GerudoBonus) + AttackUpMod + ZonaiBonus);
            DamageOutput *= LowHealth * WetPlayer * Sneakstrike * LowDurability * Bone * FlurryRush * Shatter;
            DamageOutput *= AttackUp * Headshot * Throw * OneDurability * Frozen * TreeCutter;
            DamageOutput *= ArrowEnemyMult * ComboFinisher * DemonDragon;
            DamageBeforeElement = DamageOutput;
            if (ElementalMult != 0) {
                DamageOutput += ElementalDamage;
                DamageOutput *= ElementalMult;
            }
            DamageOutput += ContinuousFire;
            DamageOutput = (float)Math.Min(2147483647, Math.Floor(DamageOutput));

            ProjectileDamage = CreateDamageNumList((int)DamageOutput);
            return DamageOutput + ProjectileDamage;
        }
        public float GetBaseAttack()
        {
            float BaseAttack = (float)Data.SelectedWeapon.BaseAttack;
            string SelectedWeapon = Data.SelectedWeapon.Name;

            // DEMON KING'S BOW
            if (SelectedWeapon == "Demon King's Bow") {
                return (float)Math.Max(1, Math.Min(60, Math.Floor(Data.Input.HP) * 2));
            }

            return BaseAttack;
        }
        public float GetFuseBaseAttack()
        {
            float FuseBaseAttack = (float)Data.SelectedFuse.BaseAttack;

            switch (WeaponType) {
                case 3:
                    return FuseBaseAttack * (float)Data.SelectedFuse.ArrowMultiplier;
                case 4:
                    if (Data.SelectedFuse.AddsShieldAttack == true) {
                        return FuseBaseAttack;
                    }
                    return 0;
                default:
                    return FuseBaseAttack;
            }
        }
        public float FuseUIAdjust(float input)
        {
            switch (WeaponType) {
                case 1:
                    return (float)Math.Floor(input * 1.052632f);
                case 2:
                    return (float)Math.Ceiling(input * 0.7536613f);
                default:
                    return (float)Math.Floor(input);
            }
        }
        public bool ScanProperties(string search)
        {
            foreach (var property in Data.Properties) {
                if (property == search) {
                    return true;
                }
            }
            return false;
        }
        public float GetZonaiBonus()
        {
            var SelectedWeapon = Data.SelectedWeapon.Name;
            bool ZonaiFuseProperty = ScanProperties("Zonai Fuse");

            if (ZonaiFuseProperty == false) {
                return 0;
            }

            switch (Data.SelectedWeapon.Property) {
                case "Zonai Lv1":
                    return 3;
                case "Zonai Lv2":
                    if (SelectedWeapon == "Strong Zonaite Spear") {
                        return 4;
                    }
                    else {
                        return 5;
                    }
                case "Zonai Lv3":
                    if (SelectedWeapon == "Mighty Zonaite Spear") {
                        return 8;
                    }
                    else {
                        return 10;
                    }
                default:
                    return 0;
            }
        }
        public float GetGerudoBonus()
        {
            bool GerudoProperty = ScanProperties("Gerudo");

            if (GerudoProperty == true) {
                return 2;
            }
            return 1;
        }
        public float GetLowHealth()
        {
            bool LowHealthProperty = ScanProperties("Low Health x2");

            if (LowHealthProperty == true && Data.Input.HP <= 1) {
                return 2;
            }
            return 1;
        }
        public float GetWetPlayer()
        {
            bool WetProperty = ScanProperties("Wet x2");
            bool WaterFuse = ScanProperties("Water");

            if (WetProperty == true && (Data.Input.Wet == true || WaterFuse)) {
                return 2;
            }
            return 1;
        }
        public float GetSneakstrike()
        {
            if (AttackType == "Sneakstrike" && Data.Input.Frozen == false) {
                bool SneakstrikeProperty = ScanProperties("Sneakstrike x2");

                if (SneakstrikeProperty == true) {
                    return 16;
                }
                return 8;
            }
            return 1;
        }
        public float GetLowDurability()
        {
            bool LowDurabilityProperty = ScanProperties("Low Durability x2");

            if (LowDurabilityProperty == true && Data.Input.Durability <= 3) {
                return 2;
            }
            return 1;
        }
        public float GetOneDurability()
        {
            if (Data.Input.Durability == 1 && Data.Input.Frozen == false && AttackType != "Combo Finisher") {
                if ((AttackType != "Throw" || Data.SelectedWeapon.Property == "Boomerang") && AttackType != "Sneakstrike") {
                    return 2;
                }
            }
            return 1;
        }
        public float GetBone()
        {
            bool BoneProperty = ScanProperties("Bone");

            if (BoneProperty == true && (Data.Input.Buff1 == "Bone Weap. Prof." || Data.Input.Buff2 == "Bone Weap. Prof.")) {
                return 1.8f;
            }
            return 1;
        }
        public float GetFlurryRush()
        {
            bool FlurryRushProperty = ScanProperties("Flurry Rush x2");

            if (FlurryRushProperty == true && AttackType == "Flurry Rush") {
                return 2;
            }
            return 1;
        }
        public float GetShatter()
        {
            bool ShatterProperty = ScanProperties("Shatter Rock");

            if (UsingBomb) {
                return 1.5f;
            }

            if (ShatterProperty == true && Data.SelectedEnemy.IsRock == true) {
                switch (WeaponType) {
                    case 0:
                        return 1.25f; // 1H
                    case 1:
                        return 1.5f; // 2H
                    case 2:
                        return 1.25f; // Spears
                    case 3:
                        return 1.5f; // Bows
                    case 4:
                        return 1.5f; // Shields
                    default:
                        return 1;
                }
            }
            return 1;
        }
        public float GetAttackUp()
        {
            string Buff1 = Data.Input.Buff1;
            string Buff2 = Data.Input.Buff2;

            if (Buff1 == "Attack Up (Lv3)" || Buff2 == "Attack Up (Lv3)") {
                return 1.5f;
            }
            else if (Buff1 == "Attack Up (Lv2)" || Buff2 == "Attack Up (Lv2)") {
                return 1.3f;
            }
            else if (Buff1 == "Attack Up (Lv1)" || Buff2 == "Attack Up (Lv1)") {
                return 1.2f;
            }

            return 1;
        }
        public float GetHeadshot()
        {
            if (Data.Input.Headshot == true) {
                return (float)Data.SelectedEnemy.HeadshotMultiplier;
            }
            return 1;
        }
        public float GetComboFinisher()
        {
            if (Data.Input.AttackType == "Combo Finisher") {
                return 2;
            }
            return 1;
        }
        public float GetThrow()
        {
            bool ProjectileProperty = ScanProperties("Melee Projectile");

            if (AttackType != "Throw" || ProjectileProperty) {
                return 1;
            }

            if (WeaponProperty == "Boomerang") {
                return 1.5f * AttackUp;
            }

            return 2 * AttackUp;
        }
        public float GetFrozen()
        {
            var SelectedEnemy = Data.SelectedEnemy.Name;

            if (SelectedEnemy == "Gibdo" || SelectedEnemy == "Moth Gibdo") {
                return 1;
            }
            if (Data.Input.Frozen == true) {
                return 3;
            }
            return 1;
        }
        public float GetTreeCutter()
        {
            bool CutProperty = ScanProperties("Cut");
            bool TreeCutterProperty = ScanProperties("Tree Cutter");

            if (Data.SelectedEnemy.Name == "Evermean") {
                if (!CutProperty && !TreeCutterProperty) {
                    return 0;
                }
                if (TreeCutterProperty) {
                    return 3;
                }
            }
            return 1;
        }
        public float GetArrowEnemyMult()
        {
            if (WeaponType == 3) {
                return (float)Data.SelectedEnemy.ArrowMultiplier;
            }
            return 1;
        }
        public bool GetUsingFire()
        {
            if (Data.SelectedFuse.Name == "Time Bomb") {
                return false;
            }
            return ScanProperties("Fire") || ScanProperties("Fire Burst") || GetHotWeatherAttack() || ScanProperties("Bomb") || Data.Input.AttackType == "Riju's Lightning";
        }
        public bool GetUsingIce()
        {
            return ScanProperties("Ice") || ScanProperties("Ice Burst") || GetColdWeatherAttack();
        }
        public bool GetUsingShock()
        {
            return ScanProperties("Shock") || ScanProperties("Shock Burst") || GetStormyWeatherAttack() || Data.Input.AttackType == "Riju's Lightning";
        }
        public bool GetUsingBomb()
        {
            return ScanProperties("Bomb") || Data.Input.AttackType == "Riju's Lightning";
        }
        public bool GetUsingWater()
        {
            return ScanProperties("Water");
        }
        public bool GetUsingBeam()
        {
            return Data.SelectedFuse.Name == "Beam Emitter";
        }
        public float GetElementalDamage()
        {
            float ElementPower = 0;
            float FireDamage = (float)Data.SelectedEnemy.FireDamage;
            float IceDamage = (float)Data.SelectedEnemy.IceDamage;
            float ShockDamage = (float)Data.SelectedEnemy.ShockDamage;
            float HotWeatherPower = 0;
            float ColdWeatherPower = 0;
            float StormyWeatherPower = 0;
            if (GetHotWeatherAttack()) { HotWeatherPower = 5; }
            if (GetColdWeatherAttack()) { ColdWeatherPower = 5; }
            if (GetStormyWeatherAttack()) { StormyWeatherPower = 5; }

            if (WeaponType == 3 || AttackType == "Throw") {
                ElementPower = Data.SelectedFuse?.ElementPower ?? 0.0f;
            }

            if (UsingBomb) {
                if (WeaponType != 3 && Data.SelectedFuse.Name == "Cannon") {
                    ElementPower = 12;
                }
                ElementPower *= (float)Data.SelectedEnemy.BombMultiplier;
            }
            if (UsingIce) {
                if (Data.Input.Frozen == true) {
                    return 0;
                }
                return ElementPower + IceDamage + ColdWeatherPower * AttackUp;
            }
            if (UsingFire) {
                ElementPower += FireDamage + HotWeatherPower * AttackUp;
            }
            if (UsingShock) {
                ElementPower += ShockDamage + StormyWeatherPower * AttackUp;
            }
            if (UsingWater) {
                ElementPower += (float)Data.SelectedEnemy.WaterDamage;
            }
            if (UsingBeam) {
                ElementPower += (12 * (float)Data.SelectedEnemy.BeamMultiplier) * AttackUp;
            }
            return ElementPower;
        }
        public bool GetHotWeatherAttack()
        {
            string Buff1 = Data.Input.Buff1;
            string Buff2 = Data.Input.Buff2;
            return (Buff1 == "Hot Weather Attack" || Buff2 == "Hot Weather Attack");
        }
        public bool GetColdWeatherAttack()
        {
            string Buff1 = Data.Input.Buff1;
            string Buff2 = Data.Input.Buff2;
            return (Buff1 == "Cold Weather Attack" || Buff2 == "Cold Weather Attack");
        }
        public bool GetStormyWeatherAttack()
        {
            string Buff1 = Data.Input.Buff1;
            string Buff2 = Data.Input.Buff2;
            return (Buff1 == "Stormy Weather Attack" || Buff2 == "Stormy Weather Attack");
        }
        public float GetElementalMult()
        {
            string EnemyElement = Data.SelectedEnemy.Element;

            switch (EnemyElement) {
                case "Fire":
                    if (UsingIce) {
                        return 2;
                    }
                    if (UsingWater) {
                        return 1.5f;
                    }
                    if (UsingFire) {
                        return 0;
                    }
                    return 1;
                case "Ice":
                    if (UsingFire) {
                        return 2;
                    }
                    if (UsingIce) {
                        return 0;
                    }
                    return 1;
                case "Shock":
                    if (UsingShock) {
                        return 0;
                    }
                    return 1;
                default:
                    return 1;
            }
        }
        public float GetContinuousFire()
        {
            if (UsingFire && Data.SelectedEnemy.Element != "Fire") {
                return (float)Data.SelectedEnemy.FireDamageContinuous;
            }
            return 0;
        }
        public float GetDemonDragon()
        {
            string FuseBaseName = Data.SelectedWeapon.FuseBaseName;

            if (Data.SelectedEnemy.Name == "Demon Dragon" && (FuseBaseName == "Master Sword" || FuseBaseName == "Decayed Master Sword")) {
                return (float)Data.SelectedWeapon.BaseAttack * 5;
            }
            return 1;
        }
        public float CreateDamageNumList(int DamageOutput)
        {
            float WindRazorAttack = 10;
            bool WindRazorElement = ScanProperties("Fire") || ScanProperties("Ice") || ScanProperties("Shock");
            int MultishotCount = 0;
            int ExtraProjectileCount = 0;
            bool MeleeProjectile = ScanProperties("Melee Projectile");
            float ProjectileAttack = 0;
            float FirstProjectileAttack = 0;
            int RijuDamage = (int)Data.SelectedEnemy.RijuDamage;

            // WIND RAZOR
            if (WindRazor) {
                damageNumList.Add((int)DamageOutput);

                WindRazorAttack *= AttackUp;
                if (ElementalMult != 0 && WindRazorElement) {
                    WindRazorAttack += ElementalDamage;
                    WindRazorAttack *= ElementalMult;
                }
                WindRazorAttack += ContinuousFire;
                damageNumList.Add((int)Math.Floor(WindRazorAttack));

                Data.damageNumList = damageNumList;
                return WindRazorAttack;
            }

            // IF USING ICE
            if (UsingIce) {
                return 0;
            }

            // MULTISHOT
            if (Data.SelectedWeapon.Property == "Multishot x2") {
                MultishotCount = 2;
            }
            else if (Data.SelectedWeapon.Property == "Multishot x3" || Data.SelectedWeapon.Property == "Multishot x3-5") {
                MultishotCount = 3;
            }
            if (Data.Input.Multishot == true) {
                MultishotCount = 5;
            }
            if (MultishotCount > 0) {
                damageNumList.Add((int)DamageOutput);

                for (int i=0;i < MultishotCount-1;i++) {
                    if (UsingBomb || UsingWater || UsingBeam) {
                        damageNumList.Add((int)DamageOutput - (int)ContinuousFire);
                    }
                    else {
                        damageNumList.Add((int)DamageBeforeElement);
                    }
                }

                if (AttackType == "Riju's Lightning") {
                    damageNumList.Add((int)RijuDamage);
                }

                Data.damageNumList = damageNumList;

                if (UsingBomb || UsingWater || UsingBeam) {
                    return (DamageOutput - ContinuousFire) * (MultishotCount-1);
                }

                return DamageBeforeElement * (MultishotCount-1);
            }

            // MELEE PROJECTILE
            if (MeleeProjectile) {
                ProjectileAttack = (float)Data.SelectedFuse.ProjectileAttack;
                if (Data.SelectedWeapon.Property == "Rod") {
                    ProjectileAttack *= 2;
                    if (Data.SelectedFuse.Property1 != "Ice Burst") {
                        ExtraProjectileCount = 2;
                    }
                }

                damageNumList.Add((int)DamageOutput);

                if (ElementalMult != 0) {
                    FirstProjectileAttack = ProjectileAttack + ElementalDamage;
                    FirstProjectileAttack *= ElementalMult;
                }
                FirstProjectileAttack += ContinuousFire;
                FirstProjectileAttack = (float)Math.Floor(FirstProjectileAttack);
                damageNumList.Add((int)FirstProjectileAttack);

                for (int i=0;i<ExtraProjectileCount;i++) {
                    if (UsingWater) {
                        damageNumList.Add((int)FirstProjectileAttack);
                    }
                    else {
                        damageNumList.Add((int)ProjectileAttack);
                    }
                }

                Data.damageNumList = damageNumList;

                if (UsingWater) {
                    return FirstProjectileAttack + FirstProjectileAttack * ExtraProjectileCount;
                }
                return FirstProjectileAttack + ProjectileAttack * ExtraProjectileCount;
            }

            // RIJU'S LIGHTNING
            if (AttackType == "Riju's Lightning") {
                damageNumList.Add((int)DamageOutput);
                damageNumList.Add((int)RijuDamage);
                Data.damageNumList = damageNumList;
                return RijuDamage;
            }
            return 0;
        }
    }
}
