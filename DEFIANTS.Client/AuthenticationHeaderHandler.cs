using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
// Idealmente, aquí usarías un servicio para obtener el token de forma segura
// desde el localStorage, pero para simplificar, lo leeremos directamente.
using Blazored.LocalStorage;

public class AuthenticationHeaderHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;

    public AuthenticationHeaderHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Obtener el token del localStorage
        var token = await _localStorage.GetItemAsync<string>("authToken");

        // Si el token existe, añadirlo a la cabecera de la petición
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
