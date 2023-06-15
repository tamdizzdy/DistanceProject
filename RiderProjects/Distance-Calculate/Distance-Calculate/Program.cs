using Distance_Calculate.Data;
using Distance_Calculate.Interface;
using Distance_Calculate.Repository;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDistanceRepository, DistanceRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "CalculationDistance Web API",
            Description = "Document for Web API",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Tamm",
                Email = string.Empty,
                Url = new Uri("https://www.facebook.com/tamm/"),
            }
            
        }
    );
    c.EnableAnnotations();
});
builder.Services.AddDbContext<WebAPIContext>(
    o => o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
        c.DefaultModelsExpandDepth(-1);
        c.DocExpansion(DocExpansion.None);
    });
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();