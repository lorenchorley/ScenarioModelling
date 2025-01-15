using GOLD;
using System.Reflection;

namespace ScenarioModelling.Parsers;

public class GoldEngineParserFactory
{
    public static Parser BuildParser(string embeddedResource)
    {
        //On crée le parser
        Parser parser = new();

        //Inutile de garder les réductions de type <A> ::= <B>
        parser.TrimReductions = true;

        //On charge les tables de la grammaire

        // On charge le fichier Embedded Ressource provenant de l'assembly (qui doit être commune entre GoldParserFactory et le fichier ressource)
        //using Stream? stream = typeof(GoldParserFactory).Assembly.GetCall.GetManifestResourceStream(ressource);

        // On charge le fichier Embedded Ressource provenant de l'assembly appellant cette méthode.
        using Stream? stream = Assembly.GetCallingAssembly().GetManifestResourceStream(embeddedResource);


        if (stream == null)
        {
            var assembly = Assembly.GetCallingAssembly();
            var list = assembly.GetManifestResourceNames();
            string bulletPointList = string.Join("\n", list.Select(f => $" -> {f}"));

            throw new Exception($"""
                The parser grammar file was not found: {embeddedResource}
                Here is the list of available resources in the target assembly '{assembly.FullName}'.
                If the file is not present in this list, make sure to set the 'Build Action' to 'Embedded resource'.
                {bulletPointList}
                """);
        }

        using BinaryReader reader = new BinaryReader(stream);

        parser.LoadTables(reader);

        return parser;
    }
}
