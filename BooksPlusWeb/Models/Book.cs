using System.ComponentModel.DataAnnotations;

namespace BooksPlusWeb.Models;

// One row of the Book table, with the rules a new book must follow.
public class Book
{
    public int BookId { get; set; }

    [Required(ErrorMessage = "Choose a category.")]
    [Display(Name = "Category")]
    public int? CategoryId { get; set; }

    [Required(ErrorMessage = "Enter the book's title.")]
    [StringLength(255, ErrorMessage = "A title can be at most 255 characters.")]
    public string Title { get; set; } = "";

    [Display(Name = "Publish date")]
    public DateTime PublishDate { get; set; }

    [Range(0.01, 500, ErrorMessage = "Enter a price from $0.01 to $500.")]
    public decimal? Price { get; set; }

    public string? Summary { get; set; }
}
