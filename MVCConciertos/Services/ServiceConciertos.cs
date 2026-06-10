using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
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

        // --- MÉTODO CORREGIDO PARA CONSULTAR A LA LAMBDA IA ---
        public async Task<string?> PreguntarIAAsync(string pregunta)
        {
            using (HttpClient client = new HttpClient())
            {
                string urlPreguntar = "https://g4etttxt75.execute-api.us-east-1.amazonaws.com/preguntar";
                
                // Forzamos el JSON de forma manual para evitar que PostAsJsonAsync ponga la "P" en minúscula
                string jsonPayload = JsonSerializer.Serialize(new { Pregunta = pregunta });
                StringContent content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(urlPreguntar, content);

                string jsonString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        using (var doc = JsonDocument.Parse(jsonString))
                        {
                            if (doc.RootElement.TryGetProperty("respuesta", out JsonElement respElement))
                            {
                                return respElement.GetString();
                            }
                            return "Error: La API no devolvió la propiedad 'respuesta'.";
                        }
                    }
                    catch
                    {
                        // Si falla el parseo, pero es un 200, devuelve el raw por seguridad
                        return jsonString;
                    }
                }
                else
                {
                    // De esta forma si algo sale mal verás el mensaje de la IA o de AWS en tu pantalla.
                    return $"Error de la IA ({response.StatusCode}): {jsonString}";
                }
            }
        }
    }
}
