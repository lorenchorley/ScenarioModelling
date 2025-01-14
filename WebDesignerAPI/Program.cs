var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var metastory = """

    """;

app.MapGet("/getmetastory", () =>
   {
       return metastory;
   })
   .WithName("GetMetaStory")
   .WithOpenApi();

app.Run();
