using System;
using System.Collections.Generic;
using System.Text;
using Shouldly;

namespace Nivaes.App.Secrets.UnitTest.Components;

public class ReadSecretsTest
{
    private readonly ITestOutputHelper _output;

    public ReadSecretsTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ReadSecret()
    {
        Nivaes.App.Secrets.UnitTest.Secrets.AppDns.ShouldNotBeNullOrWhiteSpace();
        Nivaes.App.Secrets.UnitTest.Secrets.SentryDns.ShouldNotBeNullOrWhiteSpace();
        _output.WriteLine(Nivaes.App.Secrets.UnitTest.Secrets.AppDns);
        _output.WriteLine(Nivaes.App.Secrets.UnitTest.Secrets.SentryDns);
    }
}
