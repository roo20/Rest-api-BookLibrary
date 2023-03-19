using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using RESTful_api.Data;
using RESTful_api.Dtos;
using RESTful_api.Helpers;
using RESTful_api.Models;
using RESTful_api.ResourceParameters;
using RESTful_api.Services;

namespace RESTful_api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookRepo _bookRepo;
    private readonly IMapper _mapper;
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly IPropertyCheckerService _propertyCheckerService;
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    public BookController(IBookRepo bookRepo,
        IMapper mapper
        , IPropertyMappingService propertyMappingService
        , IPropertyCheckerService propertyCheckerService
        , ProblemDetailsFactory problemDetailsFactory
       )
    {
        _bookRepo = bookRepo;
        _mapper = mapper;
        _propertyMappingService = propertyMappingService;
        _propertyCheckerService = propertyCheckerService;
        _problemDetailsFactory = problemDetailsFactory;
    }

    [HttpGet(Name ="GetBooks")]
    public ActionResult GetBooks(
        [FromQuery] BookResourceParameters bookResourceParameters,
        [FromHeader(Name ="Accept")] string? mediaType
        )
    {
        //check if the mediatype is valid media type
        if (!MediaTypeHeaderValue.TryParse(mediaType,out var parsedMediaType))
        {
            return BadRequest(
                _problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: 400,
                detail: $"Accept header media type value is not a valid media type"));
        }

        //Check if Order Parameters are valid
        if (!_propertyMappingService.ValidMappingExistsFor<BookReadDto,Book>(bookResourceParameters.OrderBy))
        {
            return BadRequest();
        }

        //Check if  datashaping Field Parameters are valid
        if (!_propertyCheckerService.TypeHasProperties<BookReadDto>(bookResourceParameters.Fields))
        {
            return BadRequest(
                _problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode:400,
                detail:$"Not All Requested data shaping fields exits on the resource : {bookResourceParameters.Fields}"));
        }

        //get books from repo after applying (filter, searching, sorting and Paging)
        var bookItem = _bookRepo.GetAllBooks(bookResourceParameters);

        //setup Pagination links
        var pageinationMetadata = new
        {
            totalcount=bookItem.TotalCount,
            pageSize=bookItem.PageSize,
            currentPage=bookItem.CurrentPage,
            totalPages=bookItem.TotalPages
        };
        //add pagination to header response
        Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pageinationMetadata));

        //create links
        var links = CreateLinkForBooks(bookResourceParameters,bookItem.HasNext,bookItem.HasPrevious);

        //shaped data
        var shapedBooks = _mapper.Map<IEnumerable<BookReadDto>>(bookItem)
            .ShapeData(bookResourceParameters.Fields);

        var shapedBooksWithLinks = shapedBooks.Select(book => {
            var bookAsDictionary = book as IDictionary<string, object?>;
            var bookLinks = CreateLinkForBook(
                (int)bookAsDictionary["Id"]
                , null);
            bookAsDictionary.Add("links", bookLinks);
            return bookAsDictionary;
        });

        var linkedCollectionResource = new 
        { 
            value=shapedBooksWithLinks,
            links=links
        };
        
        //return bookitem and apply datashaping at the end 
        return Ok(linkedCollectionResource);

    }

    [HttpGet("{id}", Name = "GetBookById")]
    public ActionResult GetBookById(int id,
        [FromQuery] string? fields,
        [FromHeader(Name = "Accept")] string? mediaType)
    {
        if (!MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType))
        {
            return BadRequest(
                _problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: 400,
                detail: $"Accept header media type value is not a valid media type"));
        }

        var bookItem = _bookRepo.GetBookById(id);

        if (!_propertyCheckerService.TypeHasProperties<BookReadDto>(fields))
        {
            return BadRequest(
                _problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: 400,
                detail: $"Not All Requested data shaping fields exits on the resource : {fields}"));
        }

        if (bookItem == null) return NotFound();

        if (parsedMediaType.MediaType=="application/vnd.company.hateoas+json")
        {
            //create links
            var links = CreateLinkForBook(id, fields);

            var LinkedResourceToRetrun = _mapper
                .Map<BookReadDto>(bookItem)
                .ShapeData(fields)
                as IDictionary<string, object?>;
            LinkedResourceToRetrun.Add("links", links);

            return Ok(LinkedResourceToRetrun); 
        }
        return Ok(_mapper
                .Map<BookReadDto>(bookItem)
                .ShapeData(fields));
    }

    [HttpPost(Name ="CreateBook")]
    public ActionResult<BookReadDto> CreateBook(BookCreateDto bookCreateDto)
    {
        var bookModel = _mapper.Map<Book>(bookCreateDto);
        _bookRepo.CreateBook(bookModel);
        _bookRepo.SaveChanges();

        var bookReadDto = _mapper.Map<BookReadDto>(bookModel);


        //create links
        var links = CreateLinkForBook(bookReadDto.Id, null);

        var LinkedResourceToRetrun = bookReadDto.ShapeData(null)
            as IDictionary<string, object?>;
       
        LinkedResourceToRetrun.Add("links", links);


        return CreatedAtRoute(nameof(GetBookById), new { id = LinkedResourceToRetrun["Id"] }, LinkedResourceToRetrun);
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

    #region helpers   
    private string? CreateBookResourceUri( BookResourceParameters bookResourceParameters,ResourceUriType type)
    {
        switch (type)
        {
            case ResourceUriType.Previous:
                return Url.Link("GetBooks", new
                {
                    fields = bookResourceParameters.Fields,
                    orderBy =bookResourceParameters.OrderBy,
                    pageNumber=bookResourceParameters.PageNumber-1,
                    pageSize=bookResourceParameters.PageSize,
                    genre=bookResourceParameters.Genre,
                    searchQuery=bookResourceParameters.SearchQuery,
                });
            case ResourceUriType.Next:
                return Url.Link("GetBooks", new
                {
                    fields=bookResourceParameters.Fields,
                    orderBy = bookResourceParameters.OrderBy,
                    pageNumber = bookResourceParameters.PageNumber + 1,
                    pageSize = bookResourceParameters.PageSize,
                    genre = bookResourceParameters.Genre,
                    searchQuery = bookResourceParameters.SearchQuery,
                });
            case ResourceUriType.Current:
            default:
                return Url.Link("GetBooks", new
                {
                    fields = bookResourceParameters.Fields,
                    orderBy = bookResourceParameters.OrderBy,
                    pageNumber = bookResourceParameters.PageNumber,
                    pageSize = bookResourceParameters.PageSize,
                    genre = bookResourceParameters.Genre,
                    searchQuery = bookResourceParameters.SearchQuery,
                });
        }

    }

    private IEnumerable<LinkDto> CreateLinkForBook (int  bookId, string? fields)
    {
        var links = new List<LinkDto>();

        if (!string.IsNullOrWhiteSpace(fields))
        {
            links.Add(
                new(Url.Link("GetBooks",new { bookId, fields}),
                "self",
                "Get"));
        }
        else
        {
            links.Add(
               new(Url.Link("GetBooks", new { bookId}),
               "self",
               "Get"));
        }
        return links;
    }

    private IEnumerable<LinkDto> CreateLinkForBooks(
        BookResourceParameters bookResourceParameters,
        bool hasNext,
        bool hasPrevious)
    {
        var links = new List<LinkDto>();

        links.Add(new(CreateBookResourceUri(bookResourceParameters,ResourceUriType.Current),"self","GET"));

        if (hasNext)
        {
            links.Add(new(CreateBookResourceUri(bookResourceParameters, ResourceUriType.Next), "nextPage", "GET"));
        }
        if (hasPrevious)
        {
            links.Add(new(CreateBookResourceUri(bookResourceParameters, ResourceUriType.Previous), "previousPage", "GET"));
        }
        return links;
    }


    #endregion

}
