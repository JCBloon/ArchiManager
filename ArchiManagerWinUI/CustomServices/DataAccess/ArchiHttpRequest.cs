using ArchiManagerWinUI.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace ArchiManagerWinUI.CustomServices.DataAccess
{
    // AVISO: Todos los errores Http, como solo viajan en una red local y los códigos de error se gestionan, se tratarán como "Errores de conexión"
    public static class ArchiHttpRequest
    {
        private static readonly HttpClient _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:9212/")
        };

        // Constructor estático para poner el header en español
        static ArchiHttpRequest()
        {
            _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("es");
        }


        // CLIENT

        // POST CLIENT
        public static async Task<ArchiHttpResult<Client>> CreateClient(Client client)
        {
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(client),
                Encoding.UTF8,
                 "application/json"
                );

            try
            {
                using HttpResponseMessage response = await _httpClient.PostAsync("clients", jsonContent);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Client clientReponse = JsonSerializer.Deserialize<Client>(jsonResponse)!;
                    return ArchiHttpResult<Client>.Success(clientReponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    // Para no implementar la clase ErrorDto del backend
                    using var docs = JsonDocument.Parse(jsonResponse);
                    var fieldErrors = docs.RootElement.GetProperty("fieldErrors");
                    if (fieldErrors.ValueKind == JsonValueKind.Array)
                    {
                        var errores = new List<string>();
                        foreach (var error in fieldErrors.EnumerateArray())
                        {
                            var field = error.GetProperty("field").GetString();
                            var message = error.GetProperty("message").GetString();

                            errores.Add(field + " - " + message);
                        }
                        return ArchiHttpResult<Client>.Failure("Existen requisitos sin cumplir: " + string.Join("; ", errores));
                    }
                    return ArchiHttpResult<Client>.Failure("Requisito no cumplido: " + docs.RootElement.GetProperty("message").GetString());
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    return ArchiHttpResult<Client>.Failure("Ya existe un cliente con DNI " + client.Dni);
                }
                else
                {
                    throw new HttpRequestException("Error inesperado");
                }

            }
            catch (HttpRequestException ex)
            {
                return ArchiHttpResult<Client>.ConnectionError(ex.Message);
            }
        }

        // GET CLIENT
        public static async Task<ArchiHttpResult<Client>> FindClient(string? name, string? surname1, string? surname2, string? dni)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(name))
                query.Add("name", name);
            if (!string.IsNullOrEmpty(surname1))
                query.Add("surname1", surname1);
            if (!string.IsNullOrEmpty(surname2))
                query.Add("surname2", surname2);
            if (!string.IsNullOrEmpty(dni))
                query.Add("dni", dni);

            string url = "clients/find";
            string queryString = query.ToString()!;
            if (!string.IsNullOrEmpty(queryString))
            {
                url += "?" + query;
            }

            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(url);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Client clientReponse = JsonSerializer.Deserialize<Client>(jsonResponse)!;
                    return ArchiHttpResult<Client>.Success(clientReponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ArchiHttpResult<Client>.Failure("Ningún cliente encontrado");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    return ArchiHttpResult<Client>.Failure("Más de 1 cliente encontrado");
                }
                else
                {
                    throw new HttpRequestException("Error inesperado");
                }

            }
            catch (HttpRequestException ex)
            {
                return ArchiHttpResult<Client>.ConnectionError(ex.Message);
            }
        }

        public async static Task<ArchiHttpResult<List<Client>>> SearchClient(string? name, string? surname1, string? surname2, string? dni, string? page, string? columnOrder, string? wayToOrder)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(name))
                query.Add("name", name);
            if (!string.IsNullOrEmpty(surname1))
                query.Add("surname1", surname1);
            if (!string.IsNullOrEmpty(surname2))
                query.Add("surname2", surname2);
            if (!string.IsNullOrEmpty(dni))
                query.Add("dni", dni);
            if (!string.IsNullOrEmpty(page))
                query.Add("page", page);
            if (!string.IsNullOrEmpty(columnOrder))
                query.Add("columnOrder", columnOrder);
            if (!string.IsNullOrEmpty(wayToOrder))
                query.Add("wayToOrder", wayToOrder);

            string url = "clients/search";
            string queryString = query.ToString()!;
            if (!string.IsNullOrEmpty(queryString))
            {
                url += "?" + query;
            }

            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(url);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    using var docs = JsonDocument.Parse(jsonResponse);
                    var clientsList = docs.RootElement.GetProperty("items");
                    bool hasNext = docs.RootElement.GetProperty("hasNext").GetBoolean();
                    List<Client> clientReponse = JsonSerializer.Deserialize<List<Client>>(clientsList)!;
                    return ArchiHttpResult<List<Client>>.Success(clientReponse, hasNext);
                }
                else
                {
                    throw new HttpRequestException("Error inesperado");
                }

            }
            catch (HttpRequestException ex)
            {
                return ArchiHttpResult<List<Client>>.ConnectionError(ex.Message);
            }
        }

        // PUT CLIENT
        public static async Task<ArchiHttpResult<Client>> UpdateClient(Client client)
        {
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(client),
                Encoding.UTF8,
                 "application/json"
                );

            try
            {
                if (client.Id == null)
                {
                    throw new HttpRequestException("El cliente no tiene id");
                }
                string url = "clients/" + client.Id.ToString();

                using HttpResponseMessage response = await _httpClient.PutAsync(url, jsonContent);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Client clientReponse = JsonSerializer.Deserialize<Client>(jsonResponse)!;
                    return ArchiHttpResult<Client>.Success(clientReponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    using var docs = JsonDocument.Parse(jsonResponse);
                    var fieldErrors = docs.RootElement.GetProperty("fieldErrors");
                    if (fieldErrors.ValueKind == JsonValueKind.Array)
                    {
                        var errores = new List<string>();
                        foreach (var error in fieldErrors.EnumerateArray())
                        {
                            var field = error.GetProperty("field").GetString();
                            var message = error.GetProperty("message").GetString();

                            errores.Add(field + " - " + message);
                        }
                        return ArchiHttpResult<Client>.Failure("Existen requisitos sin cumplir: " + string.Join("; ", errores));
                    }
                    return ArchiHttpResult<Client>.Failure("Requisito no cumplido: " + docs.RootElement.GetProperty("message").GetString());
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    return ArchiHttpResult<Client>.Failure("Ya existe un cliente con DNI " + client.Dni);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ArchiHttpResult<Client>.Failure("El cliente que se intenta actualziar no existe");
                }
                else
                {
                    throw new HttpRequestException("Error inesperado");
                }

            }
            catch (HttpRequestException ex)
            {
                return ArchiHttpResult<Client>.ConnectionError(ex.Message);
            }
        }

        // DELETE CLIENT
        public static async Task<ArchiHttpResult<object>> DeleteClient(long id)
        {
            try
            {
                string url = "clients/" + id.ToString();
                using HttpResponseMessage response = await _httpClient.DeleteAsync(url);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return ArchiHttpResult<object>.Success();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ArchiHttpResult<object>.Failure("El cliente no existe");
                }
                else
                {
                    throw new HttpRequestException("Error inesperado");
                }

            }
            catch (HttpRequestException ex)
            {
                return ArchiHttpResult<object>.ConnectionError(ex.Message);
            }
        }

        // PROJECT

        // POST PROJECT
        public static async Task<ArchiHttpResult<Project>> CreateProject(Project project, long clientId)
        {
            var createProjectDto = new CreateProjectDto(project, clientId);

            using StringContent jsonContent = new(
                JsonSerializer.Serialize(createProjectDto),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                using HttpResponseMessage response = await _httpClient.PostAsync("projects", jsonContent);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Project projectResponse = JsonSerializer.Deserialize<Project>(jsonResponse)!;
                    return ArchiHttpResult<Project>.Success(projectResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    // Para no implementar la clase ErrorDto del backend
                    using var docs = JsonDocument.Parse(jsonResponse);
                    var fieldErrors = docs.RootElement.GetProperty("fieldErrors");
                    if (fieldErrors.ValueKind == JsonValueKind.Array)
                    {
                        var errores = new List<string>();
                        foreach (var error in fieldErrors.EnumerateArray())
                        {
                            var field = error.GetProperty("field").GetString();
                            var message = error.GetProperty("message").GetString();

                            errores.Add(field + " - " + message);
                        }
                        return ArchiHttpResult<Project>.Failure("Existen requisitos sin cumplir: " + string.Join("; ", errores));
                    }
                    return ArchiHttpResult<Project>.Failure("Requisito no cumplido: " + docs.RootElement.GetProperty("message").GetString());
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    return ArchiHttpResult<Project>.Failure("Ya existe un proyecto con el Nº de expediente " + project.ExpedientNumber);
                }
                else
                {
                    throw new HttpRequestException("Error inesperado");
                }

            }
            catch (HttpRequestException ex)
            {
                return ArchiHttpResult<Project>.ConnectionError(ex.Message);
            }
        }

        // GET PROJECT
        public static async Task<ArchiHttpResult<Project>> FindProject(string? title, string? expedientNumber, short? year, string? cadastralReference)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(title))
                query.Add("title", title);
            if (!string.IsNullOrEmpty(expedientNumber))
                query.Add("expedientNumber", expedientNumber);
            if (year.HasValue)
                query.Add("year", year.Value.ToString());
            if (!string.IsNullOrEmpty(cadastralReference))
                query.Add("cadastralReference", cadastralReference);

            string url = "projects/find";
            string queryString = query.ToString()!;
            if (!string.IsNullOrEmpty(queryString))
            {
                url += "?" + query;
            }

            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(url);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Project projectReponse = JsonSerializer.Deserialize<Project>(jsonResponse)!;
                    return ArchiHttpResult<Project>.Success(projectReponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ArchiHttpResult<Project>.Failure("Ningún proyecto encontrado");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    return ArchiHttpResult<Project>.Failure("Más de 1 proyecto encontrado");
                }
                else
                {
                    throw new HttpRequestException("Error inesperado");
                }

            }
            catch (HttpRequestException ex)
            {
                return ArchiHttpResult<Project>.ConnectionError(ex.Message);
            }
        }

        public async static Task<ArchiHttpResult<List<Project>>> SearchProject(string? title, string? expedientNumber, short? year, string? cadastralReference, string? page, string? columnOrder, string? wayToOrder)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(title))
                query.Add("title", title);
            if (!string.IsNullOrEmpty(expedientNumber))
                query.Add("expedientNumber", expedientNumber);
            if (year.HasValue)
                query.Add("year", year.Value.ToString());
            if (!string.IsNullOrEmpty(cadastralReference))
                query.Add("cadastralReference", cadastralReference);
            if (!string.IsNullOrEmpty(page))
                query.Add("page", page);
            if (!string.IsNullOrEmpty(columnOrder))
                query.Add("columnOrder", columnOrder);
            if (!string.IsNullOrEmpty(wayToOrder))
                query.Add("wayToOrder", wayToOrder);

            string url = "projects/search";
            string queryString = query.ToString()!;
            if (!string.IsNullOrEmpty(queryString))
            {
                url += "?" + query;
            }

            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(url);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    using var docs = JsonDocument.Parse(jsonResponse);
                    var projectsList = docs.RootElement.GetProperty("items");
                    bool hasNext = docs.RootElement.GetProperty("hasNext").GetBoolean();
                    List<Project> projectReponse = JsonSerializer.Deserialize<List<Project>>(projectsList)!;
                    return ArchiHttpResult<List<Project>>.Success(projectReponse, hasNext);
                }
                else
                {
                    throw new HttpRequestException("Error inesperado");
                }

            }
            catch (HttpRequestException ex)
            {
                return ArchiHttpResult<List<Project>>.ConnectionError(ex.Message);
            }
        }

        public static async Task<ArchiHttpResult<byte[]>> GetImage(long projectId)
        {

            string url = "projects/image/" + projectId.ToString();

            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    byte[] projectImageBytes = await response.Content.ReadAsByteArrayAsync();
                    return ArchiHttpResult<byte[]>.Success(projectImageBytes);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    using var docs = JsonDocument.Parse(jsonResponse);
                    var docMessage = docs.RootElement.GetProperty("message");
                    return ArchiHttpResult<byte[]>.Failure(docMessage.ToString());
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ArchiHttpResult<byte[]>.Failure("Proyecto no encontrado");
                }
                else
                {
                    throw new HttpRequestException("Error inesperado");
                }
            }
            catch (HttpRequestException ex)
            {
                return ArchiHttpResult<byte[]>.ConnectionError(ex.Message);
            }
        }

        // PUT IMAGE
        public static async Task<ArchiHttpResult<object>> SetImage(long projectId, FileData? file)
        {
            try
            {
                string url = "projects/image/" + projectId.ToString();
                HttpResponseMessage response;

                if (file != null)
                {
                    using var form = new MultipartFormDataContent();
                    var streamContent = new StreamContent(new MemoryStream(file.Content));
                    form.Add(streamContent, "imageFile", file.Name);
                    response = await _httpClient.PutAsync(url, form);
                } else
                {
                    response = await _httpClient.PutAsync(url, null);
                }
                
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return ArchiHttpResult<object>.Success();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ArchiHttpResult<object>.Failure("El proyecto no existe");
                }
                else
                {
                    throw new HttpRequestException("Error inesperado");
                }

            }
            catch (HttpRequestException ex)
            {
                return ArchiHttpResult<object>.ConnectionError(ex.Message);
            }
        }

        // PUT PROJECT
        public static async Task<ArchiHttpResult<Project>> UpdateProject(Project project)
        {
            using StringContent jsonContent = new(
                JsonSerializer.Serialize(project),
                Encoding.UTF8,
                 "application/json"
            );

            try
            {
                if (project.Id == null)
                {
                    throw new HttpRequestException("El proyecto no tiene id");
                }
                string url = "projects/" + project.Id.ToString();

                using HttpResponseMessage response = await _httpClient.PutAsync(url, jsonContent);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Project projectResponse = JsonSerializer.Deserialize<Project>(jsonResponse)!;
                    return ArchiHttpResult<Project>.Success(projectResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    using var docs = JsonDocument.Parse(jsonResponse);
                    var fieldErrors = docs.RootElement.GetProperty("fieldErrors");
                    if (fieldErrors.ValueKind == JsonValueKind.Array)
                    {
                        var errores = new List<string>();
                        foreach (var error in fieldErrors.EnumerateArray())
                        {
                            var field = error.GetProperty("field").GetString();
                            var message = error.GetProperty("message").GetString();

                            errores.Add(field + " - " + message);
                        }
                        return ArchiHttpResult<Project>.Failure("Existen requisitos sin cumplir: " + string.Join("; ", errores));
                    }
                    return ArchiHttpResult<Project>.Failure("Requisito no cumplido: " + docs.RootElement.GetProperty("message").GetString());
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    return ArchiHttpResult<Project>.Failure("Ya existe un proyecto con Numero de Expediente " + project.ExpedientNumber);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ArchiHttpResult<Project>.Failure("El proyecto que se intenta actualizar no existe");
                }
                else
                {
                    throw new HttpRequestException("Error inesperado");
                }

            }
            catch (HttpRequestException ex)
            {
                return ArchiHttpResult<Project>.ConnectionError(ex.Message);
            }
        }

        // DELETE PROJECT
        public static async Task<ArchiHttpResult<object>> DeleteProject(long id)
        {
            try
            {
                string url = "projects/" + id.ToString();
                using HttpResponseMessage response = await _httpClient.DeleteAsync(url);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return ArchiHttpResult<object>.Success();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return ArchiHttpResult<object>.Failure("El proyecto no existe");
                }
                else
                {
                    throw new HttpRequestException("Error inesperado");
                }

            }
            catch (HttpRequestException ex)
            {
                return ArchiHttpResult<object>.ConnectionError(ex.Message);
            }
        }

        // ASSIGN PROJECT
        public static async Task<ArchiHttpResult<Project>> AssignClient(long projectId, long clientId)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("projectId", projectId.ToString());
            query.Add("clientId", clientId.ToString());

            string url = "projects/assign";
            string queryString = query.ToString()!;
            if (!string.IsNullOrEmpty(queryString))
            {
                url += "?" + query;
            }
            Console.WriteLine(url);
            try
            {
                using HttpResponseMessage response = await _httpClient.PutAsync(url, null);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Project projectReponse = JsonSerializer.Deserialize<Project>(jsonResponse)!;
                    return ArchiHttpResult<Project>.Success(projectReponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    using var docs = JsonDocument.Parse(jsonResponse);
                    var docMessage = docs.RootElement.GetProperty("message");
                    return ArchiHttpResult<Project>.Failure(docMessage.ToString());
                }
                else
                {
                    throw new HttpRequestException("Error inesperado");
                }
            }
            catch (HttpRequestException ex)
            {
                return ArchiHttpResult<Project>.ConnectionError(ex.Message);
            }
        }

        // UNASSIGN PROJECT
        public static async Task<ArchiHttpResult<object>> UnassignClient(long projectId, long clientId)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("projectId", projectId.ToString());
            query.Add("clientId", clientId.ToString());

            string url = "projects/unassign";
            string queryString = query.ToString()!;
            if (!string.IsNullOrEmpty(queryString))
            {
                url += "?" + query;
            }
            Console.WriteLine(url);
            try
            {
                using HttpResponseMessage response = await _httpClient.PutAsync(url, null);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (string.IsNullOrWhiteSpace(jsonResponse))
                        return ArchiHttpResult<object>.Success(null);

                    Project projectReponse = JsonSerializer.Deserialize<Project>(jsonResponse)!;
                    return ArchiHttpResult<object>.Success(projectReponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    using var docs = JsonDocument.Parse(jsonResponse);
                    var docMessage = docs.RootElement.GetProperty("message");
                    return ArchiHttpResult<object>.Failure(docMessage.ToString());
                }
                else
                {
                    throw new HttpRequestException("Error inesperado");
                }
            }
            catch (HttpRequestException ex)
            {
                return ArchiHttpResult<object>.ConnectionError(ex.Message);
            }
        }
    }
}
