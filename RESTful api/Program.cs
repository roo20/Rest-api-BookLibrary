using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RESTful_api.Data;
using RESTful_api.Dtos;
using RESTful_api.Models;
using RESTful_api.Validators;
using System;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers(options =>
{
  options.ReturnHttpNotAcceptable = true;
}).AddXmlDataContractSerializerFormatters(); 

builder.Services.AddDbContext<AppDbContext>(
    option => option.UseSqlite(builder.Configuration.GetConnectionString("Default"))
);


builder.Services.AddScoped<IBookRepo, BookRepo>();
builder.Services.AddScoped<IValidator<BookCreateDto>, BookValidator>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    setupAction.ReportApiVersions = true;
});

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
