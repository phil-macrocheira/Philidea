namespace TOTK.Website.Models
{
    public class InputModel
    {
        public int AttackUpMod { get; set; } = 0;
        public string? AttackType { get; set; } = "Standard Attack";
        public bool CriticalHitMod { get; set; } = false;
        public bool Multishot { get; set; } = false;
        public bool Zonaite { get; set; } = false;
        public bool SageWill { get; set; } = false;
        public int Durability { get; set; } = 40;
        public bool FreezeDurability { get; set; } = false;
        public bool Wet { get; set; } = false;
        public bool Headshot { get; set; } = false;
        public bool Frozen { get; set; } = false;
        public bool Weakened { get; set; } = false;
        public bool Fence { get; set; } = false;
        public float HP { get; set; } = 38;
        public string? Buff1 { get; set; } = "None";
        public string? Buff2 { get; set; } = "None";
        public bool FreeMode { get; set; }
        public bool TrueDamage { get; set; }
    }
}