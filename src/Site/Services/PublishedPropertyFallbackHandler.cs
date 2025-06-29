using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;

namespace Site.Services;

public class PublishedPropertyFallbackHandler : IPublishedPropertyFallbackHandler
{
    private readonly ILanguageService _languageService;
    private readonly IVariationContextAccessor _variationContextAccessor;

    public PublishedPropertyFallbackHandler(ILanguageService languageService, IVariationContextAccessor variationContextAccessor)
    {
        _languageService = languageService;
        _variationContextAccessor = variationContextAccessor;
    }

    public async Task<string?> GetFallbackCultureAsync(IPublishedProperty property)
    {
        // sanity check the property before proceeding
        if (property.PropertyType.VariesByCulture() is false)
        {
            return null;
        }

        string? fallbackCulture = null;

        // get the requested (contextual) culture
        var requestedCulture = _variationContextAccessor.VariationContext?.Culture;

        if (requestedCulture.IsNullOrWhiteSpace() is false)
        {
            // traverse explicit language fallback configurations (if applicable)
            var cultureToAttempt = requestedCulture;
            while (fallbackCulture is null)
            {
                // get the language configuration for the culture
                var language = await _languageService.GetAsync(cultureToAttempt);

                // break if the language itself does not have a configured fallback language
                if (language?.FallbackIsoCode is null)
                {
                    break;
                }

                // if the property has a value for the fallback language, use the fallback language culture
                if (property.HasValue(culture: language.FallbackIsoCode))
                {
                    fallbackCulture = language.FallbackIsoCode;
                }

                cultureToAttempt = language.FallbackIsoCode;
            }
        }

        if (fallbackCulture is null)
        {
            // no explicit fallback language configured, try the default culture
            var defaultCulture = await _languageService.GetDefaultIsoCodeAsync();

            // if the default culture was not requested, and the property has a value for the default
            // culture, use that for fallback
            if (defaultCulture.InvariantEquals(requestedCulture) is false && property.HasValue(culture: defaultCulture))
            {
                fallbackCulture = defaultCulture;
            }
        }

        return fallbackCulture;
    }
}