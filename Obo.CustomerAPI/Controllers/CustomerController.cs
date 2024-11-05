using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web.Resource;
using Obo.Objects;
using System.Net.Http.Headers;

namespace Obo.CustomerAPI.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfidentialClientApplication confidentialClientApplication;
        private readonly IConfiguration configuration;
        public CustomerController(IConfiguration configuration)
        {
            this.configuration = configuration;

            var builder = ConfidentialClientApplicationBuilder.CreateWithApplicationOptions(new ConfidentialClientApplicationOptions
            {
                ClientId = configuration.GetValue<string>("EntraId:ClientId"),
                ClientSecret = configuration.GetValue<string>("EntraId:ClientSecret"),
                TenantId = configuration.GetValue<string>("EntraId:TenantId")
            });

            confidentialClientApplication = builder.Build();
        }

        [HttpGet("all")]
        [Authorize(Policy = "AccessAsUser")]
        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            var orders = await CallOrderApi();

            if (orders.Any())
            {
                return
                [    new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Orders= orders.Where(v=>v.CustomerId == 1) },
                     new Customer { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Orders= orders.Where(v=>v.CustomerId == 2)},
                     new Customer { Id = 3, FirstName = "Emily", LastName = "Joe", Email = "emily.Joe@example.com", Orders= orders.Where(v=>v.CustomerId == 3) }
                ];
            }
            return null;
        }

        private async Task<string> GetAccessToken()
        {
            var assertion = string.Empty;
            var audience = configuration.GetValue<string>("OrderApi:Audience");
            var scope = configuration.GetValue<string>("OrderApi:Scope");

            string[] scopes = [$"api://{audience}/{scope}"];

            var inboundToken = HttpContext?.Request?.Headers?.Authorization.FirstOrDefault();

            if (inboundToken != null && inboundToken.StartsWith("Bearer "))
            {
                assertion = inboundToken[7..];
            }
            var builder = confidentialClientApplication.AcquireTokenOnBehalfOf(scopes, new UserAssertion(assertion));

            var authResult = await builder.ExecuteAsync();

            var accessToken = authResult.AccessToken;
            return accessToken;
        }

        private async Task<IEnumerable<Order>> CallOrderApi()
        {
            var accessToken = await GetAccessToken();

            using var httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7153/api/orders/") };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var orders = await httpClient.GetFromJsonAsync<IEnumerable<Order>>("all");
            return orders;
        }
    }
}
