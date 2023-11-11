using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LocalizationManager : EditorWindow
{
    [MenuItem("Tools/Localization Manager")]
    public static void ShowWindow()
    {
        GetWindow<LocalizationManager>("Localization Manager");
    }

    private List<Language> languageAssets;
    private string newLanguageName = "";
    private string warningMessage = "";
    const float buttonWidth = 60f;

    private void OnGUI()
    {
        GUIStyle boldStyle = new GUIStyle(GUI.skin.label);
        boldStyle.fontStyle = FontStyle.Bold;

        GUILayout.Label("Supported Languages", boldStyle);

        if (languageAssets != null)
        {
            for (int i = 0; i < languageAssets.Count; i++)
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label(languageAssets[i].name);

                if (GUILayout.Button("Open", GUILayout.Width(buttonWidth)))
                {
                    OpenLanguageEditor(languageAssets[i]);
                }

                if (GUILayout.Button("Remove", GUILayout.Width(buttonWidth)))
                {
                    bool userConfirmed = EditorUtility.DisplayDialog("Remove Confirmation", "Are you sure you want to remove the language?", "Yes", "No");

                    if (userConfirmed)
                        RemoveLanguage(languageAssets[i]);

                }

                GUILayout.EndHorizontal();
            }
        }


        GUILayout.FlexibleSpace();

        if (!string.IsNullOrEmpty(warningMessage))
        {
            EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);
        }
        GUILayout.BeginHorizontal();

        newLanguageName = GUILayout.TextField(newLanguageName);

        if (GUILayout.Button("Add Language", GUILayout.Width(buttonWidth * 2f)))
        {
            if (!string.IsNullOrWhiteSpace(newLanguageName))
                AddNewLanguage(newLanguageName);
            else
                warningMessage = "Il nome della lingua non può essere vuoto o composto solo da spazi.";
        }
        GUILayout.EndHorizontal();
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

    private void AddNewLanguage(string languageName)
    {
        Language newLanguage = ScriptableObject.CreateInstance<Language>();

        newLanguage.name = languageName;

        string assetPath = "Assets/Languages/" + languageName + ".asset";

        assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

        AssetDatabase.CreateAsset(newLanguage, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        AssetImporter importer = AssetImporter.GetAtPath(assetPath);
        if (importer != null)
        {
            AssetDatabase.SetLabels(importer, new[] { "Language" });
            importer.SaveAndReimport();
        }

        RefreshLanguageAssets();
    }

    private void OpenLanguageEditor(Language language)
    {
        string assetPath = AssetDatabase.GetAssetPath(language);
        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<Object>(assetPath));
    }

    private void RemoveLanguage(Language language)
    {
        string assetPath = AssetDatabase.GetAssetPath(language);

        AssetDatabase.DeleteAsset(assetPath);
        AssetDatabase.Refresh();

        RefreshLanguageAssets();
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
}
