using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageStringSelector : MonoBehaviour
{
    public string textID; 
    private TextMeshProUGUI textComponent;

    private void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        if (LanguageManager.instance != null)
        {
            UpdateText();
        }
        else
        {
            Debug.LogError("LanguageManager not found.");
        }

        LanguageManager.instance.RegisterTextSelector(this);
    }


    public void UpdateText()
    {
        if (textComponent != null)
        {
            string localizedText = LanguageManager.instance.GetLocalizedText(textID);

            textComponent.text = localizedText;
        }
        else
        {
            Debug.LogError("TextMeshPro not found.");
        }
    }
}
