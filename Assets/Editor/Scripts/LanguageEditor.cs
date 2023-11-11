using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Language))]
public class LanguageEditor : Editor
{
    private SerializedProperty localizedStrings;

    private Vector2 _scrollView;

    private void OnEnable()
    {
        localizedStrings = serializedObject.FindProperty("localizedStrings");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Title();

        ListOfField();
        Buttons();

        serializedObject.ApplyModifiedProperties();
    }

    private void Buttons()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add New Line"))
        {
            localizedStrings.arraySize++;
        }

        if (GUILayout.Button("Update ID"))
        {
            UpdateIDsInOtherLanguages();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Import CSV", EditorStyles.miniButtonLeft))
        {
            ImportFromCSV();
        }

        if (GUILayout.Button("Export CSV", EditorStyles.miniButtonRight))
        {
            ExportToCSV();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ListOfField()
    {
        _scrollView = EditorGUILayout.BeginScrollView(_scrollView);

        for (int i = 0; i < localizedStrings.arraySize; i++)
        {
            SerializedProperty localizedString = localizedStrings.GetArrayElementAtIndex(i);
            SerializedProperty id = localizedString.FindPropertyRelative("id");
            SerializedProperty text = localizedString.FindPropertyRelative("text");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("ID", GUILayout.Width(15));
            EditorGUILayout.PropertyField(id, GUIContent.none, GUILayout.ExpandWidth(true));

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Text", GUILayout.Width(30));
            EditorGUILayout.PropertyField(text, GUIContent.none, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Remove Entry", GUILayout.Width(90)))
            {
                bool confirm = EditorUtility.DisplayDialog("Confirm Removal", "Are you sure you want to remove this entry?", "Yes", "No");

                if (confirm)
                {
                    localizedStrings.DeleteArrayElementAtIndex(i);
                    i--;
                }
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    private void Title()
    {
        GUIStyle boldUpperCaseStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold
        };

        EditorGUILayout.LabelField(target.name.ToUpper(), boldUpperCaseStyle);
    }

    private void UpdateIDsInOtherLanguages()
    {
        string[] languageAssets = AssetDatabase.FindAssets("l:Language");

        foreach (string langPath in languageAssets)
        {
            string languagePath = AssetDatabase.GUIDToAssetPath(langPath);
            Language otherLanguage = AssetDatabase.LoadAssetAtPath<Language>(languagePath);

            if (otherLanguage != target && otherLanguage.localizedStrings != null)
            {
                foreach (LocalizedString otherLocalizedString in otherLanguage.localizedStrings)
                {
                    Language currentLanguage = (Language)target;
                    bool idExistsInCurrentLanguage = currentLanguage.localizedStrings.Any(ls => ls != null && ls.id == otherLocalizedString.id);


                    if (!idExistsInCurrentLanguage)
                        currentLanguage.localizedStrings.Add(new LocalizedString { id = otherLocalizedString.id, text = "" });
                }
            }
        }

        EditorUtility.SetDirty(target);

        AssetDatabase.SaveAssets();
    }

    private void ImportFromCSV()
    {
        string filePath = EditorUtility.OpenFilePanel("Import CSV", "", "csv");

        if (!string.IsNullOrEmpty(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);

            if (lines.Length >= 2)
            {
                localizedStrings.ClearArray();

                string[] headers = lines[0].Split(',');
                Dictionary<string, int> headerIndices = new Dictionary<string, int>();
                for (int i = 0; i < headers.Length; i++)
                {
                    headerIndices.Add(headers[i], i);
                }

                for (int i = 1; i < lines.Length; i++)
                {
                    string[] data = lines[i].Split(',');

                    if (data.Length >= 2)
                    {
                        string id = data[headerIndices["ID"]];
                        string text = data[headerIndices["Text"]];

                        localizedStrings.InsertArrayElementAtIndex(localizedStrings.arraySize);
                        SerializedProperty newLocalizedString = localizedStrings.GetArrayElementAtIndex(localizedStrings.arraySize - 1);
                        newLocalizedString.FindPropertyRelative("id").stringValue = id;
                        newLocalizedString.FindPropertyRelative("text").stringValue = text;
                    }
                }
            }
        }
    }

    private void ExportToCSV()
    {
        string filePath = EditorUtility.SaveFilePanel("Export CSV", "", target.name, "csv");

        if (!string.IsNullOrEmpty(filePath))
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("ID,Text");

                for (int i = 0; i < localizedStrings.arraySize; i++)
                {
                    SerializedProperty localizedString = localizedStrings.GetArrayElementAtIndex(i);
                    string id = localizedString.FindPropertyRelative("id").stringValue;
                    string text = localizedString.FindPropertyRelative("text").stringValue;
                    writer.WriteLine($"{id},{text}");
                }
            }

            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }
}