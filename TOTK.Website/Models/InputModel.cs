namespace TOTK.Website.Models
{
    public class InputModel
    {
        public int AttackUpMod { get; set; } = 0;
        public string? AttackType { get; set; } = "Standard Attack";
        public int Durability { get; set; } = 40;
        public bool FreezeDurability { get; set; } = false;
        public bool Wet { get; set; } = false;
        public bool Headshot { get; set; } = false;
        public bool Frozen { get; set; } = false;
        public float HP { get; set; } = 38;
        public string? Buff1 { get; set; } = "None";
        public string? Buff2 { get; set; } = "None";
    }
}