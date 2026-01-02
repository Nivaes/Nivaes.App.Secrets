namespace Test.NuGet.Secrets
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Test.NuGet.Secrets.Secrets.AppDns);
            Console.WriteLine(Test.NuGet.Secrets.Secrets.SentryDns);
        }
    }
}
