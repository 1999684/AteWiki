namespace AtelierWiki.Data
{
    public class GameEntry
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CoverImage { get; set; }
        public string Description { get; set; }

        // 新增字段：1=正作, 2=重绘/重制, 3=外传
        public int Type { get; set; }

        // 辅助属性：用于界面显示分组标题 (只读)
        public string TypeName
        {
            get
            {
                switch (Type)
                {
                    case 1: return "正作";
                    case 2: return "重制";
                    case 3: return "外传";
                    default: return "正作";
                }
            }
        }
    }
}
