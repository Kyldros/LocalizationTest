using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[CustomEditor(typeof(LanguageStringSelector))]
public class LanguageStringSelectorEditor : Editor
{
    string previewText = "";
    private int selectedLanguageIndex = 0;
    private List<Language> languageAssets;

    public override void OnInspectorGUI()
    {
        LanguageStringSelector selector = (LanguageStringSelector)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (languageAssets != null)
        {
            string[] languageOptions = languageAssets.Select(lang => lang.name).ToArray();
            selectedLanguageIndex = EditorGUILayout.Popup("Select Preview Language:", selectedLanguageIndex, languageOptions);
            Language selectedLanguage = languageAssets[selectedLanguageIndex];

            if (selectedLanguage != null)
            {
                LocalizedString localizedString = selectedLanguage.localizedStrings.Find(ls => ls.id == selector.textID);

                if (localizedString != null)
                {
                    if (!string.IsNullOrEmpty(localizedString.text))
                        previewText = localizedString.text;
                    else
                        previewText = $"!!{selector.textID}!!";
                }
                else
                {
                    previewText = $"??{selector.textID}??";
                }

                EditorGUILayout.LabelField("Localized Text Preview:", previewText);
            }
            else
            {
                EditorGUILayout.HelpBox("Selected Language is null!", MessageType.Warning);
            }
        }
        else
            EditorGUILayout.HelpBox("No Language Found!", MessageType.Warning);
    }

    private void OnEnable()
    {
        RefreshLanguageAssets();
        EditorApplication.projectChanged += OnProjectChange;
    }
    private void OnDisable()
    {
        EditorApplication.projectChanged -= OnProjectChange;
    }
    private void OnProjectChange()
    {
        RefreshLanguageAssets();
        Repaint();
    }

    private void RefreshLanguageAssets()
    {
        languageAssets = new List<Language>();
        string[] languages = AssetDatabase.FindAssets("l:Language");
        foreach (string langPath in languages)
        {
            string path = AssetDatabase.GUIDToAssetPath(langPath);
            Language language = AssetDatabase.LoadAssetAtPath<Language>(path);
            if (language != null)
            {
                languageAssets.Add(language);
            }
        }
    }
}

