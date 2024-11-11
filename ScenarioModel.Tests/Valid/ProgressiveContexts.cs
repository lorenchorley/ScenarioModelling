namespace ScenarioModel.Tests.Valid;

public static class ProgressiveContexts
{
    public static IEnumerable<string> GetContexts()
    {
        yield return """
            Entity E1 {
              EntityType EntityType1
            }
            
            EntityType EntityType1 {
            }
            """;

        yield return """
            SM SM1 {
            }
            """;

        yield return """
            Entity E1 {
              SM SM1
            }

            SM SM1 {
            }
            """;
    }
}