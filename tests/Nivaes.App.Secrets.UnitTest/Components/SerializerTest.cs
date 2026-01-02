using FsCheck.Xunit;
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
        SecretMocks secrets1 = new SecretMocks()
        {
            Secret1 = "hola",
            Secret2 = "adios",
            Secret3 = "saludos"
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
        SecretMocks secrets1 = new SecretMocks()
        {
            Secret1 = "hola",
            Secret2 = "adios",
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

    [Property(MaxTest = 500)]
    public void Serialize_secret_fscheck(SecretMocks secrets)
    {
        var codes = SecretsSerializationHelper.Serializer(new[] { secrets.Secret1, secrets.Secret2, secrets.Secret3 });

        var codes2 = SecretsSerializer.Deserialize(codes).ToArray();

        _output.WriteLine(string.Join(",", codes));

        SecretMocks secretsCopy = new SecretMocks()
        {
            Secret1 = codes2[0],
            Secret2 = codes2[1],
            Secret3 = codes2[2],
        };

        secretsCopy.Secret1.ShouldBe(secrets.Secret1);
        secretsCopy.Secret2.ShouldBe(secrets.Secret2);
        secretsCopy.Secret3.ShouldBe(secrets.Secret3);
    }
}
