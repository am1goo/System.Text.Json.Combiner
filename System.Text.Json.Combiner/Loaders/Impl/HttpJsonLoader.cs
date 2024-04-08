using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace System.Text.Json.Combiner
{
    public class HttpJsonLoader : IJsonLoader
    {
        private static readonly AssemblyName _assemblyName = typeof(JsonCombiner).Assembly.GetName();

        public string Load(Uri uri, JsonLoaderContext ctx)
        {
            var client = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });
            var name = _assemblyName.Name;
            var version = _assemblyName.Version?.ToString();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(name, version));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var task = client.GetStringAsync(uri);
            task.Wait();
            if (task.IsFaulted)
                throw task.Exception;

            return task.Result;
        }
    }
}
