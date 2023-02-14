using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using RESTful_api.Contracts;
using RESTful_api.Data;
using RESTful_api.Dtos;
using RESTful_api.Models;
using RESTful_api.Validators;

namespace RESTful_api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookRepo _bookRepo;
    private readonly IMapper _mapper;
    private readonly IValidator<BookCreateDto> _validator;


    public BookController(IBookRepo bookRepo,
        IMapper mapper,
        IValidator<BookCreateDto> validator)
    {
        _bookRepo = bookRepo;
        _mapper = mapper;
        _validator = validator;

    }

    [HttpGet]
    public ActionResult<IEnumerable<BookReadDto>> GetBooks()
    {

        var bookItem = _bookRepo.GetAllBooks();
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

        var validatorResult = _validator.Validate(bookCreateDto);

        if (!validatorResult.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, validatorResult.Errors);
        }

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

}
