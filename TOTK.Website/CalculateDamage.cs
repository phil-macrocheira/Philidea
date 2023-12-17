using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TOTK.Website.Models;
using TOTK.Website.Pages;
using Microsoft.Extensions.Logging;
using System;

namespace TOTK.Website
{
    public class CalculateDamage
    {
        private readonly ILogger<CalculateDamage> _logger;
        public totk_calculatorModel Data;
        public CalculateDamage(ILogger<CalculateDamage> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public float Calculate(totk_calculatorModel data)
        {
            Data = data;

            float DamageOutput = 0;
            float BaseAttack = GetBaseAttack();
            float AttackUpMod = (float)Data.Input.AttackUpMod;
            float FuseBaseAttack = (float)GetFuseBaseAttack();
            float ZonaiBonus = (float)GetZonaiBonus();
            float GerudoBonus = (float)GetGerudoBonus();
            float LowHealth = (float)GetLowHealth();
            float WetPlayer = (float)GetWetPlayer();
            float Sneakstrike = (float)GetSneakstrike();
            float LowDurability = (float)GetLowDurability();
            float OneDurability = (float)GetOneDurability();
            float Bone = (float)GetBone();
            float FlurryRush = (float)GetFlurryRush();
            float Shatter = (float)GetShatter();
            float AttackUp = (float)GetAttackUp();
            float Throw = (float)GetThrow(AttackUp);
            float Headshot = (float)GetHeadshot();
            float Frozen = (float)GetFrozen();
            float TreeCutter = (float)GetTreeCutter();
            float ArrowEnemyMult = (float)GetArrowEnemyMult();
            float ElementalDamage = (float)GetElementalDamage();
            float ElementalMult = (float)GetElementalMult();
            float ContinuousFire = (float)GetContinuousFire();
            float ComboFinisher = (float)GetComboFinisher();
            float MoldugaBelly = (float)GetMoldugaBelly();

            // Return enemy's HP if ancient blade
            if (Data.SelectedFuse.Name == "Ancient Blade" && Data.SelectedEnemy.AncientBladeDefeat == true)
            {
                return (float)Data.SelectedEnemy.HP;
            }

            // Melee Projectile
            if (Data.Input.AttackType == "Melee Projectile")
            {
                // WIND RAZOR
                bool CutProperty = ScanProperties("Cut");
                if (Data.SelectedWeapon.Property == "Wind Razor" && CutProperty)
                {
                    DamageOutput = (10 + FuseUIAdjust(FuseBaseAttack + AttackUpMod)) * AttackUp;
                    DamageOutput = (float)Math.Ceiling(DamageOutput * MoldugaBelly);
                    DamageOutput += ElementalDamage;
                    DamageOutput *= ElementalMult;
                    DamageOutput += ContinuousFire;
                    return (float)Math.Min(2147483647, Math.Floor(DamageOutput));
                }

                // PROJECTILES FROM FUSES
                float ProjectileAttack = (float)Data.SelectedFuse.ProjectileAttack;
                if (ProjectileAttack > 0)
                {
                    float RodMultiplier = 1;
                    if (Data.SelectedWeapon.Property == "Rod")
                    {
                        RodMultiplier = 2;
                    }
                    DamageOutput = (ProjectileAttack * RodMultiplier) * AttackUp;
                    DamageOutput = (float)Math.Ceiling(DamageOutput * MoldugaBelly);
                    DamageOutput += ElementalDamage;
                    DamageOutput *= ElementalMult;
                    DamageOutput += ContinuousFire;
                    return (float)Math.Min(2147483647, Math.Floor(DamageOutput));
                }
            }

            // MASTER SWORD BEAM
            if (Data.Input.AttackType == "Throw" && Data.SelectedWeapon.FuseBaseName == "Master Sword")
            {
                float MasterSwordBeamUp = 1.0f;

                if (Data.Input.Buff1 == "Master Sword Beam Up" || Data.Input.Buff2 == "Master Sword Beam Up")
                {
                    MasterSwordBeamUp = 1.5f;
                }
                return (float)Data.SelectedWeapon.ProjectileAttack * MasterSwordBeamUp;
            }

            DamageOutput = BaseAttack + FuseUIAdjust((FuseBaseAttack * GerudoBonus) + AttackUpMod + ZonaiBonus);
            DamageOutput *= LowHealth * WetPlayer * Sneakstrike * LowDurability * Bone * FlurryRush * Shatter;
            DamageOutput *= AttackUp * Headshot * Throw * OneDurability * Frozen * TreeCutter;
            DamageOutput *= ArrowEnemyMult * ComboFinisher;
            DamageOutput = (float)Math.Ceiling(DamageOutput * MoldugaBelly);
            DamageOutput += ElementalDamage;
            DamageOutput *= ElementalMult;
            DamageOutput += ContinuousFire;

            _logger.LogInformation($"BaseAttack: {BaseAttack}, AttackUpMod: {AttackUpMod}, FuseBaseAttack: {FuseBaseAttack}, " +
                                   $"GerudoBonus: {GerudoBonus}, ZonaiBonus: {ZonaiBonus}, LowHealth: {LowHealth}, " +
                                   $"WetPlayer: {WetPlayer}, Sneakstrike: {Sneakstrike}, LowDurability: {LowDurability}, " +
                                   $"Bone: {Bone}, FlurryRush: {FlurryRush}, Shatter: {Shatter}, AttackUp: {AttackUp}, " +
                                   $"Headshot: {Headshot}, Throw: {Throw}, OneDurability: {OneDurability}, Frozen: {Frozen}, " +
                                   $"TreeCutter: {TreeCutter}, ArrowEnemyMult: {ArrowEnemyMult}, ComboFinisher: {ComboFinisher}, " +
                                   $"MoldugaBelly: {MoldugaBelly}, " + 
                                   $"ElementalDamage: {ElementalDamage}, ElementalMult: {ElementalMult}, ContinuousFire: {ContinuousFire}");

            return (float)Math.Min(2147483647, Math.Floor(DamageOutput));
        }
        public float GetBaseAttack()
        {
            float BaseAttack = (float)Data.SelectedWeapon.BaseAttack;
            string SelectedWeapon = Data.SelectedWeapon.Name;
            string FuseBaseName = Data.SelectedWeapon.FuseBaseName;

            // DEMON KING'S BOW
            if (SelectedWeapon == "Demon King's Bow")
            {
                return (float)Math.Max(1, Math.Min(60, Math.Floor(Data.Input.HP) * 2));
            }

            // MASTER SWORD IF DEMON DRAGON
            if (Data.SelectedEnemy.Name == "Demon Dragon" && (FuseBaseName == "Master Sword" || FuseBaseName == "Decayed Master Sword"))
            {
                return BaseAttack * 5;
            }

            return BaseAttack;
        }
        public float GetFuseBaseAttack()
        {
            float FuseBaseAttack = (float)Data.SelectedFuse.BaseAttack;

            switch (Data.SelectedWeapon.Type)
            {
                case 3:
                    return FuseBaseAttack * (float)Data.SelectedFuse.ArrowMultiplier;
                case 4:
                    if (Data.SelectedFuse.AddsShieldAttack == true)
                    {
                        return FuseBaseAttack;
                    }
                    return 0;
                default:
                    return FuseBaseAttack;
            }
        }
        public float FuseUIAdjust(float input)
        {
            switch (Data.SelectedWeapon.Type)
            {
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
            foreach (var property in Data.Properties)
            {
                if (property == search)
                {
                    _logger.LogInformation($"true");
                    return true;
                }
            }
            return false;
        }
        public float GetZonaiBonus()
        {
            var SelectedWeapon = Data.SelectedWeapon.Name;
            bool ZonaiFuseProperty = ScanProperties("Zonai Fuse");

            if (ZonaiFuseProperty == false)
            {
                return 0;
            }

            switch (Data.SelectedWeapon.Property)
            {
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

            if (LowHealthProperty == true && Data.Input.HP <= 1)
            {
                return 2;
            }
            return 1;
        }
        public float GetWetPlayer()
        {
            bool WetProperty = ScanProperties("Wet x2");
            bool WaterFuse = ScanProperties("Water");

            if (WetProperty == true && (Data.Input.Wet == true || WaterFuse))
            {
                return 2;
            }
            return 1;
        }
        public float GetSneakstrike()
        {
            if (Data.Input.AttackType == "Sneakstrike" && Data.SelectedEnemy.CanSneakstrike == true)
            {
                bool SneakstrikeProperty = ScanProperties("Sneakstrike x2");

                if (SneakstrikeProperty == true)
                {
                    return 16;
                }
                return 8;
            }
            return 1;
        }
        public float GetLowDurability()
        {
            bool LowDurabilityProperty = ScanProperties("Low Durability x2");

            if (LowDurabilityProperty == true && Data.Input.Durability <= 3)
            {
                return 2;
            }
            return 1;
        }
        public float GetOneDurability()
        {
            string AttackType = Data.Input.AttackType;

            if (Data.Input.Durability == 1 && Data.Input.Frozen == false && AttackType != "Combo Finisher")
            {
                if ((AttackType != "Throw" || Data.SelectedWeapon.Property == "Boomerang") && AttackType != "Sneakstrike")
                {
                    return 2;
                }
            }
            return 1;
        }
        public float GetBone()
        {
            bool BoneProperty = ScanProperties("Bone");

            if (BoneProperty == true && (Data.Input.Buff1 == "Bone Weap. Prof." || Data.Input.Buff2 == "Bone Weap. Prof."))
            {
                return 1.8f;
            }
            return 1;
        }
        public float GetFlurryRush()
        {
            bool FlurryRushProperty = ScanProperties("Flurry Rush x2");

            if (FlurryRushProperty == true && Data.Input.AttackType == "Flurry Rush")
            {
                return 2;
            }
            return 1;
        }
        public float GetShatter()
        {
            bool ShatterProperty = ScanProperties("Shatter Rock");
            byte WeaponType = (byte)Data.SelectedWeapon.Type;

            if (ShatterProperty == true && Data.SelectedEnemy.IsRock == true)
            {
                switch (WeaponType)
                {
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

            if (Buff1 == "Attack Up (Lv3)" || Buff2 == "Attack Up (Lv3)")
            {
                return 1.5f;
            }
            else if (Buff1 == "Attack Up (Lv2)" || Buff2 == "Attack Up (Lv2)")
            {
                return 1.3f;
            }
            else if (Buff1 == "Attack Up (Lv1)" || Buff2 == "Attack Up (Lv1)")
            {
                return 1.2f;
            }

            return 1;
        }
        public float GetHeadshot()
        {
            if ((Data.SelectedWeapon.Type == 3 && Data.Input.AttackType == "Headshot") || Data.SelectedEnemy.CanMeleeHeadshot == true)
            {
                return (float)Data.SelectedEnemy.HeadshotMultiplier;
            }
            return 1;
        }
        public float GetComboFinisher()
        {
            if (Data.Input.AttackType == "Combo Finisher")
            {
                return 2;
            }
            return 1;
        }
        public float GetThrow(float AttackUp)
        {
            string FuseBaseName = Data.SelectedWeapon.FuseBaseName;
            var SelectedWeaponType = Data.SelectedWeapon.Type;
            bool ProjectileProperty = ScanProperties("Melee Projectile");

            if (Data.Input.AttackType != "Throw" || SelectedWeaponType >= 3 ||
                FuseBaseName == "Master Sword" || FuseBaseName == "Decayed Master Sword"
                || ProjectileProperty)
            {
                return 1;
            }

            if (Data.SelectedWeapon.Property == "Boomerang")
            {
                return 1.5f * AttackUp;
            }

            return 2 * AttackUp;
        }
        public float GetFrozen()
        {
            var SelectedEnemy = Data.SelectedEnemy.Name;
            string AttackType = Data.Input.AttackType;

            if (Data.SelectedEnemy.CanFreeze == false || SelectedEnemy == "Gibdo" || SelectedEnemy == "Moth Gibdo")
            {
                return 1;
            }
            if (Data.Input.Frozen == true && AttackType != "Sneakstrike" && AttackType != "Flurry Rush" && AttackType != "Headshot")
            {
                return 3;
            }
            return 1;
        }
        public float GetTreeCutter()
        {
            bool CutProperty = ScanProperties("Cut");
            bool TreeCutterProperty = ScanProperties("Tree Cutter");

            if (Data.SelectedEnemy.Name == "Evermean")
            {
                if (!CutProperty && !TreeCutterProperty)
                {
                    return 0;
                }
                if (TreeCutterProperty)
                {
                    return 3;
                }
            }
            return 1;
        }
        public float GetArrowEnemyMult()
        {
            if (Data.SelectedWeapon.Type == 3)
            {
                return (float)Data.SelectedEnemy.ArrowMultiplier;
            }
            return 1;
        }
        public float GetElementalDamage()
        {
            bool FireProperty = ScanProperties("Fire") || ScanProperties("Fire Burst");
            bool IceProperty = ScanProperties("Ice") || ScanProperties("Ice Burst");
            bool ShockProperty = ScanProperties("Shock") || ScanProperties("Shock Burst");
            bool WaterProperty = ScanProperties("Water");
            bool BombProperty = ScanProperties("Bomb");
            float FireDamage = (float)Data.SelectedEnemy.FireDamage;
            float IceDamage = (float)Data.SelectedEnemy.IceDamage;
            float ShockDamage = (float)Data.SelectedEnemy.ShockDamage;
            bool HotWeatherAttack = GetHotWeatherAttack();
            bool ColdWeatherAttack = GetColdWeatherAttack();
            bool StormyWeatherAttack = GetStormyWeatherAttack();
            float HotWeatherPower = 0;
            float ColdWeatherPower = 0;
            float StormyWeatherPower = 0;
            bool UsingFire = HotWeatherAttack || FireProperty || BombProperty;
            bool UsingIce = ColdWeatherAttack || IceProperty;
            bool UsingShock = StormyWeatherAttack || ShockProperty;
            float ElementPower = Data.SelectedFuse?.ElementPower ?? 0.0f;

            if (HotWeatherAttack) { HotWeatherPower = 5; }
            if (ColdWeatherAttack) { ColdWeatherPower = 5; }
            if (StormyWeatherAttack) { StormyWeatherPower = 5; }

            if (UsingIce)
            {
                return ElementPower + IceDamage + ColdWeatherPower;
            }
            if (UsingFire)
            {
                ElementPower += FireDamage + HotWeatherPower;
            }
            if (BombProperty)
            {
                ElementPower *= (float)Data.SelectedEnemy.BombMultiplier;
            }
            if (UsingShock)
            {
                ElementPower += ShockDamage + StormyWeatherPower;
            }
            if (WaterProperty)
            {
                ElementPower += (float)Data.SelectedEnemy.WaterDamage;
            }
            if (Data.SelectedFuse.Name == "Beam Emitter")
            {
                ElementPower += (12 * (float)Data.SelectedEnemy.BeamMultiplier);
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
            bool FireProperty = ScanProperties("Fire") || ScanProperties("Fire Burst") || ScanProperties("Bomb");
            bool IceProperty = ScanProperties("Ice") || ScanProperties("Ice Burst");
            bool ShockProperty = ScanProperties("Shock") || ScanProperties("Shock Burst");
            bool WaterProperty = ScanProperties("Water");
            bool HotWeatherAttack = GetHotWeatherAttack();
            bool ColdWeatherAttack = GetColdWeatherAttack();
            bool StormyWeatherAttack = GetStormyWeatherAttack();
            string EnemyElement = Data.SelectedEnemy.Element;

            switch (EnemyElement)
            {
                case "Fire":
                    if (IceProperty || ColdWeatherAttack)
                    {
                        return 2;
                    }
                    if (WaterProperty)
                    {
                        return 1.5f;
                    }
                    if (FireProperty || HotWeatherAttack)
                    {
                        return 0;
                    }
                    return 1;
                case "Ice":
                    if (FireProperty || HotWeatherAttack)
                    {
                        return 2;
                    }
                    if (IceProperty || ColdWeatherAttack)
                    {
                        return 0;
                    }
                    return 1;
                case "Shock":
                    if (ShockProperty)
                    {
                        return 0;
                    }
                    return 1;
                default:
                    return 1;
            }
        }
        public float GetContinuousFire()
        {
            bool FireProperty = ScanProperties("Fire") || ScanProperties("Fire Burst");
            bool HotWeatherAttack = GetHotWeatherAttack();

            if ((FireProperty || HotWeatherAttack) && Data.SelectedEnemy.Element != "Fire")
            {
                return (float)Data.SelectedEnemy.FireDamageContinuous;
            }
            return 0;
        }
        public float GetMoldugaBelly()
        {
            if (Data.SelectedEnemy.Name == "Molduga (Belly)")
            {
                return 1.2f;
            }
            return 1;
        }
    }
}
