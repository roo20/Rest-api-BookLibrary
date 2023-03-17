using RESTful_api.Helpers;
using RESTful_api.Models;
using RESTful_api.ResourceParameters;

namespace RESTful_api.Data;

public interface IBookRepo
{
    bool SaveChanges();

    IEnumerable<Book> GetAllBooks();
    PagedList<Book> GetAllBooks(BookResourceParameters bookResourceParameters);
    Book GetBookById(int id);
    void CreateBook(Book book);
    void UpdateBook(Book book);
    void DeleteBook(Book book);
}
