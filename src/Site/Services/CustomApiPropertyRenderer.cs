using Umbraco.Cms.Core.DeliveryApi;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Site.Services;

public class CustomApiPropertyRenderer : IApiPropertyRenderer
{
    private readonly IPublishedPropertyFallbackHandler _fallbackHandler;

    public CustomApiPropertyRenderer(IPublishedPropertyFallbackHandler fallbackHandler)
        => _fallbackHandler = fallbackHandler;

    public object? GetPropertyValue(IPublishedProperty property, bool expanding)
    {
        // first try the default handling
        if (property.HasValue())
        {
            return property.GetDeliveryApiValue(expanding);
        }

        var fallbackCulture = _fallbackHandler.GetFallbackCultureAsync(property).GetAwaiter().GetResult();

        // attempt to fetch the property value for the fallback culture
        return fallbackCulture is not null
            ? property.GetDeliveryApiValue(expanding, culture: fallbackCulture)
            : null;
    }
}