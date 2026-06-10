using System.Net.Http.Headers;
using System.Net.Http.Json;
using MVCConciertos.Models; 

namespace MVCConciertos.Services
{
    public class ServiceConciertos
    {
        private MediaTypeWithQualityHeaderValue header;
        private string UrlApi;

        public ServiceConciertos(IConfiguration configuration)
        {
            string? configUrl = configuration.GetValue<string>("ApiUrls:ApiEventos");
            
            this.UrlApi = !string.IsNullOrEmpty(configUrl) 
                ? configUrl 
                : "https://2t952p4yac.execute-api.us-east-1.amazonaws.com/Prod/";
                
            this.header = new MediaTypeWithQualityHeaderValue("application/json");
        }

        private async Task<T?> CallApiAsync<T>(string request)
        {
            // Medida extra de seguridad
            if (string.IsNullOrEmpty(this.UrlApi))
            {
                throw new InvalidOperationException("La URL de la API no está configurada.");
            }

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);

                HttpResponseMessage response = await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T? data = await response.Content.ReadFromJsonAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<List<Evento>?> GetEventosAsync()
        {
            string request = "api/Eventos";
            return await this.CallApiAsync<List<Evento>>(request);
        }

        public async Task<List<CategoriaEvento>?> GetCategoriasAsync()
        {
            string request = "api/Eventos/Categorias";
            return await this.CallApiAsync<List<CategoriaEvento>>(request);
        }

        public async Task<List<Evento>?> GetEventosByCategoriaAsync(int idCategoria)
        {
            string request = $"api/Eventos/Categoria/{idCategoria}";
            return await this.CallApiAsync<List<Evento>>(request);
        }
    }
}
