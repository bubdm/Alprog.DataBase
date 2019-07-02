using System;
using System.IO;
using Alprog.DataBase.DataProviders;
using Alprog.DataBase;
namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            JsonDataProvider provider = new JsonDataProvider(new JsonDataContext(new FileInfo("data.json")));
            provider.Data.Clear();
            provider.Data.Add("Bartolomey George");
            provider.Data.Remove("Bartolomey George");
            provider.Data.Add("Guga");
            int a = provider.Data.Count;
            provider.Data.ForEach(x => Console.WriteLine(x));
            Console.WriteLine(provider.Data.Contains(x => x.ToString().Length > 3));
            Console.ReadKey();
        }
    }
}
