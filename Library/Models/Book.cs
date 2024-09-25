using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
 
        [Required]
        [MaxLength(60)]
        public string Title { get; set; }

        [Required]
        [MaxLength(60)]
        public string Author { get; set; }

        [Required]
        [Range(1000, 9999)]
        public int PublicationYear { get; set; }

        [Required]
        [Range(1, 1000)]
        public int BookCopies { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public List<Review> Reviews { get; set; } = new List<Review>();

    }
}
