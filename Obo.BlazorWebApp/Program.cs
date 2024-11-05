using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Obo.BlazorWebApp.Components;

namespace Obo.BlazorWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(options =>
                {
                    options.ClientId = builder.Configuration.GetValue<string>("EntraId:ClientId");
                    options.ClientSecret = builder.Configuration.GetValue<string>("EntraId:ClientSecret");
                    options.TenantId = builder.Configuration.GetValue<string>("EntraId:TenantId");
                    options.ResponseType = builder.Configuration.GetValue<string>("EntraId:ResponseType");
                    options.Domain = builder.Configuration.GetValue<string>("EntraId:Domain");
                    options.Instance = builder.Configuration.GetValue<string>("EntraId:Instance");
                    options.CallbackPath = builder.Configuration.GetValue<string>("EntraId:CallbackPath");
                })
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddInMemoryTokenCaches();

            builder.Services.AddAuthorization();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddMicrosoftIdentityUI();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();


            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();


            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
