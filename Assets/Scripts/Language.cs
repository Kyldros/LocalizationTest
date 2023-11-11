using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LocalizedString
{
    public string id;
    public string text;
}


[CreateAssetMenu(fileName = "NewLanguage", menuName = "Localization/Language")]
public class Language : ScriptableObject
{
    public List<LocalizedString> localizedStrings;
}
