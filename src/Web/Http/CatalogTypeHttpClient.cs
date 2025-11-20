using System.Net;
using System.Text.Json;
using Ardalis.Specification;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Web.Http;

//NOTE: we are only going to implement the used methods, due to time constraints.
//      The methods are:
//         - ListAsync
public class CatalogTypeHttpClient : IRepository<CatalogType>, IReadRepository<CatalogType>
{
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<CatalogTypeHttpClient> _logger;

    public CatalogTypeHttpClient(IHttpClientFactory factory, ILogger<CatalogTypeHttpClient> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public Task<CatalogType> AddAsync(CatalogType entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CatalogType>> AddRangeAsync(IEnumerable<CatalogType> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AnyAsync(ISpecification<CatalogType> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<CatalogType> AsAsyncEnumerable(ISpecification<CatalogType> specification)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(ISpecification<CatalogType> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(CatalogType entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteRangeAsync(IEnumerable<CatalogType> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CatalogType?> FirstOrDefaultAsync(ISpecification<CatalogType> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<CatalogType, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CatalogType?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        try
        {
            using HttpClient client = _factory.CreateClient(CatalogItemHttpClient.CLIENT_KEY);
            string url = client.BaseAddress +  $"/catalog-types/{id.ToString()}";
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
                    nameof(CatalogTypeHttpClient) + nameof(GetByIdAsync),
                    response.StatusCode.ToString(),
                    responseMessage,
                    correlationId.ToString()
                );

                return null;
            }

            Type body = await response.Content.ReadFromJsonAsync<Type>(cancellationToken) ?? new (-1, "unkown");
            CatalogType type = new CatalogType(body.Name);

            return type;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "[Origin: {Origin}] Could not find element with id = {Id}",
                nameof(CatalogTypeHttpClient) + nameof(GetByIdAsync),
                id
            );

            throw e;
        }
    }

    public Task<CatalogType?> GetBySpecAsync(ISpecification<CatalogType> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> GetBySpecAsync<TResult>(ISpecification<CatalogType, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private sealed record Type(int Id, string Name);
    private sealed record ListTypeResponse(IEnumerable<Type> CatalogTypes);
    public async Task<List<CatalogType>> ListAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using HttpClient client = _factory.CreateClient(CatalogItemHttpClient.CLIENT_KEY);
            string url = client.BaseAddress + "catalog-types";
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
                    nameof(CatalogTypeHttpClient) + nameof(ListAsync),
                    response.StatusCode.ToString(),
                    errorMessage,
                    correlationId.ToString()
                );

                return [];
            }

            ListTypeResponse listTypeResponse = await response.Content.ReadFromJsonAsync<ListTypeResponse>(cancellationToken) ?? new([]);
            List<CatalogType> items = listTypeResponse
                .CatalogTypes
                .Select(type => new CatalogType(type.Name))
                .ToList();

            return items;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "[Origin: {Origin}] Could not get types from Catalog API due to internal error",
                nameof(CatalogTypeHttpClient) + nameof(ListAsync)
            );

            throw e;
        }
    }

    public Task<List<CatalogType>> ListAsync(ISpecification<CatalogType> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<TResult>> ListAsync<TResult>(ISpecification<CatalogType, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CatalogType?> SingleOrDefaultAsync(ISingleResultSpecification<CatalogType> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<CatalogType, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(CatalogType entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRangeAsync(IEnumerable<CatalogType> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}


