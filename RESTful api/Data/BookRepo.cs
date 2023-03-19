using RESTful_api.Dtos;
using RESTful_api.Helpers;
using RESTful_api.Models;
using RESTful_api.ResourceParameters;
using RESTful_api.Services;

namespace RESTful_api.Data;

public class BookRepo : IBookRepo
{
    private readonly AppDbContext _appDbContext;
    private readonly IPropertyMappingService _propertyMappingService;
    public BookRepo(AppDbContext appDbContext, IPropertyMappingService propertyMappingService)
    {
        _appDbContext = appDbContext;
        _propertyMappingService = propertyMappingService;
    }

    public void CreateBook(Book book)
    {
        if (book == null)
        {
            throw new ArgumentNullException(nameof(book));
        }

        _appDbContext.Books.Add(book);
    }

    public IEnumerable<Book> GetAllBooks()
    {
        return _appDbContext.Books.ToList();
    }
    public PagedList<Book> GetAllBooks(BookResourceParameters bookResourceParameters)
    {
        if (bookResourceParameters == null)
        {
            throw new ArgumentNullException(nameof(bookResourceParameters));
        }

       var collection = _appDbContext.Books as IQueryable<Book>;

        //Filter
        if (!string.IsNullOrWhiteSpace(bookResourceParameters.Genre))
        {
            var genre = bookResourceParameters.Genre.Trim();
            collection = collection.Where(a => a.Genre == genre);
        }

        //Search
        if (!string.IsNullOrWhiteSpace(bookResourceParameters.SearchQuery))
        {
            var searchQuery = bookResourceParameters.SearchQuery.Trim();

            collection = collection.Where(a => a.Title.Contains(searchQuery) 
            || a.Author.Contains(searchQuery) 
            || a.Description.Contains(searchQuery));
        }

        //Sorting
        if (!string.IsNullOrWhiteSpace(bookResourceParameters.OrderBy))
        {
            var bookPropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<BookReadDto,Book>();
            collection=collection.ApplySort(bookResourceParameters.OrderBy, bookPropertyMappingDictionary);
        }

        //Paging
        return PagedList<Book>.Create(collection, bookResourceParameters.PageNumber,bookResourceParameters.PageSize);
            
    }

    public Book GetBookById(int id)
    {

        return _appDbContext.Books.FirstOrDefault(b => b.Id == id);
    }

    public bool SaveChanges()
    {
        return (_appDbContext.SaveChanges() >= 0);
    }

    public void UpdateBook(Book book)
    {
        throw new NotImplementedException();
    }

    public void DeleteBook(Book book)
    {
        if (book == null)
        {
            throw new ArgumentNullException(nameof(book));
        }
        _appDbContext.Books.Remove(book);
    }

   
}
