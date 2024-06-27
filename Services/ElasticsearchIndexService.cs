using cvi_backend.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Transport;

namespace cvi_backend.Services;

public class ElasticsearchIndexService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchIndexService> _logger;

    public ElasticsearchIndexService(ILogger<ElasticsearchIndexService> logger)
    {
        var settings = new ElasticsearchClientSettings(new Uri("https://localhost:9200/"))
            .ServerCertificateValidationCallback(CertificateValidationCallback) // TODO: Only for development purposes!
            .Authentication(new BasicAuthentication("elastic", "bKIWAQB=_BjF_AyPvgZi"))
            .DefaultIndex("icons")
            .ThrowExceptions(alwaysThrow: true)
            .PrettyJson()
            .RequestTimeout(TimeSpan.FromMilliseconds(60000));

        _client = new ElasticsearchClient(settings);

        _logger = logger;
    }

    public async Task EnsureIndexExistsAsync(string indexName)
    {
        var existsResponse = await _client.Indices.ExistsAsync(indexName);

        if (!existsResponse.Exists)
        {
            var createIndexResponse = await _client.Indices.CreateAsync(indexName, c => c
                .Mappings(m => m
                    .Dynamic(DynamicMapping.True)
                    .Properties<Icon>(p => p
                        .LongNumber(p => p.Id)
                        .Text(p => p.Name)
                        .Text(p => p.Svg)
                        .Keyword(p => p.Category)
                    )
                )
            );

            if (!createIndexResponse.IsValidResponse)
            {
                throw new Exception($"Failed to create index: {createIndexResponse.DebugInformation}");
            }
        }
    }

    private static bool CertificateValidationCallback(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    {
        return true; // Ignore SSL certificate validation errors
    }

    public ElasticsearchClient Client => _client;
}

