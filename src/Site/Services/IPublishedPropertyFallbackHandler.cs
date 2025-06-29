using Umbraco.Cms.Core.Models.PublishedContent;

namespace Site.Services;

public interface IPublishedPropertyFallbackHandler
{
    Task<string?> GetFallbackCultureAsync(IPublishedProperty property);
}