using System.Reflection;
using GameOfLife.Api.Middlewares;
using GameOfLife.Infrastructure.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDatabaseContext(builder.Configuration);
builder.Services.AddDomainServices();
builder.Services.AddRepositories();
builder.Services.AddUseCases();
builder.Services.AddCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Game of Life API",
            Description = "This API simulates the features of Conway's Game Of Life"
        });

    var xml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var path = Path.Combine(AppContext.BaseDirectory, xml);

    options.IncludeXmlComments(path, includeControllerXmlComments: true);
});

var app = builder.Build();
app.UseMiddleware<ExceptionHandlerMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();