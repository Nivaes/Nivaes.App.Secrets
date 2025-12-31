using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Shouldly;

namespace Nivaes.App.Secrets.UnitTest;

internal class SerializerTest
{
    public byte[] Encode(string secret)
    {
        byte[] code = Encoding.UTF8.GetBytes(secret);
        return code;
    }

    public string Decode(byte[] code)
    {
        string value = Encoding.UTF8.GetString(code);
        return value;
    }


    private byte[] Serializer(params byte[][] codes)
    {
        //int totalLength = codes.Sum(x => Buffer.ByteLength(x)) + codes.Length * Buffer.ByteLength(new[] { 1 });
        int totalLength = codes.Sum(x => x.Length) + codes.Length * 4;
        //var result = new byte[totalLength];
        //var aa = Buffer.ByteLength(codes);
        var buffer = new byte[totalLength];

        using var ms = new MemoryStream(buffer);
        using var writer = new BinaryWriter(ms);
        writer.Write(codes.Length);
        //int offset = 0;
        foreach (var code in codes)
        {
            writer.Write(code.Length);
            writer.Write(code);
            //var size = Buffer.ByteLength(code);
            //var sizeArray = BitConverter.GetBytes(size);
            //Buffer.BlockCopy(code, 0, sizeArray, size, Buffer.ByteLength(new[] {1}));
            //offset += size;
            //Buffer.BlockCopy(code, 0, result, offset, Buffer.ByteLength(code));
            //offset += Buffer.ByteLength(code);
        }

        return ms.ToArray();
    }

    private IEnumerable<byte[]> Deserialize(byte[] codes)
    {
        using var ms = new MemoryStream(codes);
        using var reader = new BinaryReader(ms);

        var numberCodes = reader.ReadInt32();

        //for (int n = 0; n < size; n++)
        int n = 0;
        while(n++ < numberCodes)
        {
            var size = reader.ReadInt32();
            var code = reader.ReadBytes(size);
            yield return code;
        }
    }

    public class SecretMocks
    {
        byte[] val = { 1, 2, 4, 3 };
        public required string Secret1 { get; set; }
        public required string Secret2 { get; set; }
    }

    [Fact]
    public void ReadSecret()
    {
        SecretMocks secrets1 = new SecretMocks()
        {
            Secret1 = "hola",
            Secret2 = "adios"
        };
        var code1 = Encode(secrets1.Secret1);
        var code2 = Encode(secrets1.Secret2);
        var codes = Serializer(code1, code2);

        var codes2 = Deserialize(codes).ToArray();

        Console.WriteLine(string.Join(",", codes));

        SecretMocks secrets2 = new SecretMocks()
        {
            Secret1 = Decode(codes2[0]),
            Secret2 = Decode(codes2[1]),
        };

        secrets1.Secret1.ShouldBe(secrets2.Secret1);
        secrets1.Secret2.ShouldBe(secrets2.Secret2);
    }
}
