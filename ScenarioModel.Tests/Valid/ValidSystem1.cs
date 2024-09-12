namespace ScenarioModel.Tests.Valid;

public static class ValidSystem1
{
    public static System Generate()
        => new()
        {
            Entities = new()
            {
                new() { Name = "E1", State = new() { Name = "S1" } },
                new() { Name = "E2" },
            },
            StateTypes = new()
            {
                new() { Name = "ST1", States = [ new() { Name = "S1", Transitions = ["S2"] }, new() { Name = "S2" }] },
            }
            
        };
}