using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Nivaes.App.Secrets.SourceGenerator;

internal static class SecretsSerializationHelper
{
    internal static byte[] Serializer(IEnumerable<string> codes)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        writer.Write(codes.Count());
        foreach (var code in codes)
        {
            writer.Write(code);
        }

        return ms.ToArray();
    }
}
