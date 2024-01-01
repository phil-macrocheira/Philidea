using TOTK.Website.Pages;

namespace TOTK.Website
{
    public class GetFusedName
    {
        private readonly ILogger<GetFusedName> _logger;
        public totk_calculatorModel Data;
        public GetFusedName(ILogger<GetFusedName> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public string GetName(totk_calculatorModel data)
        {
            Data = data;

            string Adjective = Data.SelectedFuse.Adjective + " ";
            string BaseName = Data.SelectedWeapon.FuseBaseName;
            string WeaponNamingRule = Data.SelectedWeapon.NamingRule;
            string FuseNamingRule = Data.SelectedFuse.NamingRule;
            string WeaponName = Data.SelectedWeapon.Name;
            string FuseName = Data.SelectedFuse.Name;
            bool FuseIsWeapon = Data.SelectedFuse.WeaponDurability > 0;
            bool ReplaceProperties = (bool)Data.SelectedFuse.ReplaceProperties;
            string WeaponProperty = Data.SelectedWeapon.Property;
            bool IsBoomerang = WeaponProperty == "Boomerang";
            byte WeaponType = (byte)Data.SelectedWeapon.Type;
            bool MeleeProjectileProperty = ScanProperties("Melee Projectile");
            bool ShatterProperty = ScanProperties("Shatter Rock");
            bool TreeCutterProperty = ScanProperties("Tree Cutter");
            string BindType;
            bool WeaponCanCut = (bool)Data.SelectedWeapon.CanCut;
            bool FuseCanCut = (bool)Data.SelectedFuse.CanCut;
            bool CanCut = ScanProperties("Cut");
            bool IsZonaiWeapon = false;
            string ElementAdjective = "Flame ";
            bool ReuseSeedElement = false;

            if (WeaponType == 2)
                BindType = Data.SelectedFuse.BindTypeSpear;
            else
                BindType = Data.SelectedFuse.BindTypeSword;

            if (WeaponProperty == "Zonai Lv1" || WeaponProperty == "Zonai Lv2" || WeaponProperty == "Zonai Lv3")
                IsZonaiWeapon = true;

            if (BaseName == "Master Sword") {
                if (WeaponName == "Master Sword (Prologue)")
                    return "MsgNotFound";
                else
                    return "Master Sword";
            }
            if (BaseName == "Decayed Master Sword")
                return BaseName;

            if (WeaponType == 3)
                return WeaponName + " (" + FuseName + " Arrow)";

            if (WeaponType == 4)
                return Adjective + BaseName;

            if (WeaponType == 5)
                return WeaponName;

            if (FuseIsWeapon)
                return Adjective + BaseName;

            if (FuseNamingRule == "Whip")
                return Adjective + "Tail Whip";

            if (FuseName == "Courser Bee Honey")
                return Adjective + BaseName;

            if (MeleeProjectileProperty) {
                if (WeaponProperty == "Bone" || IsBoomerang || IsZonaiWeapon)
                    return Adjective + "Rod";
                else
                    return Adjective + BaseName;
            }

            if (FuseNamingRule == "UnlikeHammer") {
                switch (WeaponType) {
                    case 0:
                        return Adjective + "Pounder"; // 1H
                    case 1:
                        return Adjective + "Smasher"; // 2H
                    case 2:
                        return Adjective + "Pulverizer"; // Spear
                    default:
                        break;
                }
            }

            if (ShatterProperty) {
                if (WeaponName == "Gloom Club") { // Removed code about BindType == Attach?
                    return Adjective + BaseName;
                }
                if (IsBoomerang)
                    return Adjective + "Boomerang";
                if (WeaponType == 2)
                    return Adjective + "Sledge"; // Spear
                if (WeaponType < 2)
                    return Adjective + "Hammer"; // 1H or 2H
            }

            if (WeaponNamingRule == "ImpressiveGrip") {
                if (WeaponType == 2 && !WeaponCanCut)
                    return Adjective + BaseName;
            }

            if ((WeaponNamingRule == "ImpressiveGrip" || WeaponNamingRule == "UnlikeBat") && !CanCut)
                return Adjective + BaseName;

            // Incorrect rule?
            //if (WeaponType == 2 && !FuseCanCut)
            //    return Adjective + "Spear";

            if (FuseNamingRule == "Fan") {
                if (WeaponType == 2)
                    return Adjective + "Spear";
                else
                    return Adjective + "Guster";
            }

            if (FuseNamingRule == "PowHammer") {
                if (IsZonaiWeapon) {
                    switch (WeaponType) {
                        case 0:
                            return "Bouncy Club"; // 1H
                        case 1:
                            return "Bouncy Bat"; // 2H
                        case 2:
                            return "Bouncy Spear"; // Spear
                        default:
                            break;
                    }
                }
                return "Bouncy " + BaseName;
            }

            if (FuseNamingRule == "ReuseSeedFireBurst") {
                ElementAdjective = "Flame ";
                ReuseSeedElement = true;
            }
            if (FuseNamingRule == "ReuseSeedIceBurst") {
                ElementAdjective = "Freezing ";
                ReuseSeedElement = true;
            }
            if (FuseNamingRule == "ReuseSeedShockBurst") {
                ElementAdjective = "Electric ";
                ReuseSeedElement = true;
            }

            if (ReuseSeedElement) {
                if (IsZonaiWeapon) {
                    switch (WeaponType) {
                        case 0:
                            return ElementAdjective + "Club"; // 1H
                        case 1:
                            return ElementAdjective + "Bat"; // 2H
                        case 2:
                            return ElementAdjective + "Spear"; // Spear
                        default:
                            break;
                    }
                }
                return ElementAdjective + BaseName;
            }

            if (FuseNamingRule == "LongThrow" || FuseNamingRule == "SpWing") {
                if (IsZonaiWeapon) {
                    switch (WeaponType) {
                        case 0:
                            return "Soaring Club"; // 1H
                        case 1:
                            return "Soaring Bat"; // 2H
                        case 2:
                            return "Soaring Spear"; // Spear
                        default:
                            break;
                    }
                }
                return "Soaring " + BaseName;
            }

            if (IsBoomerang)
                return Adjective + BaseName;

            if (FuseNamingRule == "Torch")
                return Adjective + "Torch";

            if (TreeCutterProperty) {
                switch (WeaponType) {
                    case 0:
                        return Adjective + "Axe"; // 1H
                    case 1:
                        return Adjective + "Two-Handed Axe"; // 2H
                    case 2:
                        return Adjective + "Halberd"; // Spear
                    default:
                        break;
                }
            }

            if (CanCut && ReplaceProperties) {
                switch (WeaponType) {
                    case 0:
                        return Adjective + "Reaper"; // 1H
                    case 1:
                        return Adjective + "Blade"; // 2H
                    case 2:
                        return Adjective + "Spear"; // Spear
                    default:
                        break;
                }
            }

            if ((!CanCut && WeaponType != 2) || ReplaceProperties || IsZonaiWeapon) {
                switch (WeaponType) {
                    case 0:
                        return Adjective + "Club"; // 1H
                    case 1:
                        return Adjective + "Bat"; // 2H
                    case 2:
                        return Adjective + "Spear"; // Spear
                    default:
                        break;
                }
            }

            // Pointless check?
            //if (CanCut && !ReplaceProperties && BindType == "Replace")
            //    return Adjective + BaseName;

            return Adjective + BaseName;
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
    }
}
