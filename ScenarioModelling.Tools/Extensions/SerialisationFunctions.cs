using ProtoBuf;
using System.IO.Compression;
using System.Net;

namespace ScenarioModelling.Tools.Extensions;

public static class SerialisationFunctions
{
    public static string ProtoBufSerializeAndCompress<T>(T obj)
    {
        using var serialisedStream = new MemoryStream();

        // Serialize with Protobuf
        Serializer.Serialize(serialisedStream, obj);

        // Compress with Gzip
        MemoryStream compressedStream = CompressStream(serialisedStream);

        // Convert to Base64 and URL Encode
        byte[] compressedArray = compressedStream.ToArray();
        string base64 = Convert.ToBase64String(compressedArray);
        var urlEncoded = WebUtility.UrlEncode(base64);
        return urlEncoded;
    }

    public static T ProtoBufDecompressAndDeserialize<T>(string urlEncoded)
    {
        // URL Decode and Convert from Base64
        string base64 = WebUtility.UrlDecode(urlEncoded);
        byte[] compressedArray = Convert.FromBase64String(base64);
        using var compressedStream = new MemoryStream(compressedArray);

        // Decompress
        using Stream serialisedStream = DecompressStream(compressedStream);

        // Deserialize with Protobuf
        return Serializer.Deserialize<T>(serialisedStream);
    }

    private static MemoryStream CompressStream(MemoryStream inputStream)
    {
        if (inputStream == null || inputStream.Length == 0)
        {
            throw new ArgumentException("Input stream cannot be null or empty.");
        }

        MemoryStream compressedStream = new MemoryStream();

        using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Compress, true))
        {
            inputStream.Position = 0; // Ensure we're at the beginning
            inputStream.CopyTo(gzipStream);
        }

        compressedStream.Position = 0; // Reset position for reading
        return compressedStream;
    }

    private static MemoryStream DecompressStream(MemoryStream compressedStream)
    {
        if (compressedStream == null || compressedStream.Length == 0)
        {
            throw new ArgumentException("Compressed stream cannot be null or empty.");
        }

        compressedStream.Position = 0;

        MemoryStream decompressedStream = new MemoryStream();
        using GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);

        gzipStream.CopyTo(decompressedStream);

        decompressedStream.Position = 0; // Reset position for reading
        return decompressedStream;
    }

    public static string ProtoBufSerializeToBase64<T>(T obj)
    {
        using var ms = new MemoryStream();
        Serializer.Serialize(ms, obj);
        byte[] bytes = ms.ToArray();
        string base64 = Convert.ToBase64String(bytes);
        return WebUtility.UrlEncode(base64);
    }

    public static T ProtoBufDeserializeFromBase64<T>(string encodedData)
    {
        string base64 = WebUtility.UrlDecode(encodedData);
        byte[] bytes = Convert.FromBase64String(base64);
        using var ms = new MemoryStream(bytes);
        return Serializer.Deserialize<T>(ms);
    }

}
