﻿//using LanguageExt;
//using YamlDotNet.Serialization;
//using YamlDotNet.Serialization.NamingConventions;

//namespace ScenarioModel.Serialisation;

//public class YamlSerialiserV1 : ISerialiser
//{
//    public string SerialiseScenario(Scenario scenario)
//    {
//        var serializer =
//            new SerializerBuilder()
//                .WithNamingConvention(CamelCaseNamingConvention.Instance)
//                .Build();

//        return serializer.Serialize(scenario);
//    }

//    public Option<List<Scenario>> DeserialiseScenarios(string text, Context context)
//    {
//        var deserializer = new DeserializerBuilder()
//            .WithNamingConvention(UnderscoredNamingConvention.Instance)
//            .Build();

//        try
//        {
//            return deserializer.Deserialize<List<Scenario>>(text);
//        }
//        catch (Exception e)
//        {
//            return Option<List<Scenario>>.None;
//        }
//    }

//    public string SerialiseSystem(System system)
//    {
//        var serializer =
//            new SerializerBuilder()
//                .WithNamingConvention(CamelCaseNamingConvention.Instance)
//                .Build();

//        return serializer.Serialize(system);
//    }

//    public Option<System> DeserialiseSystem(string text, Context context)
//    {
//        var deserializer = new DeserializerBuilder()
//            .WithNamingConvention(UnderscoredNamingConvention.Instance)
//            .Build();

//        try
//        {
//            return deserializer.Deserialize<System>(text);
//        }
//        catch (Exception e)
//        {
//            return Option<System>.None;
//        }
//    }
//}
