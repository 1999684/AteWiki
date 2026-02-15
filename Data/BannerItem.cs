namespace AtelierWiki.Data
{
    public class BannerItem
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } // 背景图 (Background)
        public string IconPath { get; set; }  // 左侧图标 (Logo)
        public string PersonPath { get; set; } // 右侧人物 (Character)
        public string SteamUrl { get; set; }
        public int SortOrder { get; set; }
    }
}
