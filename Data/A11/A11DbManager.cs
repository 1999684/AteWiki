using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

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

                conn.Execute(@"
                    CREATE TABLE IF NOT EXISTS Materials (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Level INTEGER,
                        Locations TEXT,
                        Monsters TEXT,
                        BgImagePath TEXT
                    );");

                long count = conn.ExecuteScalar<long>("SELECT COUNT(*) FROM Materials");
                if (count == 0)
                {
                    string sql = "INSERT INTO Materials (Name, Level, Locations, Monsters, BgImagePath) VALUES (@Name, @Level, @Locations, @Monsters, @BgImagePath)";

                    conn.Execute(sql, new[] {
                        new { Name = "模拟数据", Level = 1, Locations = "附近的森林、機械領域．變異", Monsters = "", BgImagePath = "/Assets/UserImages/mock.png" },
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
                string sql = @"
                    INSERT INTO Materials (Name, Level, Locations, Monsters, BgImagePath) 
                    VALUES (@Name, @Level, @Locations, @Monsters, @BgImagePath)";

                conn.Execute(sql, m);
            }
        }

        public static void DeleteMaterial(int id)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
                conn.Execute("DELETE FROM Materials WHERE Id = @Id", new { Id = id });
        }
    }
}
