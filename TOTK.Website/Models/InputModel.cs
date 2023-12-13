namespace TOTK.Website.Models
{
    public class InputModel
    {
        public int AttackUpMod { get; set; }
        public string? AttackType {  get; set; }
        public bool LowDurability {  get; set; }
        public bool Multishot {  get; set; }
        public bool Wet {  get; set; }
        public string? EnemyCondition { get; set; }
        public float HP {  get; set; }
        public string? Buff1 {  get; set; }
        public string? Buff2 { get; set; }
    }
}