using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LanguageManager))]
public class LanguageManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LanguageManager languageManager = (LanguageManager)target;

        if (languageManager.supportedLanguages == null || languageManager.supportedLanguages.Count == 0)
        {
            EditorGUILayout.HelpBox("Add supported languages first.", MessageType.Warning);
            DrawDefaultInspector();
        }
        else
        {
            string[] languageNames = languageManager.supportedLanguages.Where(lang => lang != null).Select(lang => lang.name).ToArray();
            int selectedIndex = languageManager.supportedLanguages.FindIndex(lang => lang != null && lang.name == languageManager.defaultLanguage);
            selectedIndex = Mathf.Max(0, selectedIndex);
            int newSelectedIndex = EditorGUILayout.Popup("Default Language", selectedIndex, languageNames);

            if (newSelectedIndex != selectedIndex)
            {
                languageManager.defaultLanguage = languageManager.supportedLanguages[newSelectedIndex].name;
            }

            DrawDefaultInspector();
        }
    }
}
