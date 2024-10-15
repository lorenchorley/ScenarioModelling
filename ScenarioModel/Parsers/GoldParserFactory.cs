using GOLD;
using System.Reflection;

namespace ScenarioModel.Parsers;

public class GoldEngineParserFactory
{
    public static Parser BuildParser(string embeddedRessource)
    {
        //On crée le parser
        Parser parser = new();

        //Inutile de garder les réductions de type <A> ::= <B>
        parser.TrimReductions = true;

        //On charge les tables de la grammaire

        // On charge le fichier Embedded Ressource provenant de l'assembly (qui doit être commune entre GoldParserFactory et le fichier ressource)
        //using Stream? stream = typeof(GoldParserFactory).Assembly.GetCall.GetManifestResourceStream(ressource);

        // On charge le fichier Embedded Ressource provenant de l'assembly appellant cette méthode.
        using Stream? stream = Assembly.GetCallingAssembly().GetManifestResourceStream(embeddedRessource);

        if (stream == null)
            throw new Exception("Le fichier grammaire du parser de filtre n'a pas été trouvé");

        using BinaryReader reader = new BinaryReader(stream);

        parser.LoadTables(reader);

        return parser;
    }
}
