using Nivaes.App.Secrets.SourceGenerator;
using Nivaes.DataTestGenerator.Xunit;
using Shouldly;

namespace Nivaes.App.Secrets.UnitTest;

public class SerializerTest
{
    private readonly ITestOutputHelper _output;

    public SerializerTest(ITestOutputHelper output)
    {
        _output = output;
    }

    public class SecretMocks
    {
        public required string Secret1 { get; set; }
        public required string Secret2 { get; set; }
        public required string Secret3 { get; set; }
    }


    [Fact]
    public void Serializa_secris()
    {
        var aa = new Lazy<byte[]>(() => { return new byte[] { 0, 2, 3, 2, 3 }; });
        SecretMocks secrets1 = new SecretMocks()
        {
            Secret1 = "http://localhost",
            Secret2 = "http://localhost:8925",
            Secret3 = "http://name@localhost?ps=123"
        };
        var codes = SecretsSerializationHelper.Serializer(new[] { secrets1.Secret1, secrets1.Secret2, secrets1.Secret3 });

        var codes2 = SecretsSerializer.Deserialize(codes).ToArray();

        _output.WriteLine(string.Join(",", codes));

        SecretMocks secrets2 = new SecretMocks()
        {
            Secret1 = codes2[0],
            Secret2 = codes2[1],
            Secret3 = codes2[2],
        };

        secrets1.Secret1.ShouldBe(secrets2.Secret1);
        secrets1.Secret2.ShouldBe(secrets2.Secret2);
        secrets1.Secret3.ShouldBe(secrets2.Secret3);
    }

    [Theory]
    [GenerateStringInlineData(50, MinSize = 50, MaxSize = 1000)]
    public void Serialize_secret_multi(string secret)
    {
        var aa = new Lazy<byte[]>(() => { return new byte[] { 0, 2, 3, 2, 3 }; });
        SecretMocks secrets1 = new SecretMocks()
        {
            Secret1 = "Holow",
            Secret2 = "By",
            Secret3 = secret
        };
        var codes = SecretsSerializationHelper.Serializer(new[] { secrets1.Secret1, secrets1.Secret2, secrets1.Secret3 });

        var codes2 = SecretsSerializer.Deserialize(codes).ToArray();

        _output.WriteLine(string.Join(",", codes));

        SecretMocks secrets2 = new SecretMocks()
        {
            Secret1 = codes2[0],
            Secret2 = codes2[1],
            Secret3 = codes2[2],
        };

        secrets1.Secret1.ShouldBe(secrets2.Secret1);
        secrets1.Secret2.ShouldBe(secrets2.Secret2);
        secrets1.Secret3.ShouldBe(secrets2.Secret3);
        secrets1.Secret3.ShouldBe(secret);
    }
}
