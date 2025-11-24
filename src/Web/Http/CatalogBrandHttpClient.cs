using System.Net;
using System.Text.Json;
using Ardalis.Specification;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Web.Http;

//NOTE: we are only going to implement the used methods, due to time constraints.
//      The methods are:
//         - ListAsync
public class CatalogBrandHttpClient : IRepository<CatalogBrand>, IReadRepository<CatalogBrand>
{
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<CatalogBrandHttpClient> _logger;

    public CatalogBrandHttpClient(IHttpClientFactory factory, ILogger<CatalogBrandHttpClient> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public Task<CatalogBrand> AddAsync(CatalogBrand entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CatalogBrand>> AddRangeAsync(IEnumerable<CatalogBrand> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AnyAsync(ISpecification<CatalogBrand> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<CatalogBrand> AsAsyncEnumerable(ISpecification<CatalogBrand> specification)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(ISpecification<CatalogBrand> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(CatalogBrand entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteRangeAsync(IEnumerable<CatalogBrand> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CatalogBrand?> FirstOrDefaultAsync(ISpecification<CatalogBrand> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<CatalogBrand, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CatalogBrand?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        try
        {
            using HttpClient client = _factory.CreateClient(CatalogItemHttpClient.CLIENT_KEY);
            string url = client.BaseAddress +  $"/catalog-brands/{id.ToString()}";
            Guid correlationId = Guid.NewGuid();

            HttpRequestMessage request = new()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("correlation-id", correlationId.ToString());

            HttpResponseMessage response = await client.SendAsync(request, cancellationToken: cancellationToken);

            if (response.StatusCode is not (HttpStatusCode.OK or HttpStatusCode.NoContent))
            {
                string responseMessage = "No content in response";
                if (response.Content is HttpContent content)
                    responseMessage = await content.ReadAsStringAsync(cancellationToken);

                _logger.LogError(
                    "[Origin: {Origin}] Response did not indicate success. StatusCode = {StatusCode}, Message = {Message}, CorrelationId = {CorrelationId}",
                    nameof(CatalogBrandHttpClient) + nameof(GetByIdAsync),
                    response.StatusCode.ToString(),
                    responseMessage,
                    correlationId.ToString()
                );

                return null;
            }

            Brand body = await response.Content.ReadFromJsonAsync<Brand>(cancellationToken) ?? new (-1, "unkown");
            CatalogBrand brand = new CatalogBrand(body.Name);
            return brand;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "[Origin: {Origin}] Could not find element with id = {Id}",
                nameof(CatalogBrandHttpClient) + nameof(GetByIdAsync),
                id
            );

            throw e;
        }
    }

    public Task<CatalogBrand?> GetBySpecAsync(ISpecification<CatalogBrand> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> GetBySpecAsync<TResult>(ISpecification<CatalogBrand, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private sealed record Brand(int Id, string Name);
    private sealed record ListBrandResponse(IEnumerable<Brand> CatalogBrands);
    public async Task<List<CatalogBrand>> ListAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using HttpClient client = _factory.CreateClient(CatalogItemHttpClient.CLIENT_KEY);
            string url = client.BaseAddress + "catalog-brands";
            Guid correlationId = Guid.NewGuid();

            HttpRequestMessage request = new()
            {
                RequestUri = new (url),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("correlation-id", correlationId.ToString());

            HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
            if (response.StatusCode is not (HttpStatusCode.OK or HttpStatusCode.NoContent))
            {
                var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogError(
                    "[Origin: {Origin}] Response from Catalog API did not indicate success, Status = {Status}, Message = {Message}, CorrelationId = {CorrelationId}",
                    nameof(CatalogBrandHttpClient) + nameof(ListAsync),
                    response.StatusCode.ToString(),
                    errorMessage,
                    correlationId.ToString()
                );

                return [];
            }

            ListBrandResponse listBrandResponse = await response.Content.ReadFromJsonAsync<ListBrandResponse>(cancellationToken) ?? new([]);
            List<CatalogBrand> items = listBrandResponse
                .CatalogBrands
                .Select(brand => new CatalogBrand(brand.Name))
                .ToList();

            return items;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "[Origin: {Origin}] Could not get brands from Catalog API due to internal error",
                nameof(CatalogBrandHttpClient) + nameof(ListAsync)
            );

            throw e;
        }
    }

    public Task<List<CatalogBrand>> ListAsync(ISpecification<CatalogBrand> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<TResult>> ListAsync<TResult>(ISpecification<CatalogBrand, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CatalogBrand?> SingleOrDefaultAsync(ISingleResultSpecification<CatalogBrand> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<CatalogBrand, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(CatalogBrand entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRangeAsync(IEnumerable<CatalogBrand> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

