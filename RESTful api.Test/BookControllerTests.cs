using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTful_api.Controllers;
using RESTful_api.Data;
using RESTful_api.Dtos;
using RESTful_api.Models;
using RESTful_api.Profiles;
using Xunit;

namespace RESTful_api.Test
{
    public class BookControllerTests : IDisposable

    {
        Mock<IBookRepo> mockRepo;
        BookProfile realProfile;
        MapperConfiguration config;
        IMapper mapper;

        public BookControllerTests()
        {
            mockRepo = new Mock<IBookRepo>();
            realProfile = new BookProfile();
            config = new MapperConfiguration(cfg => cfg.AddProfile(realProfile));
            mapper = new Mapper(config);
        }
        public void Dispose()
        {
            mockRepo = null;
            realProfile = null;
            config = null;
            mapper = null;
        }

        private List<Book> GetBooks(int count)
        {
            var books = new List<Book>();
            if (count > 0)
            {
                books.Add(new Book
                {
                    Id = 0,
                    Title = "Harry Potter and the Philosopher's Stone",
                    Author = "Rowling, J.K.",
                    Description = "A young boy discovers he's a wizard and attends a magical school.",
                    Genre = "Fantasy",
                    Price = 15.99f,
                    PublishDate = DateTime.Now,

                });
            }
            return books;
        }


        #region GetAllBooks Tests
        [Fact]
        public void GetAllBooks_Return200OK_WhenDBIsEmpty()
        {

            //Arrange
            mockRepo.Setup(repo => repo.GetAllBooks()).Returns(GetBooks(0));
            var controller = new BookController(mockRepo.Object, mapper);

            //Act 
            var result = controller.GetBooks();

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetAllBooks_ReturnOneItem_WhenDBHasOneResource()
        {
            //Arrange
            mockRepo.Setup(repo => repo.GetAllBooks()).Returns(GetBooks(1));
            var controller = new BookController(mockRepo.Object, mapper);

            //Act 
            var result = controller.GetBooks();

            //Assert
            var okResult = result.Result as OkObjectResult;
            var books = okResult.Value as List<BookReadDto>;
            Assert.Single(books);
        }

        [Fact]
        public void GetAllBooks_Return200OK_WhenDBHasOneResource()
        {
            //Arrange
            mockRepo.Setup(repo => repo.GetAllBooks()).Returns(GetBooks(1));
            var controller = new BookController(mockRepo.Object, mapper);

            //Act 
            var result = controller.GetBooks();

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetAllBooks_ReturnCorrectType_WhenDBHasOneResource()
        {
            //Arrange
            mockRepo.Setup(repo => repo.GetAllBooks()).Returns(GetBooks(1));
            var controller = new BookController(mockRepo.Object, mapper);

            //Act 
            var result = controller.GetBooks();

            //Assert
            Assert.IsType<ActionResult<IEnumerable<BookReadDto>>>(result);
        }
        #endregion

        #region GetBookById Tests
        [Fact]
        public void GetBookByID_Returns404NotFound_WhenNonExistentIDProvided()
        {
            //Arrange
            mockRepo.Setup(repo => repo.GetBookById(0)).Returns(() => null);
            var controller = new BookController(mockRepo.Object, mapper);

            //Act
            var result = controller.GetBookById(1);

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetBookByID_Returns200OK_WhenValidIDProvided()
        {
            //Arrange
            mockRepo.Setup(repo => repo.GetBookById(1)).Returns(new Book
            {
                Id = 1,
                Title = "Test",
                Author = "Test",
                Genre = "Test",
                Description = "Test",
                Price = 0.0f,
                PublishDate = DateTime.Now,

            });
            var controller = new BookController(mockRepo.Object, mapper);

            //Act
            var result = controller.GetBookById(1);

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetBookByID_ReturnsCorrectType_WhenValidIDProvided()
        {
            //Arrange
            mockRepo.Setup(repo => repo.GetBookById(0)).Returns(new Book
            {
                Id = 1,
                Title = "Test",
                Author = "Test",
                Genre = "Test",
                Description = "Test",
                Price = 0.0f,
                PublishDate = DateTime.Now,

            });
            var controller = new BookController(mockRepo.Object, mapper);

            //Act
            var result = controller.GetBookById(1);

            //Assert
            Assert.IsType<ActionResult<BookReadDto>>(result);
        }
        #endregion

        #region CreateBook Tests
        [Fact]
        public void CreateBook_ReturnsCorrectType_WhenValidObjectSubmitted()
        {
            //Arrange
            mockRepo.Setup(repo => repo.GetBookById(0)).Returns(new Book
            {
                Id = 1,
                Title = "Test",
                Author = "Test",
                Genre = "Test",
                Description = "Test",
                Price = 0.0f,
                PublishDate = DateTime.Now,

            });
            var controller = new BookController(mockRepo.Object, mapper);

            //Act
            var result = controller.CreateBook(new BookCreateDto { });

            //Assert
            Assert.IsType<ActionResult<BookReadDto>>(result);
        }

        [Fact]
        public void CreateBook_Returns201Created_WhenValidObjectSubmitted()
        {
            //Arrange
            mockRepo.Setup(repo => repo.GetBookById(1)).Returns(new Book
            {
                Id = 1,
                Title = "Test",
                Author = "Test",
                Genre = "Test",
                Description = "Test",
                Price = 1f,
                PublishDate = DateTime.Now,

            });
            var controller = new BookController(mockRepo.Object, mapper);

            //Act
            var result = controller.CreateBook(new BookCreateDto { });

            //Assert
            Assert.IsType<CreatedAtRouteResult>(result.Result);
        }
        #endregion

        #region UpdateBook Test
        //[Fact]
        //public void UpdateBook_Returns204NoContent_WhenValidObjectSubmitted()
        //{
        //    //Arrange
        //    mockRepo.Setup(repo => repo.GetBookById(1)).Returns(new Book
        //    {
        //        Id = 1,
        //        Title = "Test",
        //        Author = "Test",
        //        Genre = "Test",
        //        Description = "Test",
        //        Price = 1f,
        //        PublishDate = DateTime.Now,

        //    });
        //    var controller = new BookController(mockRepo.Object, mapper);

        //    //Act
        //    var result = controller.UpdateBook(1,new BookUpdateDto { });

        //    //Assert
        //    Assert.IsType<NoContentResult>(result);
        //}

        //[Fact]
        //public void UpdateBook_Returns404NoFound_WhenNonExistentIDSubmitted()
        //{
        //    //Arrange
        //    mockRepo.Setup(repo => repo.GetBookById(0)).Returns(()=> null);
        //    var controller = new BookController(mockRepo.Object, mapper);

        //    //Act
        //    var result = controller.UpdateBook(0, new BookUpdateDto { });

        //    //Assert
        //    Assert.IsType<NotFoundResult>(result);
        //}
        #endregion

    }
}
