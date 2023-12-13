using System.Data.SqlClient;

namespace TOTK.Website.Models
{
    public class Weapon
    {
        public string? ActorID { get; set; }
        public string? Name { get; set; }
        public byte? Type { get; set; }
        public bool? CanCut { get; set; }
        public byte? BaseAttack { get; set; }
        public byte? Durability { get; set; }
        public string? Property { get; set; }
        public bool? CanFuseTo { get; set; }
        public byte? FuseExtraDurability { get; set; }
        public string? FuseBaseName { get; set; }
        public string? IconURL { get; set; }
    }
    public class Fuse
    {
        public string? ActorID { get; set; }
        public string? Name { get; set; }
        public byte? BaseAttack { get; set; }
        public bool? CanCut { get; set; }
        public bool? AddsShieldAttack { get; set; }
        public bool? ReplaceProperties { get; set; }
        public string? Property1 { get; set; }
        public string? Property2 { get; set; }
        public string? Property3 { get; set; }
        public string? Property4 { get; set; }
        public string? IconURL { get; set; }
    }
    public class Enemy
    {
        public string? ActorID { get; set; }
        public string? Name { get; set; }
        public bool? HasGloomVariant { get; set; }
        public short? HP { get; set; }
        public short? FireDamage { get; set; }
        public short? FireDamageContinuous { get; set; }
        public short? IceDamage { get; set; }
        public short? ShockDamage { get; set; }
        public byte? WaterDamage { get; set; }
        public bool? WeakToWater { get; set; }
        public short? RijuDamage { get; set; }
        public bool? AncientBladeDefeat { get; set; }
        public bool? IsRock { get; set; }
        public bool? CanSneakstrike { get; set; }
        public float? ArrowMultiplier { get; set; }
        public float? BeamMultiplier { get; set; }
        public float? BombMultiplier { get; set; }
        public string? IconURL { get; set; }
    }

    public class ImportData
    {
        private string connectionString = "Data Source=(localdb)\\mssqllocaldb;Integrated Security=True;Encrypt=False";
        public List<Weapon> LoadWeapons()
        {
            List<Weapon> weapons = new List<Weapon>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM weapons ORDER BY SortOrder";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Weapon weapon = new Weapon
                            {
                                ActorID = reader["ActorID"].ToString(),
                                Name = reader["Name"].ToString(),
                                Type = (byte?)reader["Type"],
                                CanCut = (bool?)reader["CanCut"],
                                BaseAttack = (byte?)reader["BaseAttack"],
                                Durability = (byte?)reader["Durability"],
                                Property = reader["Property"].ToString(),
                                CanFuseTo = (bool?)reader["CanFuseTo"],
                                FuseExtraDurability = (byte?)reader["FuseExtraDurability"],
                                FuseBaseName = reader["FuseBaseName"].ToString(),
                                IconURL = reader["IconURL"].ToString()
                            };

                            weapons.Add(weapon);
                        }
                    }
                }
            }
            return weapons;
        }
        public List<Fuse> LoadFuse()
        {
            List<Fuse> fuses = new List<Fuse>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM fuse ORDER BY SortOrder";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Fuse fuse = new Fuse
                            {
                                ActorID = reader["ActorID"].ToString(),
                                Name = reader["Name"].ToString(),
                                BaseAttack = (byte?)reader["BaseAttack"],
                                CanCut = (bool?)reader["CanCut"],
                                AddsShieldAttack = (bool?)reader["AddsShieldAttack"],
                                ReplaceProperties = (bool?)reader["ReplaceProperties"],
                                Property1 = reader["Property1"].ToString(),
                                Property2 = reader["Property2"].ToString(),
                                Property3 = reader["Property3"].ToString(),
                                Property4 = reader["Property4"].ToString(),
                                IconURL = reader["IconURL"].ToString()
                            };

                            fuses.Add(fuse);
                        }
                    }
                }
            }
            return fuses;
        }
        public List<Enemy> LoadEnemies()
        {
            List<Enemy> enemies = new List<Enemy>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM enemies ORDER BY SortOrder";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Enemy enemy = new Enemy
                            {
                                ActorID = reader["ActorID"].ToString(),
                                Name = reader["Name"].ToString(),
                                HasGloomVariant = (bool?)reader["HasGloomVariant"],
                                HP = (short?)reader["HP"],
                                FireDamage = (short?)reader["FireDamage"],
                                FireDamageContinuous = (short?)reader["FireDamageContinuous"],
                                IceDamage = (short?)reader["IceDamage"],
                                ShockDamage = (short?)reader["ShockDamage"],
                                WaterDamage = (byte?)reader["WaterDamage"],
                                RijuDamage = (short?)reader["RijuDamage"],
                                AncientBladeDefeat = (bool?)reader["AncientBladeDefeat"],
                                IsRock = (bool?)reader["IsRock"],
                                CanSneakstrike = (bool?)reader["CanSneakstrike"],
                                ArrowMultiplier = (float?)reader["ArrowMultiplier"],
                                BeamMultiplier = (float?)reader["BeamMultiplier"],
                                BombMultiplier = (float?)reader["BombMultiplier"],
                                IconURL = reader["IconURL"].ToString()
                            };

                            enemies.Add(enemy);
                        }
                    }
                }
            }
            return enemies;
        }
    }
}