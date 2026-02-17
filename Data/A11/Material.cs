namespace AtelierWiki.Data.A11
{
    public class Material
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string Locations { get; set; }
        public string Monsters { get; set; }

        public string BgImagePath { get; set; }

        public bool IsAddButton { get; set; } = false;
    }
}
