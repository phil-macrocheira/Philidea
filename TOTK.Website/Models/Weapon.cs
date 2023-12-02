using System.Text.Json;
using System.Text.Json.Serialization;

namespace TOTK.Website.Models
{
    public class Weapon
    {
        public string Name { get; set; }
        public string Type { get; set; }

        [JsonPropertyName("Base Attack Power")]
        public int BaseAttack { get; set; }

        [JsonPropertyName("Base Attack Power (UI)")]
        public string BaseAttackUI { get; set; }

        public int Durability { get; set; }
        public string Image { get; set; }
        public override string ToString() => JsonSerializer.Serialize<Weapon>(this);
    }
}
