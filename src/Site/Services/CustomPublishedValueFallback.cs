using Umbraco.Cms.Core.Models.PublishedContent;

namespace Site.Services;

public class CustomPublishedValueFallback : IPublishedValueFallback
{
    private readonly IPublishedPropertyFallbackHandler _fallbackHandler;
    private readonly IPublishedValueFallback _noopFallback = new NoopPublishedValueFallback();

    public CustomPublishedValueFallback(IPublishedPropertyFallbackHandler fallbackHandler)
        => _fallbackHandler = fallbackHandler;

    public bool TryGetValue(IPublishedProperty property, string? culture, string? segment, Fallback fallback, object? defaultValue, out object? value)
        => TryGetValueWithCultureFallback(property, culture, out value);

    public bool TryGetValue<T>(IPublishedProperty property, string? culture, string? segment, Fallback fallback, T? defaultValue, out T? value)
        => TryGetValueWithCultureFallback(property, culture, out value);

    public bool TryGetValue(IPublishedElement content, string alias, string? culture, string? segment, Fallback fallback, object? defaultValue, out object? value)
        => TryGetValueWithCultureFallback(content, alias, culture, out value, out _);

    public bool TryGetValue<T>(IPublishedElement content, string alias, string? culture, string? segment, Fallback fallback, T? defaultValue, out T? value)
        => TryGetValueWithCultureFallback(content, alias, culture, out value, out _);

    public bool TryGetValue(IPublishedContent content, string alias, string? culture, string? segment, Fallback fallback, object? defaultValue, out object? value, out IPublishedProperty? noValueProperty)
        => TryGetValueWithCultureFallback(content, alias, culture, out value, out noValueProperty);

    public bool TryGetValue<T>(IPublishedContent content, string alias, string? culture, string? segment, Fallback fallback, T defaultValue, out T? value, out IPublishedProperty? noValueProperty)
        => TryGetValueWithCultureFallback(content, alias, culture, out value, out noValueProperty);

    private bool TryGetValueWithCultureFallback<T>(
        IPublishedElement content,
        string alias,
        string? culture,
        out T? value,
        out IPublishedProperty? noValueProperty)
    {
        noValueProperty = null;

        var property = content.GetProperty(alias);
        if (property is not null)
        {
            return TryGetValueWithCultureFallback(property, culture, out value);
        }
        
        value = default;
        return false;
    }
    
    private bool TryGetValueWithCultureFallback<T>(IPublishedProperty property, string? culture, out T? value)
    {
        // if a specific culture was requested, don't attempt a fallback to another culture
        if (culture.IsNullOrWhiteSpace() is false)
        {
            value = default;
            return false;
        }

        // get the fallback culture for the property (if any)
        var fallbackCulture = _fallbackHandler.GetFallbackCultureAsync(property).GetAwaiter().GetResult();
        if (fallbackCulture is null)
        {
            // no fallback culture or no property value for the fallback culture
            value = default;
            return false;
        }

        // get the property value for the fallback culture - prevent further fallback handling by using:
        // - the no-op published value fallback implementation (from core)
        // - explicit Fallback.None as fallback option
        value = property.Value<T>(_noopFallback, fallbackCulture, null, Fallback.To(Fallback.None));
        return true;
    }
}
