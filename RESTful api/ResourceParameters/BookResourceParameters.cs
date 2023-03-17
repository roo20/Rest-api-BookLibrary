namespace RESTful_api.ResourceParameters;

public class BookResourceParameters
{
    const int maxPageSize = 20;
    private int pageSize = 10;

    public string? Genre { get; set; }
    public string? SearchQuery { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get => pageSize; set => pageSize = (value > maxPageSize) ? maxPageSize : value; }

    public string? OrderBy { get; set; }

}
