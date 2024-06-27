using cvi_backend.Models;
using cvi_backend.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace cvi_backend.Events;

public class ElasticsearchIndexEventHandler(ElasticsearchIndexService elasticsearchService, ILogger<ElasticsearchIndexEventHandler> logger) : INotificationAsyncHandler<ContentPublishingNotification>
{
    private readonly ElasticsearchIndexService _elasticsearchService = elasticsearchService;
    private readonly ILogger<ElasticsearchIndexEventHandler> _logger = logger;

    public async Task HandleAsync(ContentPublishingNotification notification, CancellationToken cancellationToken)
    {
        foreach (var content in notification.PublishedEntities.Where(pe => pe.ContentType.Name.ToLower() == "icon"))
        {
            await _elasticsearchService.EnsureIndexExistsAsync("icons");
            await IndexContentInElasticsearchAsync(content, cancellationToken);
        }
    }

    private async Task IndexContentInElasticsearchAsync(IContent content, CancellationToken cancellationToken)
    {
        var client = _elasticsearchService.Client;

        var iconData = new Icon { Id = content.Id, Name = content.Name, Svg = content.GetValue<string>("svg"), Category = GetCategoryValue(content.GetValue<string>("category")) };

        var response = await client.IndexAsync(iconData, i => i.Index("icons"), cancellationToken);

        if (!response.IsValidResponse)
        {
            _logger.LogError("Failed to index content in Elasticsearch: {0}", response.DebugInformation);
        }
    }

    private string GetCategoryValue(string categoryJson)
    {
        if (string.IsNullOrEmpty(categoryJson))
        {
            return null;
        }

        // Parse the JSON string to an array
        var categoryArray = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(categoryJson);

        // Assuming it's a single-select dropdown, return the first value
        // If it's a multi-select dropdown, you can adjust this accordingly
        return categoryArray?.FirstOrDefault();
    }
}
