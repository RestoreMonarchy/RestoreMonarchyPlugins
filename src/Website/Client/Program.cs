using BlazorTable;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AKSoftware.Localization.MultiLanguages;
using Website.Client.Providers;
using Website.Client.Services;

namespace Website.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, SteamAuthProvider>();
            builder.Services.AddScoped<CartService>();
            builder.Services.AddTransient<StorageService>();
            builder.Services.AddTransient<ZIPService>();
            builder.Services.AddBlazorTable();
            builder.Services.AddLanguageContainer(Assembly.GetExecutingAssembly(), CultureInfo.GetCultureInfo("en_US"));

            await builder.Build().RunAsync();
        }
    }
}
