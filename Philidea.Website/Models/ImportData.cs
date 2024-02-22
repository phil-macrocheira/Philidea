using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Philidea.Website.Models
{
    public class Weapon
    {
        public string? ActorID { get; set; }
        public string? Name { get; set; }
        public byte? Type { get; set; }
        public bool? CanCut { get; set; }
        public byte? BaseAttack { get; set; }
        public byte? ProjectileAttack { get; set; }
        public short? Durability { get; set; }
        public string? Property { get; set; }
        public bool? CanHaveAttackUpMod { get; set; }
        public byte? FuseExtraDurability { get; set; }
        public string? FuseBaseName { get; set; }
        public string? NamingRule { get; set; }
        public string? IconURL { get; set; }
    }
    public class Fuse
    {
        public string? ActorID { get; set; }
        public string? Name { get; set; }
        public byte? BaseAttack { get; set; }
        public byte? ProjectileAttack { get; set; }
        public byte? ElementPower { get; set; }
        public short? WeaponDurability { get; set; }
        public byte? MineruDurability { get; set; }
        public bool? CanFuseToArrow { get; set; }
        public byte? ArrowMultiplier { get; set; }
        public bool? CanCut { get; set; }
        public bool? AddsShieldAttack { get; set; }
        public bool? ReplaceProperties { get; set; }
        public string? Property1 { get; set; }
        public string? Property2 { get; set; }
        public string? Property3 { get; set; }
        public string? NamingRule { get; set; }
        public string? Adjective { get; set; }
        public string? BindTypeSword { get; set; }
        public string? BindTypeSpear { get; set; }
        public string? IconURL { get; set; }
    }
    public class Enemy
    {
        public string? ActorID { get; set; }
        public string? Name { get; set; }
        public short? HP { get; set; }
        public string? Element { get; set; }
        public short? FireDamage { get; set; }
        public short? FireDamageContinuous { get; set; }
        public bool? CanFreeze { get; set; }
        public short? IceDamage { get; set; }
        public short? ShockDamage { get; set; }
        public byte? WaterDamage { get; set; }
        public short? RijuDamage { get; set; }
        public bool? AncientBladeDefeat { get; set; }
        public bool? IsRock { get; set; }
        public bool? CanSneakstrike { get; set; }
        public bool? CanMeleeHeadshot { get; set; }
        public float? HeadshotMultiplier { get; set; }
        public float? ArrowMultiplier { get; set; }
        public float? BeamMultiplier { get; set; }
        public float? BombMultiplier { get; set; }
        public string? IconURL { get; set; }
    }
    public class ImportData
    {
        private string connectionString;
        public ImportData()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfiguration configuration = builder.Build();
            connectionString = configuration.GetConnectionString("connectionString");
        }
        public string GetLocalConnectionStringIfOffline()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            try {
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString)) {
                    connection.Open();
                    return connectionString;
                }
            }
            catch (Exception) {
                return "Data Source=(localdb)\\mssqllocaldb;Integrated Security=True;Encrypt=False";
            }
        }
        public List<Weapon> LoadWeapons()
        {
            List<Weapon> weapons = new List<Weapon>();

            //connectionString = GetLocalConnectionStringIfOffline();

            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();

                string query = "SELECT * FROM weapons ORDER BY SortOrder";
                using (SqlCommand command = new SqlCommand(query, connection)) {
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            Weapon weapon = new Weapon {
                                ActorID = reader["ActorID"].ToString(),
                                Name = reader["Name"].ToString(),
                                Type = (byte?)reader["Type"],
                                CanCut = (bool?)reader["CanCut"],
                                BaseAttack = (byte?)reader["BaseAttack"],
                                ProjectileAttack = (byte?)reader["ProjectileAttack"],
                                Durability = (short?)reader["Durability"],
                                Property = reader["Property"].ToString(),
                                CanHaveAttackUpMod = (bool?)reader["CanHaveAttackUpMod"],
                                FuseExtraDurability = (byte?)reader["FuseExtraDurability"],
                                FuseBaseName = reader["FuseBaseName"].ToString(),
                                NamingRule = reader["NamingRule"].ToString(),
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

            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();

                string query = "SELECT * FROM fuse ORDER BY SortOrder";
                using (SqlCommand command = new SqlCommand(query, connection)) {
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            Fuse fuse = new Fuse {
                                ActorID = reader["ActorID"].ToString(),
                                Name = reader["Name"].ToString(),
                                BaseAttack = (byte?)reader["BaseAttack"],
                                ProjectileAttack = (byte?)reader["ProjectileAttack"],
                                ElementPower = (byte?)reader["ElementPower"],
                                WeaponDurability = (short?)reader["WeaponDurability"],
                                MineruDurability = (byte?)reader["MineruDurability"],
                                CanFuseToArrow = (bool?)reader["CanFuseToArrow"],
                                ArrowMultiplier = (byte?)reader["ArrowMultiplier"],
                                CanCut = (bool?)reader["CanCut"],
                                AddsShieldAttack = (bool?)reader["AddsShieldAttack"],
                                ReplaceProperties = (bool?)reader["ReplaceProperties"],
                                Property1 = reader["Property1"].ToString(),
                                Property2 = reader["Property2"].ToString(),
                                Property3 = reader["Property3"].ToString(),
                                NamingRule = reader["NamingRule"].ToString(),
                                Adjective = reader["Adjective"].ToString(),
                                BindTypeSword = reader["BindTypeSword"].ToString(),
                                BindTypeSpear = reader["BindTypeSpear"].ToString(),
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

            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();

                string query = "SELECT * FROM enemies ORDER BY SortOrder";
                using (SqlCommand command = new SqlCommand(query, connection)) {
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            Enemy enemy = new Enemy {
                                ActorID = reader["ActorID"].ToString(),
                                Name = reader["Name"].ToString(),
                                HP = (short?)reader["HP"],
                                Element = reader["Element"].ToString(),
                                FireDamage = (short?)reader["FireDamage"],
                                FireDamageContinuous = (short?)reader["FireDamageContinuous"],
                                CanFreeze = (bool?)reader["CanFreeze"],
                                IceDamage = (short?)reader["IceDamage"],
                                ShockDamage = (short?)reader["ShockDamage"],
                                WaterDamage = (byte?)reader["WaterDamage"],
                                RijuDamage = (short?)reader["RijuDamage"],
                                AncientBladeDefeat = (bool?)reader["AncientBladeDefeat"],
                                IsRock = (bool?)reader["IsRock"],
                                CanSneakstrike = (bool?)reader["CanSneakstrike"],
                                CanMeleeHeadshot = (bool?)reader["CanMeleeHeadshot"],
                                HeadshotMultiplier = (float?)reader["HeadshotMultiplier"],
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
        public List<Fuse> LoadFuseArrow()
        {
            List<Fuse> fuses = new List<Fuse>();

            using (SqlConnection connection = new SqlConnection(connectionString)) {
                connection.Open();

                string query = "SELECT * FROM fuse WHERE CanFuseToArrow=1 ORDER BY SortOrder";
                using (SqlCommand command = new SqlCommand(query, connection)) {
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            Fuse fuse = new Fuse {
                                ActorID = reader["ActorID"].ToString(),
                                Name = reader["Name"].ToString(),
                                BaseAttack = (byte?)reader["BaseAttack"],
                                ProjectileAttack = (byte?)reader["ProjectileAttack"],
                                ElementPower = (byte?)reader["ElementPower"],
                                WeaponDurability = (short?)reader["WeaponDurability"],
                                MineruDurability = (byte?)reader["MineruDurability"],
                                CanFuseToArrow = (bool?)reader["CanFuseToArrow"],
                                ArrowMultiplier = (byte?)reader["ArrowMultiplier"],
                                CanCut = (bool?)reader["CanCut"],
                                AddsShieldAttack = (bool?)reader["AddsShieldAttack"],
                                ReplaceProperties = (bool?)reader["ReplaceProperties"],
                                Property1 = reader["Property1"].ToString(),
                                Property2 = reader["Property2"].ToString(),
                                Property3 = reader["Property3"].ToString(),
                                NamingRule = reader["NamingRule"].ToString(),
                                Adjective = reader["Adjective"].ToString(),
                                BindTypeSword = reader["BindTypeSword"].ToString(),
                                BindTypeSpear = reader["BindTypeSpear"].ToString(),
                                IconURL = reader["IconURL"].ToString()
                            };

                            fuses.Add(fuse);
                        }
                    }
                }
            }
            return fuses;
        }
    }
}