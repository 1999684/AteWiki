using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AtelierWiki.Data.A11
{
    public class A11DbManager
    {
        private static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
        private static string DbPath => Path.Combine(BaseDirectory, "Data", "A11", "A11.db");
        private static string ConnectionString => $"Data Source={DbPath};Version=3;";

        public static void Initialize()
        {
            string dir = Path.GetDirectoryName(DbPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (!File.Exists(DbPath)) SQLiteConnection.CreateFile(DbPath);

            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();

                // 1. 素材表
                conn.Execute(@"
                    CREATE TABLE IF NOT EXISTS Materials (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Level INTEGER,
                        Locations TEXT,
                        Monsters TEXT,
                        BgImagePath TEXT
                    );");

                // 2. 特性表
                conn.Execute(@"
                    CREATE TABLE IF NOT EXISTS Features (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Extension TEXT,
                        Cost INTEGER,
                        IsSynthesize INTEGER,
                        IsAttack INTEGER,
                        IsHeal INTEGER,
                        IsWeapon INTEGER,
                        IsArmor INTEGER,
                        IsAccessory INTEGER,
                        IsExtension INTEGER,
                        Description TEXT,
                        Note TEXT
                    );");

                // 3. 调合表 (更新结构)
                conn.Execute(@"
                    CREATE TABLE IF NOT EXISTS Harmonies (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Level INTEGER,
                        Mat1 TEXT,
                        Mat2 TEXT,
                        Mat3 TEXT,
                        Mat4 TEXT,
                        Category TEXT,
                        Acquisition TEXT,
                        BgImagePath TEXT
                    );");

                // 数据库迁移逻辑：检查 IsExtension
                var columns = conn.Query<string>("SELECT name FROM pragma_table_info('Features')").ToList();
                if (!columns.Contains("IsExtension"))
                {
                    conn.Execute("ALTER TABLE Features ADD COLUMN IsExtension INTEGER DEFAULT 0");
                }

                // 数据库迁移逻辑：检查 Harmonies 是否需要更新结构
                var hColumns = conn.Query<string>("SELECT name FROM pragma_table_info('Harmonies')").ToList();
                if (hColumns.Contains("Cat1")) // 旧结构标志
                {
                    conn.Execute("DROP TABLE Harmonies;");
                    conn.Execute(@"
                        CREATE TABLE Harmonies (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL,
                            Level INTEGER,
                            Mat1 TEXT,
                            Mat2 TEXT,
                            Mat3 TEXT,
                            Mat4 TEXT,
                            Category TEXT,
                            Acquisition TEXT,
                            BgImagePath TEXT
                        );");
                }

                // 初始化素材
                if (conn.ExecuteScalar<long>("SELECT COUNT(*) FROM Materials") == 0)
                {
                    conn.Execute("INSERT INTO Materials (Name, Level, Locations, Monsters, BgImagePath) VALUES (@Name, @Level, @Locations, @Monsters, @BgImagePath)",
                        new[] { new { Name = "模拟数据", Level = 1, Locations = "附近的森林", Monsters = "", BgImagePath = "" } });
                }

                // 初始化特性
                if (conn.ExecuteScalar<long>("SELECT COUNT(*) FROM Features") == 0)
                {
                    string sql = @"
                        INSERT INTO Features (Name, Extension, Cost, IsSynthesize, IsAttack, IsHeal, IsWeapon, IsArmor, IsAccessory, IsExtension, Description, Note)
                        VALUES (@Name, @Extension, @Cost, @IsSynthesize, @IsAttack, @IsHeal, @IsWeapon, @IsArmor, @IsAccessory, @IsExtension, @Description, @Note)";

                    conn.Execute(sql, new[] {
                        new { Name="高價Lv1", Extension="", Cost=2, IsSynthesize=true, IsAttack=true, IsHeal=true, IsWeapon=false, IsArmor=true, IsAccessory=true, IsExtension=false, Description="基本價格增加20%", Note="" },
                        new { Name="高價Lv2", Extension="", Cost=5, IsSynthesize=true, IsAttack=true, IsHeal=true, IsWeapon=false, IsArmor=true, IsAccessory=true, IsExtension=false, Description="基本價格增加50%", Note="" },
                        new { Name="高級品", Extension="", Cost=10, IsSynthesize=true, IsAttack=true, IsHeal=true, IsWeapon=false, IsArmor=true, IsAccessory=true, IsExtension=false, Description="基本價格增加100%", Note="高價Lv1ｘ高價Lv2" }
                    });
                }

                // 初始化调合数据
                if (conn.ExecuteScalar<long>("SELECT COUNT(*) FROM Harmonies") == 0)
                {
                    string sql = @"
                        INSERT INTO Harmonies (Name, Level, Mat1, Mat2, Mat3, Mat4, Category, Acquisition, BgImagePath)
                        VALUES (@Name, @Level, @Mat1, @Mat2, @Mat3, @Mat4, @Category, @Acquisition, @BgImagePath)";

                    conn.Execute(sql, new[] {
                        new { Name="中和劑·紅", Level=1, Mat1="（花）", Mat2="水", Mat3="", Mat4="", Category="调合品", Acquisition="初期持有", BgImagePath="" },
                        new { Name="中和劑·藍", Level=1, Mat1="（矿石）", Mat2="水", Mat3="", Mat4="", Category="调合品", Acquisition="初期持有", BgImagePath="" }
                    });
                }
            }
        }

        public static List<Material> GetAllMaterials()
        {
            using (var conn = new SQLiteConnection(ConnectionString))
                return conn.Query<Material>("SELECT * FROM Materials ORDER BY Level, Name").ToList();
        }

        public static List<Material> SearchMaterials(string keyword)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
                return conn.Query<Material>("SELECT * FROM Materials WHERE Name LIKE @Kw OR Locations LIKE @Kw", new { Kw = $"%{keyword}%" }).ToList();
        }

        public static void AddMaterial(Material m)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                string sql = @"INSERT INTO Materials (Name, Level, Locations, Monsters, BgImagePath) VALUES (@Name, @Level, @Locations, @Monsters, @BgImagePath); SELECT last_insert_rowid();";
                m.Id = conn.ExecuteScalar<int>(sql, m);
            }
        }

        public static void DeleteMaterial(int id)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
                conn.Execute("DELETE FROM Materials WHERE Id = @Id", new { Id = id });
        }

        public static void UpdateMaterial(Material m)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
                conn.Execute("UPDATE Materials SET Name=@Name, Level=@Level, Locations=@Locations, Monsters=@Monsters, BgImagePath=@BgImagePath WHERE Id=@Id", m);
        }

        // ==========================================
        // Harmony 方法
        // ==========================================
        public static List<Harmony> GetAllHarmonies()
        {
            using (var conn = new SQLiteConnection(ConnectionString))
                return conn.Query<Harmony>("SELECT * FROM Harmonies ORDER BY Level, Name").ToList();
        }

        public static List<Harmony> SearchHarmonies(string keyword)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
                return conn.Query<Harmony>(@"SELECT * FROM Harmonies
                                           WHERE Name LIKE @Kw OR Category LIKE @Kw OR Acquisition LIKE @Kw
                                           OR Mat1 LIKE @Kw OR Mat2 LIKE @Kw OR Mat3 LIKE @Kw OR Mat4 LIKE @Kw",
                                           new { Kw = $"%{keyword}%" }).ToList();
        }

        public static void AddHarmony(Harmony h)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                string sql = @"INSERT INTO Harmonies (Name, Level, Mat1, Mat2, Mat3, Mat4, Category, Acquisition, BgImagePath)
                               VALUES (@Name, @Level, @Mat1, @Mat2, @Mat3, @Mat4, @Category, @Acquisition, @BgImagePath);
                               SELECT last_insert_rowid();";
                h.Id = conn.ExecuteScalar<int>(sql, h);
            }
        }

        public static void DeleteHarmony(int id)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
                conn.Execute("DELETE FROM Harmonies WHERE Id = @Id", new { Id = id });
        }

        public static void UpdateHarmony(Harmony h)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
                conn.Execute(@"UPDATE Harmonies SET Name=@Name, Level=@Level, Mat1=@Mat1, Mat2=@Mat2,
                               Mat3=@Mat3, Mat4=@Mat4, Category=@Category, Acquisition=@Acquisition, BgImagePath=@BgImagePath WHERE Id=@Id", h);
        }

        // ==========================================
        // Feature 方法 (特性相关)
        // ==========================================

        public static async Task<List<Feature>> GetAllFeaturesAsync()
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                var result = await conn.QueryAsync<Feature>("SELECT * FROM Features ORDER BY Cost, Name");
                return result.ToList();
            }
        }

        public static async Task<List<Feature>> SearchFeaturesAsync(string keyword)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                var result = await conn.QueryAsync<Feature>(
                    "SELECT * FROM Features WHERE Name LIKE @Kw OR Description LIKE @Kw",
                    new { Kw = $"%{keyword}%" });
                return result.ToList();
            }
        }

        public static void AddFeature(Feature item)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                string sql = @"
                    INSERT INTO Features (Name, Extension, Cost, IsSynthesize, IsAttack, IsHeal, IsWeapon, IsArmor, IsAccessory, IsExtension, Description, Note)
                    VALUES (@Name, @Extension, @Cost, @IsSynthesize, @IsAttack, @IsHeal, @IsWeapon, @IsArmor, @IsAccessory, @IsExtension, @Description, @Note);
                    SELECT last_insert_rowid();";
                item.Id = conn.ExecuteScalar<int>(sql, item);
            }
        }

        public static void UpdateFeature(Feature item)
        {
            if (item == null) return;
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                string sql = @"
                    UPDATE Features SET Name=@Name, Extension=@Extension, Cost=@Cost, IsSynthesize=@IsSynthesize, IsAttack=@IsAttack,
                    IsHeal=@IsHeal, IsWeapon=@IsWeapon, IsArmor=@IsArmor, IsAccessory=@IsAccessory, IsExtension=@IsExtension,
                    Description=@Description, Note=@Note WHERE Id=@Id";
                conn.Execute(sql, item);
            }
        }

        public static void DeleteFeature(int id)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
                conn.Execute("DELETE FROM Features WHERE Id = @Id", new { Id = id });
        }
    }
}
