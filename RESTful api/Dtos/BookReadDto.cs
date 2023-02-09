using System.ComponentModel.DataAnnotations;

namespace RESTful_api.Dtos;

public class BookReadDto
{

 
    public int Id { get; set; }


    public string Author { get; set; } = string.Empty;


    public string Title { get; set; } = string.Empty;

    public string? Genre { get; set; }


    public float Price { get; set; } = 0f;

    public DateTime PublishDate { get; set; }
    public string? Description { get; set; }
}
