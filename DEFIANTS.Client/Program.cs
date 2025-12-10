using DEFIANTS.Client;
using DEFIANTS.Shared.Clients;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization; // <-- AÑADIDO

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// --- CONFIGURACIÓN DE SERVICIOS DE AUTENTICACIÓN ---

// 1. Registrar Blazored.LocalStorage para poder guardar/leer el token
builder.Services.AddBlazoredLocalStorage();

// 2. Añadir los servicios de autorización de Blazor
builder.Services.AddAuthorizationCore();

// 3. Registrar nuestro proveedor de autenticación personalizado
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// ----------------------------------------------------


// 4. Registrar el handler que añadirá el token a las peticiones
builder.Services.AddTransient<AuthenticationHeaderHandler>();

// 5. Configurar Refit
builder.Services.AddRefitClient<IApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5031"))
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

await builder.Build().RunAsync();
