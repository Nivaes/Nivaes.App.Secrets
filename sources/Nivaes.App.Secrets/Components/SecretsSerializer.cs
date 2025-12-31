using System;
using System.Collections.Generic;
using System.Text;

namespace Nivaes.App.Secrets;

public static class SecretsSerializer
{
    public static IEnumerable<string> Deserialize(byte[] codes)
    {
        using var ms = new MemoryStream(codes);
        using var reader = new BinaryReader(ms);

        var numberCodes = reader.ReadInt32();

        while (numberCodes-- > 0)
        {
            var code = reader.ReadString();
            yield return code;
        }
    }

    public static string Read(Lazy<string[]> secres, int n) => secres.Value[n];
}
