using Microsoft.EntityFrameworkCore;
using RESTful_api.Data;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(
    option => option.UseSqlite(builder.Configuration.GetConnectionString("Default"))
    ) ;

builder.Services.AddScoped<IBookRepo, BookRepo>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
