using RESTful_api.Models;

namespace RESTful_api.Data;

public class BookRepo : IBookRepo
{
    private readonly AppDbContext _appDbContext;
    public BookRepo(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
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
        return  _appDbContext.Books.ToList();
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
