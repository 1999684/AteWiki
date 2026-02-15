using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace AtelierWiki.Data
{
    public class DbManager
    {
        // ==========================================
        // 1. 基础配置区域
        // ==========================================

        private static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
        private static string DbPath => System.IO.Path.Combine(BaseDirectory, "AtelierWiki.db");
        private static string ConnectionString => $"Data Source={DbPath};Version=3;";

        // ==========================================
        // 2. 初始化与升级逻辑 (核心)
        // ==========================================

        public static void Initialize()
        {
            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
            }

            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();

                // 2.1 确保基础表结构存在
                EnsureSchema(conn);

                // 2.2 获取当前数据库的内部版本号 (Int32)
                int currentDbVersion = conn.ExecuteScalar<int>("PRAGMA user_version");

                // 打印当前版本用于调试
                string verStr = IntToSemVer(currentDbVersion);
                System.Diagnostics.Debug.WriteLine($"[DB初始化] 当前数据库版本: {verStr} (Int: {currentDbVersion})");

                // ==========================================
                // 3. 版本迁移 (Migration) 区域
                // ==========================================

                // --- 目标版本: 1.0.1 ---
                int v1_0_1 = GetVersionInt(1, 0, 1);

                if (currentDbVersion < v1_0_1)
                {
                    InsertVersion101Data(conn);
                    UpdateVersion(conn, 1, 0, 1);
                    currentDbVersion = v1_0_1; // 更新内存变量，防止后续逻辑误判
                }
            }
        }

        // ==========================================
        // 4. 核心辅助方法：版本号算法
        // ==========================================

        // 将 Major.Minor.Patch 转换为 32位整数
        // 规则: Major占高8位，Minor占中12位，Patch占低12位
        // 支持范围: Major(0-255).Minor(0-4095).Patch(0-4095)
        private static int GetVersionInt(int major, int minor, int patch)
        {
            return (major << 24) | (minor << 12) | patch;
        }

        // 更新数据库版本号
        private static void UpdateVersion(SQLiteConnection conn, int major, int minor, int patch)
        {
            int intVersion = GetVersionInt(major, minor, patch);
            conn.Execute($"PRAGMA user_version = {intVersion}");
            System.Diagnostics.Debug.WriteLine($"[DB升级] 数据库已升级到: {major}.{minor}.{patch}");
        }

        // 把整数还原成字符串 (仅用于调试显示)
        private static string IntToSemVer(int intVersion)
        {
            if (intVersion == 0) return "1.0.1";
            int major = (intVersion >> 24) & 0xFF;
            int minor = (intVersion >> 12) & 0xFFF;
            int patch = intVersion & 0xFFF;
            return $"{major}.{minor}.{patch}";
        }

        // ==========================================
        // 5. 具体的 SQL 操作
        // ==========================================

        private static void EnsureSchema(SQLiteConnection conn)
        {
            conn.Execute(@"
                CREATE TABLE IF NOT EXISTS Banners (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ImagePath TEXT NOT NULL,
                    IconPath TEXT,
                    PersonPath TEXT,
                    SteamUrl TEXT,
                    SortOrder INTEGER DEFAULT 0
                );");

            conn.Execute(@"
                CREATE TABLE IF NOT EXISTS Games (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    CoverImage TEXT,
                    Description TEXT,
                    Type INTEGER DEFAULT 1
                );");
        }

        // 对应 1.0.1 版本的数据插入
        private static void InsertVersion101Data(SQLiteConnection conn)
        {
            string insertBannerSql = @"
                INSERT INTO Banners (ImagePath, IconPath, PersonPath, SteamUrl, SortOrder) 
                VALUES (@ImagePath, @IconPath, @PersonPath, @SteamUrl, @SortOrder)";

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a1r.jpg",
                IconPath = "", // 确保后缀正确
                PersonPath = "/Assets/Images/banner/person/a1r.png",
                SteamUrl = "https://store.steampowered.com/app/2138090/_Remake/",
                SortOrder = 1
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a2-a10.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a2-a10.png",
                SteamUrl = "",
                SortOrder = 2
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a11.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a11.png",
                SteamUrl = "https://store.steampowered.com/app/936160/__DX/",
                SortOrder = 3
            }); conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a12.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a12.png",
                SteamUrl = "https://store.steampowered.com/app/936180/_2_DX/",
                SortOrder = 4
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a13.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a13.png",
                SteamUrl = "https://store.steampowered.com/app/936190/_3_DX/",
                SortOrder = 5
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a14.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a14.png",
                SteamUrl = "https://store.steampowered.com/app/1152300/__DX/",
                SortOrder = 6
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a15.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a15.png",
                SteamUrl = "https://store.steampowered.com/app/1152310/__DX/",
                SortOrder = 7
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a16.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a16.png",
                SteamUrl = "https://store.steampowered.com/app/1152320/__DX/",
                SortOrder = 8
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a17.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a17.png",
                SteamUrl = "https://store.steampowered.com/app/1502970/__DX/",
                SortOrder = 9
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a18.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a18.png",
                SteamUrl = "https://store.steampowered.com/app/1502980/__DX/",
                SortOrder = 10
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a19.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a19.png",
                SteamUrl = "https://store.steampowered.com/app/1502990/__DX/",
                SortOrder = 11
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a20.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a20.png",
                SteamUrl = "https://store.steampowered.com/app/1045620/_4/",
                SortOrder = 12
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a21.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a21.png",
                SteamUrl = "https://store.steampowered.com/app/1121560/_/",
                SortOrder = 13
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a22.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a22.png",
                SteamUrl = "https://store.steampowered.com/app/1257290/2/",
                SortOrder = 14
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a23.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a23.png",
                SteamUrl = "https://store.steampowered.com/app/1621310/2/",
                SortOrder = 15
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a24.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a24.png",
                SteamUrl = "https://store.steampowered.com/app/1999770/3/",
                SortOrder = 16
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a25-rw.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a25-rw.png",
                SteamUrl = "https://store.steampowered.com/app/3259600/_/",
                SortOrder = 17
            });

            conn.Execute(insertBannerSql, new
            {
                ImagePath = "/Assets/Images/banner/bg/a26.jpg",
                IconPath = "",
                PersonPath = "/Assets/Images/banner/person/a26.png",
                SteamUrl = "https://store.steampowered.com/app/3123410/_/",
                SortOrder = 18
            });

            // 插入 Game 数据
            string insertGameSql = @"
                INSERT INTO Games (Title, CoverImage, Description, Type)
                VALUES (@Title, @CoverImage, @Description, @Type)";

            conn.Execute(insertGameSql, new[]
            {
                new { Title = "A11 萝乐娜的炼金工房 ～亚兰德的炼金术士～", CoverImage = "/Assets/Images/Library/a11.png", Description = "", Type = 1 },
                new { Title = "A12 托托莉的炼金工房 ～亚兰德之炼金术士2～", CoverImage = "/Assets/Images/Library/a12.png", Description = "", Type = 1 },
                new { Title = "A13 梅露露的炼金工房 ～亚兰德之炼金术士3～", CoverImage = "/Assets/Images/Library/a13.png", Description = "", Type = 1 },
                new { Title = "A14 爱夏的炼金工房 ～黄昏大地的炼金术士～", CoverImage = "/Assets/Images/Library/a14.jpg", Description = "", Type = 1 },
                new { Title = "A15 爱丝卡&罗吉的炼金工房 ～黄昏天空的炼金术士～", CoverImage = "/Assets/Images/Library/a15.jpg", Description = "", Type = 1 },
                new { Title = "A16 夏莉的炼金工房 ～黄昏海洋的炼金术士～", CoverImage = "/Assets/Images/Library/a16.jpg", Description = "", Type = 1 },
                new { Title = "A17 苏菲的炼金工房 ～不可思议书的炼金术士～", CoverImage = "/Assets/Images/Library/a17.jpg", Description = "", Type = 1 },
                new { Title = "A18 菲莉丝的炼金工房 ～不可思议旅的炼金术士～", CoverImage = "/Assets/Images/Library/a18.jpg", Description = "", Type = 1 },
                new { Title = "A19 莉迪&苏瑞的炼金工房 ～不可思议绘画的炼金术士～", CoverImage = "/Assets/Images/Library/a19.jpg", Description = "", Type = 1 },
                new { Title = "A20 露露亚的炼金工房 ～亚兰德之炼金术士4～", CoverImage = "/Assets/Images/Library/a20.png", Description = "", Type = 1 },
                new { Title = "A21 莱莎的炼金工房 ～常暗女王与秘密藏身处～", CoverImage = "/Assets/Images/Library/a21.jpg", Description = "", Type = 1 },
                new { Title = "A22 莱莎的炼金工房2 ～失落传说与秘密妖精～", CoverImage = "/Assets/Images/Library/a22.jpg", Description = "", Type = 1 },
                new { Title = "A23 苏菲的炼金工房2 ～不可思议梦的炼金术士～", CoverImage = "/Assets/Images/Library/a23.jpg", Description = "", Type = 1 },
                new { Title = "A24 莱莎的炼金工房3 ～终结之炼金术士与秘密钥匙～", CoverImage = "/Assets/Images/Library/a24.jpg", Description = "", Type = 1 },
                new { Title = "A25 蕾斯莱莉娅娜的炼金工房 ～忘却的炼金术与极夜的解放者～", CoverImage = "/Assets/Images/Library/a25.jpg", Description = "", Type = 1 },
                new { Title = "A26 优米雅的炼金工房 ～追忆之炼金术士与幻创之地～", CoverImage = "/Assets/Images/Library/a26.jpg", Description = "", Type = 1 },
                new { Title = "A1RE 玛莉的炼金工房 ～萨尔布鲁克的炼金术士～", CoverImage = "/Assets/Images/Library/a1r.jpg", Description = "", Type = 2 },
                new { Title = "A25RW 红色的炼金术士和白色的守护者 ～蕾斯莱莉娅娜的炼金工房～", CoverImage = "/Assets/Images/Library/a25rw.jpg", Description = "", Type = 3 },
            });
        }

        // ==========================================
        // 6. 数据读取方法
        // ==========================================

        public static List<BannerItem> GetBanners()
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                return conn.Query<BannerItem>("SELECT * FROM Banners ORDER BY SortOrder").ToList();
            }
        }
         
        public static List<GameEntry> GetGames()
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                return conn.Query<GameEntry>("SELECT * FROM Games").ToList();
            }
        }
    }
}
