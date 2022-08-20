using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

namespace Tahsin
{
    [RequireComponent(typeof(Text))]
    public class TextTranslator : MonoBehaviour, ISerializationCallbackReceiver
    {
        public enum Alignment { Width, Height, Both }
        public Alignment alignment;
        public NaturalLanguage selected;
        public Text text;
        public Dictionary<NaturalLanguage, string> translations = new();
        [SerializeReference]
        private NaturalLanguage[] _languages;
        [SerializeReference]
        private string[] _texts;
        public void OnAfterDeserialize()
        {
            for(int i = 0; i < _languages.Length; i++) 
            {
                translations.Add(_languages[i], _texts[i]);
            }
        }
        public void OnBeforeSerialize()
        {
            _languages = translations.Keys.ToArray();
            _texts = translations.Values.ToArray();
        }
        public void Translate(NaturalLanguage language) 
        {
            if (text == null) text = GetComponent<Text>();
            if (translations.ContainsKey(language))
            {
                text.text = translations[language];
            }
            else Debug.LogError("This language could not find : " + language.languageName);
            switch (alignment) 
            {
                case Alignment.Height:
                    float height = LayoutUtility.GetPreferredHeight(text.rectTransform);
                    text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, height);
                    break;
                case Alignment.Width:
                    float width = LayoutUtility.GetPreferredWidth(text.rectTransform);
                    text.rectTransform.sizeDelta = new Vector2(width, text.rectTransform.sizeDelta.y);
                    break;
                case Alignment.Both:
                    height = LayoutUtility.GetPreferredHeight(text.rectTransform); 
                    width = LayoutUtility.GetPreferredWidth(text.rectTransform);
                    text.rectTransform.sizeDelta = new Vector2(width, height);
                    break;
            }
        }
        public void Translate() 
        {
            if (selected == null) return;
            Translate(selected);
        }
        private void Awake()
        {
            text = GetComponent<Text>();
        }
        private void Start()
        {
            Translate(selected);
        }
    }
    [CustomEditor(typeof(TextTranslator))]
    public class TextTranslatorEditor : Editor 
    {
        public override void OnInspectorGUI()
        {
            TextTranslator tt = target as TextTranslator;
            tt.alignment = (TextTranslator.Alignment)EditorGUILayout.EnumPopup(tt.alignment);
            tt.selected = EditorGUILayout.ObjectField("Selected", tt.selected, typeof(NaturalLanguage), false) as NaturalLanguage;
            foreach (NaturalLanguage nl in NLManager.languages) 
            {
                if (!tt.translations.ContainsKey(nl))
                {
                    tt.translations.Add(nl, null);
                }
            }
            NaturalLanguage[] languages = tt.translations.Keys.ToArray();
            for (int i = 0; i < languages.Length; i++) 
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Select")) 
                { 
                    tt.selected = languages[i];
                    tt.Translate();
                }
                GUILayout.Label(languages[i].languageName + $" ({languages[i].originalName})");
                GUILayout.EndHorizontal();
                tt.translations[languages[i]] = EditorGUILayout.TextArea(tt.translations[languages[i]]);
            }
        }
    }
}
