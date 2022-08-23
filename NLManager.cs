using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Tahsin
{
    [System.Serializable]
    [ExecuteInEditMode]
    [EditorWindowTitle(title = "NLManager")]
    public class NLManager : EditorWindow , ISerializationCallbackReceiver
    {
        public static List<NaturalLanguage> languages = new();
        #region For Editor
        private enum Mode 
        {
            Overview,
            Analyze,
            Edit
        }
        private enum AnalyzeSubmod
        {
            ErrorCheck, Alphabet, Font, Text
        }
        private Mode mode;
        private AnalyzeSubmod analyzeSubmod;
        public readonly Color _compatible = Color.green;
        public readonly Color _uncompatible = Color.red;
        private Font fontToCheck;
        private NaturalLanguage languageToCheck;
        private bool checkWithCurrent;
        private List<NaturalLanguage> languagesToCheck = new();
        private List<Font> fontsToCheck = new();
        private int languagesToCheck_count;
        private int fontsToCheck_count;
        private string textToCheck;
        [SerializeReference]
        private NaturalLanguage[] _languages;
        #endregion
        [MenuItem("Window/NLManager")]
        static void Init()
        {
            GetWindow<NLManager>().Show();
        }
        private void OnGUI()
        {
            mode = (Mode)EditorGUILayout.EnumPopup("Mode", mode);
            EditorGUI.indentLevel++;
            switch (mode) 
            {
                case Mode.Overview:
                    OverviewMode();
                    break;
                case Mode.Analyze:
                    AnalyzeMode();
                    break;
                case Mode.Edit:
                    EditMode();
                    break;
            }
            EditorGUI.indentLevel--;
        }
        private void OverviewMode() 
        {
            if (GUILayout.Button("Add Selected")) 
            {
                foreach (Object @object in Selection.objects) 
                {
                    if (@object is NaturalLanguage nl && !languages.Contains(nl)) 
                    {
                        languages.Add(nl);
                    }
                }
            }
            for (int i = 0; i < languages.Count; i++)
            {
                GUILayout.BeginHorizontal();
                bool b = GUILayout.Button("Remove " + languages[i].languageName);
                if (b)
                {
                    languages.RemoveAt(i);
                }
                else
                {
                    if (languages[i].IsFontContainsAllOfTheCharacters() && languages[i].IsAllEqual()) GUI.color = _compatible;
                    else GUI.color = _uncompatible;
                    EditorGUILayout.LabelField(languages[i].languageName + $"({languages[i].originalName})");
                }
                GUILayout.EndHorizontal();
                GUI.color = Color.white;
            }
            GUI.color = Color.white;
        }
        private void AnalyzeMode()
        {
            analyzeSubmod = (AnalyzeSubmod)EditorGUILayout.EnumPopup("Submod",analyzeSubmod);
            switch (analyzeSubmod)
            {
                case AnalyzeSubmod.ErrorCheck:
                    foreach (NaturalLanguage nl in languages)
                    {
                        bool comp = nl.IsFontContainsAllOfTheCharacters() && nl.IsAllEqual();
                        GUI.color = comp ? _compatible : _uncompatible;
                        GUILayout.Label(nl.languageName + $"({nl.originalName})");
                        if (!comp)
                        {
                            GUILayout.Label("Error(s):");
                            if (!nl.IsFontContainsAllOfTheCharacters())
                            {
                                GUILayout.Label("Font " + nl.font.ToString());
                                if (nl.font != null)
                                {
                                    string s = null;
                                    foreach (char c in nl.GetUncompatibleCharacters())
                                    {
                                        s += " " + c;
                                    }
                                    GUILayout.Label("Uncompatible(s)" + s);
                                }
                            }
                            if (!nl.IsAllEqual())
                            {
                                GUILayout.Label("Upper and lower alphabets leng not same :" + $"{nl.upperAlphabet.Length} != {nl.lowerAlphabet.Length}");
                            }
                        }
                        GUI.color = Color.white;
                    }
                    break;
                case AnalyzeSubmod.Alphabet:
                    languageToCheck = (NaturalLanguage)EditorGUILayout.ObjectField("Language To Check", languageToCheck, typeof(NaturalLanguage), false);
                    if (GUILayout.Button("Add Selected Font(s) "))
                        foreach (Object @object in Selection.objects)
                        {
                            if (@object is Font font && !fontsToCheck.Contains(font))
                            {
                                fontsToCheck.Add(font);
                                fontsToCheck_count = fontsToCheck.Count;
                            }
                        }
                    fontsToCheck_count = EditorGUILayout.IntField("Count",fontsToCheck_count);
                    while (fontsToCheck.Count < fontsToCheck_count) 
                    {
                        fontsToCheck.Add(null);
                    }
                    while (fontsToCheck.Count > fontsToCheck_count) 
                    {
                        fontsToCheck.RemoveAt(fontsToCheck.Count - 1);
                    }
                    for (int i = 0; i < fontsToCheck.Count; i++) 
                    {
                        GUI.color = fontsToCheck[i] != null && languageToCheck != null && languageToCheck.IsFontContainsAllOfTheCharacters(fontsToCheck[i]) ? _compatible : _uncompatible;
                        fontsToCheck[i] = (Font)EditorGUILayout.ObjectField(fontsToCheck[i] == null ? null : fontsToCheck[i].name, fontsToCheck[i], typeof(Font), false);
                        GUI.color = Color.white;
                    }
                    break;
                case AnalyzeSubmod.Font:
                    fontToCheck = (Font)EditorGUILayout.ObjectField("Font to check ", fontToCheck, typeof(Font), false);
                    checkWithCurrent = EditorGUILayout.Toggle("Check With Current", checkWithCurrent);
                    if(GUILayout.Button("Add Selected Language(s) ")) 
                        foreach(Object @object in Selection.objects) 
                        {
                            if (@object is NaturalLanguage nl && !languagesToCheck.Contains(nl)) 
                            {
                                languagesToCheck.Add(nl);
                                languagesToCheck_count = languagesToCheck.Count;
                            }
                        }
                    if (checkWithCurrent)
                    {
                        foreach (NaturalLanguage nl in languages) 
                        {
                            GUI.color = nl.IsFontContainsAllOfTheCharacters(fontToCheck)? _compatible : _uncompatible;
                            GUILayout.Label(nl.languageName + $" ({nl.originalName})");
                            GUI.color = Color.white;
                        }
                    }
                    else 
                    {
                        languagesToCheck_count = EditorGUILayout.IntField("Count",languagesToCheck_count);
                        if (languagesToCheck.Count < languagesToCheck_count) 
                        {
                            while (languagesToCheck.Count != languagesToCheck_count) 
                            {
                                languagesToCheck.Add(null);
                            }
                        }
                        else if (languagesToCheck.Count > languagesToCheck_count) 
                        {
                            while (languagesToCheck.Count != languagesToCheck_count)
                            {
                                languagesToCheck.RemoveAt(languagesToCheck.Count - 1);
                            }
                        }
                        for (int i = 0; i < languagesToCheck.Count; i++) 
                        {
                            GUI.color = languagesToCheck[i].IsFontContainsAllOfTheCharacters(fontToCheck) ? _compatible : _uncompatible;
                            languagesToCheck[i] = EditorGUILayout.ObjectField(languagesToCheck[i], typeof(NaturalLanguage), false) as NaturalLanguage;
                            GUI.color = Color.white;
                        }
                        
                    }
                    break;
                case AnalyzeSubmod.Text:
                    textToCheck = EditorGUILayout.TextArea(textToCheck);
                    fontsToCheck_count = EditorGUILayout.IntField("Count", fontsToCheck_count);
                    while (fontsToCheck.Count < fontsToCheck_count)
                    {
                        fontsToCheck.Add(null);
                    }
                    while (fontsToCheck.Count > fontsToCheck_count)
                    {
                        fontsToCheck.RemoveAt(fontsToCheck.Count - 1);
                    }
                    for (int i = 0; i < fontsToCheck.Count; i++)
                    {
                        GUI.color = _compatible;
                        if (fontsToCheck[i] != null) 
                        {
                            foreach (char c in textToCheck)
                            {
                                if (!fontsToCheck[i].HasCharacter(c))
                                {
                                    GUI.color = _uncompatible;
                                    break;
                                }
                            } 
                        }
                        else 
                        {
                            GUI.color = _uncompatible;
                        }
                        fontsToCheck[i] = (Font)EditorGUILayout.ObjectField(fontsToCheck[i] == null ? null : fontsToCheck[i].name, fontsToCheck[i], typeof(Font), false);
                        GUI.color = Color.white;
                    }
                    break;
            }
        }
        private void EditMode() 
        {
            checkWithCurrent = EditorGUILayout.Toggle("Check With Curent", checkWithCurrent);
            if (checkWithCurrent) 
            {
                foreach(NaturalLanguage nl in languages) 
                {
                    GUI.color = !nl.IsFontContainsAllOfTheCharacters() || !nl.IsAllEqual() ? _uncompatible : _compatible;
                    GUILayout.Label(nl.languageName + $" ({nl.originalName}) ");
                    GUI.color = Color.white;
                    nl.OnGUI();
                }
            }
            else { }
        }

        public void OnBeforeSerialize()
        {
            _languages = languages.ToArray();
        }

        public void OnAfterDeserialize()
        {
            foreach (NaturalLanguage nl in _languages) 
            {
                languages.Add(nl);
            }
        }
    }
}