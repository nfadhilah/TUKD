using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using TUKD.Web;
using TUKD.Web.Services;
using TUKD.Web.ViewModels;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices(opt =>
{
    opt.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
});
builder.Services.AddSingleton<ILoadingService, LoadingService>();
builder.Services.AddScoped<WeatherForecastViewModel>();
await builder.Build().RunAsync();
