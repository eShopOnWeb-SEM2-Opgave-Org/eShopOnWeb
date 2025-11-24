using System.Net;
using Ardalis.Specification;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.Web.Http;

//NOTE: we are only going to implement the used methods, due to time constraints.
//      The methods are:
//         - ListAsync
//         - UpdateAsync
//         - CountAsync
//         - GetByIdAsync
public class CatalogItemHttpClient : IRepository<CatalogItem>, IReadRepository<CatalogItem>
{
    internal const string CLIENT_KEY = "catalog-item-client";

    private readonly IHttpClientFactory _factory;
    private readonly ILogger<CatalogItemHttpClient> _logger;

    public CatalogItemHttpClient(IHttpClientFactory factory, ILogger<CatalogItemHttpClient> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public Task<CatalogItem> AddAsync(CatalogItem entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CatalogItem>> AddRangeAsync(IEnumerable<CatalogItem> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AnyAsync(ISpecification<CatalogItem> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<CatalogItem> AsAsyncEnumerable(ISpecification<CatalogItem> specification)
    {
        throw new NotImplementedException();
    }

    private sealed record PagedItemResponse(IEnumerable<CatalogItem> CatalogItems, int PageCount);
    public async Task<int> CountAsync(ISpecification<CatalogItem> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            using HttpClient client = _factory.CreateClient(CLIENT_KEY);
            string url = client.BaseAddress + "catalog-items";
            Guid correlationId = Guid.NewGuid();

            HttpRequestMessage request = new()
            {
                RequestUri = new(url),
                Method = HttpMethod.Get
            };
            request.Headers.Add("correlation-id", correlationId.ToString());

            HttpResponseMessage response = await client.SendAsync(request, cancellationToken);
            if (response.StatusCode is not (HttpStatusCode.OK or HttpStatusCode.NoContent))
            {
                var errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogError(
                    "[Origin: {Origin}] Response from server did not indicate success. Status = {Status}, Message = {Message}, CorrelationId = {CorrelationId}",
                    nameof(CatalogItemHttpClient) + nameof(GetByIdAsync),
                    response.StatusCode.ToString(),
                    errorMessage,
                    correlationId.ToString()
                );

                throw new InvalidOperationException("Response from Catalot API did not indicate success");
            }

            PagedItemResponse? content = await response.Content.ReadFromJsonAsync<PagedItemResponse>();
            IEnumerable<CatalogItem> items = content?.CatalogItems ?? [];

            int itemCount = specification.Evaluate(items).Count();
            return itemCount;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Could not get item count from Catalog API due to internal error"
            );

            throw e;
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using HttpClient client = _factory.CreateClient(CLIENT_KEY);
            string url = client.BaseAddress + "catalog-items/conut";
            Guid correlationId = Guid.NewGuid();

            HttpRequestMessage request = new()
            {
                RequestUri = new(url),
                Method = HttpMethod.Get,
            };
            request.Headers.Add("correlation-id", correlationId.ToString());

            HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

            if (response.StatusCode is not HttpStatusCode.OK)
            {
                string errorMessage = "Response did not contain any content";
                if (response.Content is HttpContent httpContent)
                    errorMessage = await httpContent.ReadAsStringAsync(cancellationToken);

                _logger.LogError(
                    "Response status code from Catalog API url: api/catalog-items/count did not return a valid status code. Status = {Status}, Content = {Content}, CorrelationId = {CorrelationId}",
                    response.StatusCode.ToString(),
                    errorMessage,
                    correlationId.ToString()
                );

                throw new InvalidOperationException("Response from server did not indicate success");
            }

            string content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!int.TryParse(content, out int itemCount))
            {
                _logger.LogError(
                    "Response content from api/catalog-items/count did not contain a valid response. Content = {Content}, Correlation Id = {Id}",
                    content,
                    correlationId.ToString()
                );

                throw new InvalidOperationException("Could not deserialize response content from CatalogItem API");
            }

            return itemCount;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Could not fetch item count from api"
            );

            throw e;
        }
    }

    public Task DeleteAsync(CatalogItem entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteRangeAsync(IEnumerable<CatalogItem> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CatalogItem?> FirstOrDefaultAsync(ISpecification<CatalogItem> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<CatalogItem, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<CatalogItem?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        try
        {
            using HttpClient client = _factory.CreateClient(CLIENT_KEY);
            string url = client.BaseAddress +  $"/catalog-items/{id.ToString()}";
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
                    nameof(CatalogItemHttpClient) + nameof(GetByIdAsync),
                    response.StatusCode.ToString(),
                    responseMessage,
                    correlationId.ToString()
                );

                return null;
            }

            var body = await response.Content.ReadFromJsonAsync<CatalogItem>(cancellationToken);
            return body;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Could not find element with id = {Id}",
                id
            );

            throw e;
        }
    }

    public Task<CatalogItem?> GetBySpecAsync(ISpecification<CatalogItem> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> GetBySpecAsync<TResult>(ISpecification<CatalogItem, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<CatalogItem>> ListAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<CatalogItem>> ListAsync(ISpecification<CatalogItem> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            using HttpClient client = _factory.CreateClient(CLIENT_KEY);
            string url = client.BaseAddress + "catalog-items";
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
                    "Response from Catalog API did not indicate success, Status = {Status}, Message = {Message}, CorrelationId = {CorrelationId}",
                    response.StatusCode.ToString(),
                    errorMessage,
                    correlationId.ToString()
                );

                return [];
            }

            PagedItemResponse catalogPage = await response.Content.ReadFromJsonAsync<PagedItemResponse>(cancellationToken) ?? new([], 1);
            List<CatalogItem> items = specification
                .Evaluate(catalogPage.CatalogItems)
                .ToList();

            return items;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Could not get items from Catalog API due to internal error"
            );

            throw e;
        }
    }

    public Task<List<TResult>> ListAsync<TResult>(ISpecification<CatalogItem, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<CatalogItem?> SingleOrDefaultAsync(ISingleResultSpecification<CatalogItem> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<CatalogItem, TResult> specification, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(CatalogItem entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRangeAsync(IEnumerable<CatalogItem> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
