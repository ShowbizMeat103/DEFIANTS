using DEFIANTS.Client;
using DEFIANTS.Shared.Clients;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Refit;
using Blazored.LocalStorage; // <-- AÑADIDO

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. Registrar Blazored.LocalStorage para poder guardar/leer el token
builder.Services.AddBlazoredLocalStorage();

// 2. Registrar el handler que añadirá el token a las peticiones
builder.Services.AddTransient<AuthenticationHeaderHandler>();

// 3. Configurar Refit
builder.Services.AddRefitClient<IApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5031")) // URL de tu backend
    .AddHttpMessageHandler<AuthenticationHeaderHandler>(); // <-- ¡LA MAGIA! Adjunta el handler a Refit

await builder.Build().RunAsync();
