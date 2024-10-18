using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public interface ILocalizationManager
{
    event Action OnLanguageChanged;
    void SetLanguage(string localeCode);
    void Clear();
}


public class LocalizationManager : ILocalizationManager
{
    public event Action OnLanguageChanged;

    public static string GetCurrentLanguageCode()
    {
        return LocalizationSettings.SelectedLocale.Identifier.Code;
    }

    public void SetLanguage(string localeCode)
    {
        var locale = GetLocale(localeCode);
        SetLanguage(locale);
    }

    public void Clear()
    {
        OnLanguageChanged = null;
    }

    public static string GetLocalizedString(string table, string key)
    {
        string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
        return localizedString;
    }

    public static string GetLocalizedString(string table, string key, params object[] arguments)
    {
        string localizedString = LocalizationSettings.StringDatabase.GetLocalizedString(table, key, arguments);
        return localizedString;
    }

    public static Sprite GetLocalizedSprite(string table, string key)
    {
        Sprite localizedTexture = LocalizationSettings.AssetDatabase.GetLocalizedAsset<Sprite>(table, key, null);
        return localizedTexture;
    }

    public static string GetLocalizedTaskTitle(string nameKey)
    {
        var title = GetLocalizedString("Game Names", nameKey);
        return title;
    }

    private void SetLanguage(Locale locale)
    {
        LocalizationSettings.SelectedLocale = locale;
        OnLanguageChanged?.Invoke();
    }

    private static Locale GetLocale(string localeCode) => LocalizationSettings.AvailableLocales.Locales
                                                                              .First(locale => locale.Identifier.Code == localeCode);
}
