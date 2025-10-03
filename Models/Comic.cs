namespace ComicSystem.Models
{
    public class Comic
    {
        public int ComicId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Price { get; set; }
        public int Stock { get; set; }
    }
}