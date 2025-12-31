using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json.Linq;
using Shouldly;

namespace Nivaes.App.Secrets.UnitTest;

public class SerializerTest
{
    private readonly ITestOutputHelper _output;

    public SerializerTest(ITestOutputHelper output)
    {
        _output = output;
    }

    private byte[] Serializer(IEnumerable<string> codes)
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

    private IEnumerable<string> Deserialize(byte[] codes)
    {
        using var ms = new MemoryStream(codes);
        using var reader = new BinaryReader(ms);

        var numberCodes = reader.ReadInt32();

        while(numberCodes-- > 0)
        {
            var code = reader.ReadString();
            yield return code;
        }
    }

    public class SecretMocks
    {
        public required string Secret1 { get; set; }
        public required string Secret2 { get; set; }
    }

    [Fact]
    public void ReadSecret()
    {
        var aa = new Lazy<byte[]>(() => { return new byte[] { 0, 2, 3, 2, 3 }; });
        SecretMocks secrets1 = new SecretMocks()
        {
            Secret1 = "hola",
            Secret2 = "adios"
        };
        var codes = Serializer(new[] { secrets1.Secret1, secrets1.Secret2 });

        var codes2 = Deserialize(codes).ToArray();

        _output.WriteLine(string.Join(",", codes));

        SecretMocks secrets2 = new SecretMocks()
        {
            Secret1 = codes2[0],
            Secret2 = codes2[1],
        };

        secrets1.Secret1.ShouldBe(secrets2.Secret1);
        secrets1.Secret2.ShouldBe(secrets2.Secret2);
    }
}
