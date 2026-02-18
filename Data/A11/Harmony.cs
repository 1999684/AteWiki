namespace AtelierWiki.Data.A11
{
    public class Harmony
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }

        // 材料 1-4
        public string Mat1 { get; set; }
        public string Mat2 { get; set; }
        public string Mat3 { get; set; }
        public string Mat4 { get; set; }

        public string Category { get; set; }      // 类别
        public string Acquisition { get; set; }   // 习得方式

        public string BgImagePath { get; set; }
        public bool IsAddButton { get; set; } = false;
    }
}
