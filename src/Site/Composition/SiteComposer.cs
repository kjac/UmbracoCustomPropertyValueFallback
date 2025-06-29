using Site.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Site.Composition;

public class SiteComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // replace core services
        builder.Services.AddUnique<IPublishedValueFallback, CustomPublishedValueFallback>();
        builder.Services.AddUnique<IApiPropertyRenderer, CustomApiPropertyRenderer>();

        // register own services
        builder.Services.AddSingleton<IPublishedPropertyFallbackHandler, PublishedPropertyFallbackHandler>();
    }
}