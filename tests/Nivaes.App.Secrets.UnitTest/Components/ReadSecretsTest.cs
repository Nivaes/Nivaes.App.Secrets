using System;
using System.Collections.Generic;
using System.Text;
using Shouldly;

namespace Nivaes.App.Secrets.UnitTest.Components;

public class ReadSecretsTest
{
    [Fact]
    public void ReadSecret()
    {
        Nivaes.App.Secrets.UnitTest.Secrets.AppDns.ShouldNotBeNullOrWhiteSpace();
        Nivaes.App.Secrets.UnitTest.Secrets.SentryDns.ShouldNotBeNullOrWhiteSpace();
    }
}
