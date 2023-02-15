using RESTful_api.Models;

namespace RESTful_api.Data;

public interface IBookRepo
{
    bool SaveChanges();

    IEnumerable<Book> GetAllBooks();
    Book GetBookById(int id);
    void CreateBook(Book book);
    void UpdateBook(Book book);
    void DeleteBook(Book book);
}
