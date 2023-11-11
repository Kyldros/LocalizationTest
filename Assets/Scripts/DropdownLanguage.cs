using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropdownLanguage : MonoBehaviour
{
    private TMP_Dropdown dropdown;

    private void Start()
    {
        if (LanguageManager.instance != null)
        {
            dropdown = GetComponent<TMP_Dropdown>();
            InitializeDropdown();
        }
        else
        {
            Debug.LogError("LanguageManager not found.");
        }
    }

    private void InitializeDropdown()
    {
        if (dropdown != null)
        {
            dropdown.ClearOptions();
            List<Language> supportedLanguages = LanguageManager.instance.supportedLanguages;
            List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();

            foreach (Language lang in supportedLanguages)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(lang.name);
                dropdownOptions.Add(option);
            }

            dropdown.AddOptions(dropdownOptions);
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
        else
        {
            Debug.LogError("TMPDropdown not found.");
        }
    }

    private void OnDropdownValueChanged(int index)
    {
        string selectedLanguage = dropdown.options[index].text;
        LanguageManager.instance.LoadLanguage(selectedLanguage);
    }
}
