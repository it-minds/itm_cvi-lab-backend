using cvi_backend.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Notifications;

namespace cvi_backend.Events.Composers;

public class ElasticsearchIndexComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddTransient<ElasticsearchIndexService>();
        builder.AddNotificationAsyncHandler<ContentPublishingNotification, ElasticsearchIndexEventHandler>();
    }
}
