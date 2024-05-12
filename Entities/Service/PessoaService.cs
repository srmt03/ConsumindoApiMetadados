using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;

namespace ConsumindoApiMetadados.Entities.Service
{
    class PessoaService
    {
        private const string API_URL = "https://apimetadadosback-dev.metadados.com.br";
        private const string email = "avaliacao.api@metadados.com.br";
        private const string senha = "Aval@123";

        private static async Task<string> GetToken(string email, string senha)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var loginData = new { email, senha };
                    var requestBody = new StringContent(System.Text.Json.JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync($"{API_URL}/login", requestBody);
                    response.EnsureSuccessStatusCode();

                    var tokenObject = await response.Content.ReadAsStringAsync();

                    var documentToken = JsonDocument.Parse(tokenObject);
                    var rootToken = documentToken.RootElement;
                    
                    string token = rootToken.GetProperty("accessToken").GetString();

                    return token;

                }                    
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Erro na requisição: {e.Message}");
                return null;
            }
        }

        public static async Task<dynamic> GetCadastroPreAdmissao(string _cpf)
        {
            string _token = await GetToken(email, senha);
            string _endpoint = "/cadastro/preAdmissao";
            string _url = API_URL + _endpoint;
            string _method = "GET";

            dynamic pessoa = await Requisicao(_url, _token, _method, _cpf);

            return pessoa;
        }

        public static async Task<dynamic> PostCadastroPreAdmissao(dynamic _bodyResponseObject)
        {
            string _token = await GetToken(email, senha);
            string _endpoint = "/cadastro/preAdmissao";
            string _url = API_URL + _endpoint;
            string _method = "POST";

            dynamic id = await Requisicao(_url, _token, _method, _bodyResponseObject);

            return id;
        }

        private static async Task<dynamic> Requisicao(string _url, string _token, string _method, string _queryParams = null, dynamic _pessoa = null)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string _requestUri;
                    string method = _method.ToString();
                    var _uriBuilder = new UriBuilder(_url);
                    var request = new HttpRequestMessage();
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                    StringContent _bodyContent;

                    if (_method == "GET")
                    {
                        var _query = System.Web.HttpUtility.ParseQueryString(_uriBuilder.Query);
                        _query["cpf"] = _queryParams;
                        _uriBuilder.Query = _query.ToString();
                        _requestUri = _uriBuilder.ToString();
                        request.Method = HttpMethod.Get;
                    }                                        
                    else //POST
                    {
                        string _json = JsonConvert.SerializeObject(new { pessoa = _pessoa });
                        _bodyContent = new StringContent(_json, Encoding.UTF8, "application/json");
                        request.Content = _bodyContent;
                        //request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        _requestUri = _url;
                        request.Method = HttpMethod.Post;
                    }

                    request = new HttpRequestMessage(request.Method, _requestUri);

                    var response = await client.SendAsync(request);
                   
                    if (!response.IsSuccessStatusCode)
                    {                    
                        Console.WriteLine($"Erro na requisição: {response.StatusCode}");
                        return null;
                    } 
                    else
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        dynamic jsonObejectReturn = JsonConvert.DeserializeObject<dynamic>(responseBody);
                        Console.WriteLine(jsonObejectReturn);
                        return jsonObejectReturn;
                    }                                                         
                }                    
            }
            catch (HttpRequestException e)
            {                
                Console.WriteLine($"Erro na requisição: {e.Message}");
                return null;
            }
        }

        //private static async Task<dynamic> PostRequisicaoComParametro(dynamic _pessoa, string _url, string _token)
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {                    
        //            string json = JsonConvert.SerializeObject(new { pessoa = _pessoa });
                                        
        //            var bodyContent = new StringContent(json, Encoding.UTF8, "application/json");

        //            var request = new HttpRequestMessage(HttpMethod.Post, $"{_url}");
        //            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        //            request.Content = bodyContent;
        //            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        //            var response = await client.SendAsync(request);

        //            if (!response.IsSuccessStatusCode)
        //            {
        //                Console.WriteLine($"Erro na requisição: {response.StatusCode}");
        //                return null;
        //            }
        //            else
        //            {
        //                var responseBody = await response.Content.ReadAsStringAsync();
        //                dynamic idPessoa = JsonConvert.DeserializeObject<dynamic>(responseBody);
        //                Console.WriteLine(idPessoa);
        //                return idPessoa;
        //            }
        //        }
        //    }
        //    catch (HttpRequestException e)
        //    {
        //        Console.WriteLine($"Erro na requisição: {e.Message}");
        //        return null;
        //    }
        //}
    }
}
