using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager instance;

    [HideInInspector]
    public string defaultLanguage = "English";

    private Language currentLanguage;

    public List<Language> supportedLanguages;

    private Dictionary<string, string> localizedText;

    private List<LanguageStringSelector> registeredText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadLanguage(defaultLanguage);
    }

    public void LoadLanguage(string language)
    {
        foreach (Language lang in supportedLanguages)
        {
            if (lang.name == language)
            {
                currentLanguage = lang;
                UpdateLocalizedText();
                UpdateVisibleText();
                return;
            }
        }

        Debug.LogWarning("Language not found: " + language);
    }

    private void UpdateVisibleText()
    {
        if (registeredText != null)
            foreach (LanguageStringSelector selector in registeredText)
            {
                selector.UpdateText();
            }
    }

    private void UpdateLocalizedText()
    {
        localizedText = new Dictionary<string, string>();

        foreach (LocalizedString entry in currentLanguage.localizedStrings)
        {
            localizedText[entry.id] = entry.text;
        }
    }

    public string GetLocalizedText(string id)
    {
        if (localizedText.ContainsKey(id))
        {
            if (string.IsNullOrWhiteSpace(localizedText[id]) || localizedText[id] == null)
                return $"!! {id} !!";

            return localizedText[id];
        }

        return "?? " + id + " ??";
    }

    public void RegisterTextSelector(LanguageStringSelector selector)
    {
        if (registeredText == null)
            registeredText = new List<LanguageStringSelector>();

        if (!registeredText.Contains(selector) && selector != null)
            registeredText.Add(selector);
    }
}
