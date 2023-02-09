
using System.ComponentModel.DataAnnotations;

namespace RESTful_api.Dtos;

public class BookCreateDto
{
    [Required]
    public string Author { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Genre { get; set; }

    [Required]
    public float Price { get; set; } = 0f;

    [Required]
    public DateTime PublishDate { get; set; }

    [Required]
    public string Description { get; set; }
}
