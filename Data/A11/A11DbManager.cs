using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks; // 必须引用：用于异步操作

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

                // 数据库迁移逻辑：检查 IsExtension
                var columns = conn.Query<string>("SELECT name FROM pragma_table_info('Features')").ToList();
                if (!columns.Contains("IsExtension"))
                {
                    conn.Execute("ALTER TABLE Features ADD COLUMN IsExtension INTEGER DEFAULT 0");
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
            }
        }

        // ==========================================
        // Material 方法
        // ==========================================
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
        // Feature 方法 (特性相关) - 优化为异步
        // ==========================================

        // 异步获取所有特性
        public static async Task<List<Feature>> GetAllFeaturesAsync()
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                // 使用 QueryAsync 避免阻塞 UI 线程
                var result = await conn.QueryAsync<Feature>("SELECT * FROM Features ORDER BY Cost, Name");
                return result.ToList();
            }
        }

        // 异步搜索特性
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
