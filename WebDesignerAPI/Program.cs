using Microsoft.AspNetCore.Mvc;
using ScenarioModelling;
using ScenarioModelling.CoreObjects;
using ScenarioModelling.Serialisation.CustomSerialiser.Reserialisation;
using ScenarioModelling.Serialisation.ProtoBuf;
using System.Text;

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
    
    MetaStory "MetaStory recorded by hooks" {
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

app.MapGet("/getmetastory", () =>
   {
       return metastory;
   })
   .WithName("GetMetaStory")
   .WithOpenApi();

ScenarioModellingContainer decodingContainer = new();

app.MapPost("/decode", async (HttpRequest request) =>
    {
        using var scope = decodingContainer.StartScope(); // Correct usage ?

        using StreamReader reader = new StreamReader(request.Body, CustomContextSerialiser.CompressionEncoding, true, 1024, true);
        string encodedContextText = await reader.ReadToEndAsync();
        
        Context context =
            scope.GetService<Context>()
                 .UseSerialiser<CustomContextSerialiser>(new() { { "Compress", "true" } })
                 .LoadContext<CustomContextSerialiser>(encodedContextText)
                 .RemoveSerialiser<CustomContextSerialiser>()
                 .UseSerialiser<CustomContextSerialiser>()
                 .Initialise();

        var result = context.Serialise<CustomContextSerialiser>();
        var message = result.Match(s => s, e => e.Message);

        if (result.IsSuccess)
        {
            return Results.Ok(message);
        }
        else
        {
            return Results.BadRequest(message);
        }
    })
   .WithName("Decode")
   .WithOpenApi();

ScenarioModellingContainer encodingContainer = new();
app.MapPost("/encode", async (HttpRequest request) =>
    {
        using var scope = encodingContainer.StartScope(); // Correct usage ?
        
        using StreamReader reader = new StreamReader(request.Body, CustomContextSerialiser.CompressionEncoding, true, 1024, true);
        string clearContextText = await reader.ReadToEndAsync();

        Context context =
            scope.GetService<Context>()
                 .UseSerialiser<CustomContextSerialiser>()
                 .LoadContext<CustomContextSerialiser>(clearContextText)
                 .RemoveSerialiser<CustomContextSerialiser>()
                 .UseSerialiser<CustomContextSerialiser>(new() { { "Compress", "true" } })
                 .Initialise();

        var result = context.Serialise<CustomContextSerialiser>();
        var message = result.Match(s => s, e => e.Message);

        if (result.IsSuccess)
        {
            return Results.Ok(message);
        }
        else
        {
            return Results.BadRequest(message);
        }
    })
   .WithName("Encode")
   .WithOpenApi();

app.Run();
