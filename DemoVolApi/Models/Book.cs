namespace DemoVolApi.Models
{
    [Table("Books")]
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
    }
}
