using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RESTful_api.Models;

[Table("Books")]
public class Book
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Auther { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; }=string.Empty;

    [Required]
    public string Genre { get; set; } = string.Empty;

    [Required]
    public float Price { get; set; }

    [Required]
    public DateTime PublishDate { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;



}
