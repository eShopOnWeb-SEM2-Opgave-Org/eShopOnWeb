using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.Web.Http;

namespace Microsoft.eShopWeb.Web.Configuration;

public static class HttpConfiguration
{
    public static IServiceCollection AddCatalogGatewayServices(this IServiceCollection @this, string baseUrl)
    {
        @this.AddHttpClient(
            CatalogItemHttpClient.CLIENT_KEY,
            config => config.BaseAddress = new (baseUrl)
        );
        @this.AddScoped<IRepository<CatalogItem>, CatalogItemHttpClient>();

        return @this;
    }
}
