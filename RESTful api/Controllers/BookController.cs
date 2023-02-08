using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RESTful_api.Data;
using RESTful_api.Dtos;

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
		Console.WriteLine("getting books...");
		var bookItem= _bookRepo.GetAllBooks();
		return Ok(_mapper.Map<IEnumerable<BookReadDto>>(bookItem));

	}

}
