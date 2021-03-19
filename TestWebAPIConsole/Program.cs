using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TestWebAPIConsole
{

    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            Task<string> result = client.GetStringAsync("https://localhost:44301/api/companies"); 
            string stringResult = result.Result;
            Console.WriteLine(stringResult);
            Console.ReadKey();
        }
    }
}
