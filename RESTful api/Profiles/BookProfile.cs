using AutoMapper;
using RESTful_api.Dtos;
using RESTful_api.Models;

namespace RESTful_api.Profiles;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookReadDto>();

        CreateMap<BookCreateDto, Book>();
    }

}
