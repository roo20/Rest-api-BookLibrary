using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
        [FromQuery] BookResourceParameters bookResourceParameters
        )
    {
        if (!_propertyMappingService.ValidMappingExistsFor<BookReadDto,Book>(bookResourceParameters.OrderBy))
        {
            return BadRequest();
        }

        if (!_propertyCheckerService.TypeHasProperties<BookReadDto>(bookResourceParameters.Fields))
        {
            return BadRequest(
                _problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode:400,
                detail:$"Not All Requested data shaping fields exits on the resource : {bookResourceParameters.Fields}"));
        }

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

        return Ok(_mapper.Map<IEnumerable<BookReadDto>>(bookItem)
            .ShapeData(bookResourceParameters.Fields));

    }

    [HttpGet("{id}", Name = "GetBookById")]
    public ActionResult GetBookById(int id, [FromQuery] string? fields)
    {
        var bookItem = _bookRepo.GetBookById(id);

        if (!_propertyCheckerService.TypeHasProperties<BookReadDto>(fields))
        {
            return BadRequest(
                _problemDetailsFactory.CreateProblemDetails(HttpContext,
                statusCode: 400,
                detail: $"Not All Requested data shaping fields exits on the resource : {fields}"));
        }

        if (bookItem != null)
        {
            return Ok(_mapper.Map<BookReadDto>(bookItem).ShapeData(fields));
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
}
