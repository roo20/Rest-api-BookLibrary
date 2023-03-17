using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RESTful_api.Data;
using RESTful_api.Dtos;
using RESTful_api.Helpers;
using RESTful_api.Models;
using RESTful_api.ResourceParameters;

namespace RESTful_api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookRepo _bookRepo;
    private readonly IMapper _mapper;
    //private readonly IValidator<BookCreateDto> _validator;
    //private readonly ILogger<BookController> _logger;
    //private readonly ILoggerManager _loggerx;

    public BookController(IBookRepo bookRepo,
        IMapper mapper
       )
    {
        _bookRepo = bookRepo;
        _mapper = mapper;

    }

    [HttpGet(Name ="GetBooks")]
    public ActionResult GetBooks(
        [FromQuery] BookResourceParameters bookResourceParameters
        )
    {
        var bookItem = _bookRepo.GetAllBooks(bookResourceParameters);
        var previousPageLink=bookItem.HasPrevious ? CreateBookResourceUri(bookResourceParameters,ResourceUriType.Previous) : null;
        var nextPageLink = bookItem.HasNext ? CreateBookResourceUri(bookResourceParameters, ResourceUriType.Next) : null;

        var pageinationMetadata = new
        {
            totalcount=bookItem.TotalCount,
            pageSize=bookItem.PageSize,
            currentPage=bookItem.CurrentPage,
            totalPages=bookItem.TotalPages,
            previousPageLink=previousPageLink,
            nextPageLink=nextPageLink
        };
       

        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageinationMetadata));

        return Ok(_mapper.Map<IEnumerable<BookReadDto>>(bookItem));

    }

    [HttpGet("{id}", Name = "GetBookById")]
    public ActionResult<BookReadDto> GetBookById(int id)
    {
        var bookItem = _bookRepo.GetBookById(id);
        if (bookItem != null)
        {
            return Ok(_mapper.Map<BookReadDto>(bookItem));

        }
        return NotFound();
    }

    [HttpPost]
    public ActionResult<BookReadDto> CreateBook(BookCreateDto bookCreateDto)
    {

        //var validatorResult = _validator.Validate(bookCreateDto);

        //if (!validatorResult.IsValid)
        //{
        //    return StatusCode(StatusCodes.Status400BadRequest, validatorResult.Errors);
        //}

        var bookModel = _mapper.Map<Book>(bookCreateDto);
        _bookRepo.CreateBook(bookModel);
        _bookRepo.SaveChanges();

        var bookReadDto = _mapper.Map<BookReadDto>(bookModel);

        return CreatedAtRoute(nameof(GetBookById), new { id = bookReadDto.Id }, bookReadDto);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteBook(int id)
    {
        var bookToDelete = _bookRepo.GetBookById(id);
        if (bookToDelete == null)
        {
            return NotFound();
        }
        _bookRepo.DeleteBook(bookToDelete);
        _bookRepo.SaveChanges();
        return NoContent();
    }


    private string? CreateBookResourceUri( BookResourceParameters bookResourceParameters,ResourceUriType type)
    {
        switch (type)
        {
            case ResourceUriType.Previous:
                return Url.Link("GetBooks", new
                {
                    pageNumber=bookResourceParameters.PageNumber-1,
                    pageSize=bookResourceParameters.PageSize,
                    genre=bookResourceParameters.Genre,
                    searchQuery=bookResourceParameters.SearchQuery,
                });
            case ResourceUriType.Next:
                return Url.Link("GetBooks", new
                {
                    pageNumber = bookResourceParameters.PageNumber + 1,
                    pageSize = bookResourceParameters.PageSize,
                    genre = bookResourceParameters.Genre,
                    searchQuery = bookResourceParameters.SearchQuery,
                });
            default:
                return Url.Link("GetBooks", new
                {
                    pageNumber = bookResourceParameters.PageNumber,
                    pageSize = bookResourceParameters.PageSize,
                    genre = bookResourceParameters.Genre,
                    searchQuery = bookResourceParameters.SearchQuery,
                });
        }

    }
}
