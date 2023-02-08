using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RESTful_api.Data;
using RESTful_api.Dtos;
using RESTful_api.Models;

namespace RESTful_api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly IBookRepo _bookRepo;
    private readonly IMapper _mapper;

    public BookController(IBookRepo bookRepo, IMapper mapper)
	{
		_bookRepo=bookRepo;
		_mapper=mapper;
	}

	[HttpGet]
	public ActionResult<IEnumerable<BookReadDto>> GetBooks()
	{
		var bookItem= _bookRepo.GetAllBooks();
		return Ok(_mapper.Map<IEnumerable<BookReadDto>>(bookItem));

	}

    [HttpGet("{id}",Name = "GetBookById")]
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
        var bookModel=_mapper.Map<Book>(bookCreateDto);
        _bookRepo.CreateBook(bookModel);
        _bookRepo.SaveChanges();

        var bookReadDto=_mapper.Map<BookReadDto>(bookModel);

        return CreatedAtRoute(nameof(GetBookById), new { id=bookReadDto.Id},bookReadDto );
    }

}
