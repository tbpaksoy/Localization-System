using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Tahsin
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "New Language Data", fileName = "Language Data")]
    public class NaturalLanguage : ScriptableObject
    {
        public string languageName = "English";
        public string originalName = "English";
        public string lowerAlphabet = "abcdefghijklmnopqrstuwvxyz";
        public string upperAlphabet = "ABCDEFGHIJKLMNOQPRSTUWVXYZ";
        public Font font;
        public bool IsFontContainsAllOfTheCharacters() => IsFontContainsAllOfTheCharacters(font);
        public bool IsFontContainsAllOfTheCharacters(Font font)
        {
            if (font == null) return false;
            if (font.dynamic && font.name != "Arial")
            {
                Debug.LogWarning("The font check system does not work with dynamic fonts.");
                return false;
            }
            foreach (char c in lowerAlphabet)
            {
                if (!font.HasCharacter(c)) return false;
            }
            foreach (char c in upperAlphabet)
            {
                if (!font.HasCharacter(c)) return false;
            }
            return true;
        }
        public List<char> GetUncompatibleCharacters() 
        {
            List<char> result = new();
            foreach (char c in lowerAlphabet) 
            {
                if (!font.HasCharacter(c)) 
                {
                    result.Add(c);
                }
            }
            foreach (char c in upperAlphabet)
            {
                if (!font.HasCharacter(c))
                {
                    result.Add(c);
                }
            }
            return result;
        }
        public bool IsAllEqual() => lowerAlphabet.Length == upperAlphabet.Length;
        [ContextMenu("GiveInfo")]
        public void GiveInfo() 
        {
            if (IsFontContainsAllOfTheCharacters()) 
            {
                Debug.Log("All character compatible with alphabet");
            }
            else 
            {
                Debug.LogError("All character incompatible with alphabet");
                string s = null;
                foreach (char c in GetUncompatibleCharacters()) 
                {
                    s += " " + c;
                }
                Debug.LogWarning(s);
            }
        }
        [ContextMenu("Remove Unnecessary")]
        public void RemoveUneccesaryCharacters() 
        {
            List<char> temp = new();
            for (int i = 0; i < lowerAlphabet.Length; i++) 
            {
                if (!temp.Contains(lowerAlphabet[i]) && lowerAlphabet[i] != ' ') 
                {
                    temp.Add(lowerAlphabet[i]);
                }
            }
            lowerAlphabet = new string(temp.ToArray());
            temp = new();
            for (int i = 0; i < upperAlphabet.Length; i++)
            {
                if (!temp.Contains(upperAlphabet[i]) && upperAlphabet[i] != ' ')
                {
                    temp.Add(upperAlphabet[i]);
                }
            }
            upperAlphabet = new string(temp.ToArray());
        }
        public void OnGUI() 
        {
            lowerAlphabet = EditorGUILayout.TextField("Lower Alphabet", lowerAlphabet);
            upperAlphabet = EditorGUILayout.TextField("Upper Alphabet", upperAlphabet);
            font = EditorGUILayout.ObjectField("Font", font, typeof(Font), false) as Font;
        }
    }
}