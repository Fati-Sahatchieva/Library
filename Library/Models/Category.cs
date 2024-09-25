using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();

    }
}
