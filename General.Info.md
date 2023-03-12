# Restful API asp.net

### info

- be always consistent with naming and use verbs ie:
  ```coffeescript
  GET     /api/{apiVersion}/Books
  GET     /api/{apiVersion}/Books/{BookId}
  POST    /api/{apiVersion}/Books
  PUT     /api/{apiVersion}/Books/{BookId}
  PATCH   /api/{apiVersion}/Books/{BookId}
  DELETE  /api/{apiVersion}/Books/{BookId}
  ```
- Payloads are the views in MVC and normally the payload are in json but can be also XML ,,,

### HTTP Response codes

# first setup

### Controller

basic setup for controller goes as follows:

```Csharp

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")] //api/v1/books Base URI
public class BooksController : ControllerBase
{

    //Ctor and all the dependencies (Mapper, Repositories,loggers,...)

[HttpGet]  // GET /api/v1/books return all books
public ActionResult<IEnumerable<BookReadDto>> GetBooks()
    {}
[HttpGet("{id}", Name = "GetBookById")] // GET /api/v1/books/1 return book with id=1
public ActionResult<BookReadDto> GetBookById(int id)
    {}
[HttpPost] // POST /api/v1/books Create new book
public ActionResult<BookReadDto> CreateBook(BookCreateDto bookCreateDto)
    {}
}
```

when using **api versioning** it needs to be added to services in **Program.cs**
it will be like follows:

```csharp

builder.Services.AddControllers();

builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.ReportApiVersions = true;
});

var app = builder.Build();
```

when working with multiple versions of the api there would be multiple versions of the controller with diffrent

```csharp
[ApiVersion("2.0")]
```

# AppDbContext

create a class for AppDbContext:

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
    {
    }

    public DbSet<Book> Books { get; set; }

}
```

need to be added as a service to the **Program.cs**

```csharp program.cs
builder.Services.AddDbContext<AppDbContext>(
    option => option.UseSqlite(builder.Configuration.GetConnectionString("Default")));
```

# Repository

1- create interface to interact with AppDbContext

```cs
public interface IBookRepo
{
    bool SaveChanges();
    IEnumerable<Book> GetAllBooks();
    Book GetBookById(int id);
    void CreateBook(Book book);
    void UpdateBook(Book book);
    void DeleteBook(Book book);
}
```

2- create class that implement this interface

```cs
public class BookRepo : IBookRepo
{
    private readonly AppDbContext _appDbContext; // Dependency injection of AppDbContext into BookRepo
    public BookRepo(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public IEnumerable<Book> GetAllBooks() // Get list of all books in database
    {
        return _appDbContext.Books.ToList();
    }
    .
    .
    public bool SaveChanges() // save changes normally after editing database Create(POST), Update(PUT), Delete(DELETE), PartianUpdate(PATCH)
    {
        return (_appDbContext.SaveChanges() >= 0);
    }
}
```

3- add as scoped Service inside the **Program.cs**

```csharp
builder.Services.AddScoped<IBookRepo, BookRepo>();
```

# DTO

# Mapper


