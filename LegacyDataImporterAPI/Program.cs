using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("all", builder => builder.AllowAnyOrigin()
                                                .AllowAnyHeader()
                                                .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7000); // to listen for incoming http connection on port 7000
});

var app = builder.Build();

string swaggerBasePath = "api";

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = swaggerBasePath + "/swagger/{documentName}/swagger.json";
    });

    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);

        options.SwaggerEndpoint($"/{swaggerBasePath}/swagger/v1/swagger.json", $"APP API - v1");

        options.RoutePrefix = $"{swaggerBasePath}/swagger";
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
