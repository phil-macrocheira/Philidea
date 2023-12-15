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
        public IndexModel Data;
        public CalculateDamage(ILogger<CalculateDamage> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public float Calculate(IndexModel data)
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
            float Bone = (float)GetBone();
            float FlurryRush = (float)GetFlurryRush();
            float Shatter = (float)GetShatter();

            float AttackUp = (float)GetAttackUp();
            float Headshot = (float)GetHeadshot();
            float Throw = (float)GetThrow();
            float Frozen = (float)GetFrozen();
            float TreeCutter = (float)GetTreeCutter();

            float ElementalDamage = (float)GetElementalDamage();
            float ElementalMult = (float)GetElementalMult();
            float ContinuousFire = (float)GetContinuousFire();

            float ArrowEnemyMult = (float)GetArrowEnemyMult();
            //float BeamEnemyMult = (float)GetBeamEnemyMult();
            // implement rod projectiles, rods do x2
            // cannons do 40 as bomb arrows

            // Return enemy's HP if ancient blade
            if (Data.SelectedFuse.Name == "Ancient Blade" && Data.SelectedEnemy.AncientBladeDefeat == true)
            {
                return (float)Data.SelectedEnemy.HP;
            }

            DamageOutput += BaseAttack + FuseUIAdjust((FuseBaseAttack * GerudoBonus) + AttackUpMod + ZonaiBonus);
            DamageOutput *= LowHealth * WetPlayer * Sneakstrike * LowDurability * Bone * FlurryRush * Shatter;
            DamageOutput *= AttackUp * Headshot * (Throw * AttackUp) * Frozen * TreeCutter; // Also multiply combo finisher later
            DamageOutput = (float)Math.Floor(DamageOutput);
            DamageOutput += ElementalDamage;
            DamageOutput *= ElementalMult;
            DamageOutput += ContinuousFire;

            _logger.LogInformation($"BaseAttack: {BaseAttack}, AttackUpMod: {AttackUpMod}, FuseBaseAttack: {FuseBaseAttack}, " +
                                   $"GerudoBonus: {GerudoBonus}, ZonaiBonus: {ZonaiBonus}, LowHealth: {LowHealth}, " +
                                   $"WetPlayer: {WetPlayer}, Sneakstrike: {Sneakstrike}, LowDurability: {LowDurability}, " +
                                   $"Bone: {Bone}, FlurryRush: {FlurryRush}, Shatter: {Shatter}, AttackUp: {AttackUp}, " +
                                   $"Headshot: {Headshot}, Throw: {Throw}, Frozen: {Frozen}, TreeCutter: {TreeCutter}, " +
                                   $"ElementalDamage: {ElementalDamage}, ElementalMult: {ElementalMult}, ContinuousFire: {ContinuousFire}");

            return (float)Math.Floor(DamageOutput);
        }
        public float GetBaseAttack()
        {
            string SelectedWeapon = Data.SelectedWeapon.Name;
            float MasterSwordBeamUp = 1.0f;

            if (SelectedWeapon == "Demon King's Bow")
            {
                return (float)Math.Max(1, Math.Min(60, Math.Floor(Data.Input.HP) * 2));
            }

            if (Data.Input.AttackType == "Throw" && (SelectedWeapon == "Master Sword" || SelectedWeapon == "Master Sword (Prologue)" ||
                SelectedWeapon == "Master Sword (Awakened +15)" || SelectedWeapon == "Master Sword (Awakened +30)"))
            {
                if (Data.Input.Buff1 == "Master Sword Beam Up" || Data.Input.Buff2 == "Master Sword Beam Up")
                {
                    MasterSwordBeamUp = 1.5f;
                }
                return (float)Data.SelectedWeapon.ProjectileAttack * MasterSwordBeamUp;
            }

            return (float)Data.SelectedWeapon.BaseAttack;
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

            if (LowDurabilityProperty == true && Data.Input.LowDurability == true)
            {
                return 2;
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
            if (Data.SelectedWeapon.Type == 3 && Data.Input.AttackType == "Headshot")
            {
                return 2;
            }
            return 1;
        }
        public float GetThrow()
        {
            string SelectedWeapon = Data.SelectedWeapon.Name;
            var SelectedWeaponType = Data.SelectedWeapon.Type;

            if (Data.Input.AttackType != "Throw" || SelectedWeaponType >= 3 ||
                SelectedWeapon == "Decayed Master Sword" || SelectedWeapon == "Master Sword (Prologue)")
            {
                return 1;
            }

            if (Data.SelectedWeapon.Property == "Boomerang")
            {
                return 1.5f;
            }

            return 2;
        }
        public float GetFrozen()
        {
            var SelectedEnemy = Data.SelectedEnemy.Name;

            if (Data.SelectedEnemy.CanFreeze == false || SelectedEnemy == "Gibdo" || SelectedEnemy == "Moth Gibdo")
            {
                return 1;
            }
            if (Data.Input.Frozen == true)
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
            float ElementPower = Data.SelectedFuse?.ElementPower ?? 0.0f;

            if (FireProperty)
            {
                return ElementPower + FireDamage;
            }
            if (IceProperty)
            {
                return ElementPower + (float)Data.SelectedEnemy.IceDamage;
            }
            if (ShockProperty)
            {
                return ElementPower + (float)Data.SelectedEnemy.ShockDamage;
            }
            if (WaterProperty)
            {
                return ElementPower + (float)Data.SelectedEnemy.WaterDamage;
            }
            if (BombProperty)
            {
                return (ElementPower + FireDamage) * (float)Data.SelectedEnemy.BombMultiplier;
            }
            return 0;
        }
        public float GetElementalMult()
        {
            bool FireProperty = ScanProperties("Fire") || ScanProperties("Fire Burst") || ScanProperties("Bomb");
            bool IceProperty = ScanProperties("Ice") || ScanProperties("Ice Burst");
            bool WaterProperty = ScanProperties("Water");
            string EnemyElement = Data.SelectedEnemy.Element;

            switch (EnemyElement)
            {
                case "Fire":
                    if (IceProperty)
                    {
                        return 2;
                    }
                    if (WaterProperty)
                    {
                        return 1.5f;
                    }
                    return 1;
                case "Ice":
                    if (FireProperty)
                    {
                        return 2;
                    }
                    return 1;
                default:
                    return 1;
            }
        }
        public float GetContinuousFire()
        {
            bool FireProperty = ScanProperties("Fire") || ScanProperties("Fire Burst");

            if (FireProperty)
            {
                return (float)Data.SelectedEnemy.FireDamageContinuous;
            }
            return 0;
        }
    }
}
