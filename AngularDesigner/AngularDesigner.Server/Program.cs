var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/getscenario", () =>
{
    return """
    Entity Actor {
      EntityType ET1
      State "Amy Stake"
      CharacterStyle Red
    }
    
    EntityType ET1 {
      SM Name
    }
    
    SM Name {
      State "Amy Stake"
      State "Brock Lee"
      State "Clara Nett"
      State "Dee Zaster"
      "Amy Stake" -> "Brock Lee" : ChangeName
      "Brock Lee" -> "Clara Nett" : ChangeName
      "Clara Nett" -> "Dee Zaster" : ChangeName
    }
    
    Scenario "Scenario recorded by hooks" {
      Dialog SayName {
        Text "The actor {Actor.State} says hello and introduces themselves"
        Character Actor
      }
      While <Actor.State != "Dee Zaster"> {
        If <Actor.State == "Amy Stake"> {
          Dialog {
            Text "The actor Mrs Stake makes a bad pun to do with their name"
            Character Actor
          }
        }
        If <Actor.State == "Brock Lee"> {
          Dialog {
            Text "The actor Mr Lee makes a bad pun to do with their name"
            Character Actor
          }
        }
        If <Actor.State == "Clara Nett"> {
          Dialog {
            Text "The actor Mrs Nett makes a bad pun to do with their name"
            Character Actor
          }
        }
        Transition {
          Actor : ChangeName
        }
      }
      If <Actor.State == "Dee Zaster"> {
        Dialog {
          Text "The actor Mr Zaster makes a bad pun to do with their name"
          Character Actor
        }
      }
    }
    """;
})
.WithName("GetScenario")
.WithOpenApi();

app.MapFallbackToFile("/index.html");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
