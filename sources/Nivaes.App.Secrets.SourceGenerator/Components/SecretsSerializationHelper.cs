using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Nivaes.App.Secrets.SourceGenerator;

internal static class SecretsSerializationHelper
{
    internal static byte[] Serializer(IEnumerable<string> codes)
    {
        int totalLength = codes.Sum(x => x.Length) + codes.Count() * 4;
        var buffer = new byte[totalLength];

        using var ms = new MemoryStream(buffer);
        using var writer = new BinaryWriter(ms);
        writer.Write(codes.Count());
        foreach (var code in codes)
        {
            writer.Write(code);
        }

        return ms.ToArray();
    }
}
