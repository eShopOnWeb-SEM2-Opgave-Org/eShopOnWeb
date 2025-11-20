using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.Web.Http;

namespace Microsoft.eShopWeb.Web.Configuration;

public static class HttpConfiguration
{
    public static IServiceCollection AddCatalogHttpServices(this IServiceCollection @this, string baseUrl)
    {
        @this.AddHttpClient(
            CatalogItemHttpClient.CLIENT_KEY,
            config => config.BaseAddress = new (baseUrl)
        );
        @this.AddScoped<IReadRepository<CatalogItem>, CatalogItemHttpClient>();
        @this.AddScoped<IRepository<CatalogItem>, CatalogItemHttpClient>();

        @this.AddScoped<IReadRepository<CatalogBrand>, CatalogBrandHttpClient>();
        @this.AddScoped<IRepository<CatalogBrand>, CatalogBrandHttpClient>();

        @this.AddScoped<IReadRepository<CatalogType>, CatalogTypeHttpClient>();
        @this.AddScoped<IRepository<CatalogType>, CatalogTypeHttpClient>();

        return @this;
    }
}
