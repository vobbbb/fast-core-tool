using UnityEngine;
using UnityEditor;
using System.Linq;

namespace FCT.Localization.Editor
{
    [CustomEditor(typeof(LocalizeText))]
    [CanEditMultipleObjects]
    public class LocalizeTextEditor : UnityEditor.Editor
    {
        private SerializedProperty localizationKeyProp;
        private string[] availableKeys;
        private int selectedIndex = -1;

        private void OnEnable()
        {
            localizationKeyProp = serializedObject.FindProperty("localizationKey");
            RefreshKeys();
        }

        private void RefreshKeys()
        {
            var guids = AssetDatabase.FindAssets("t:LocalizationData");
            if (guids.Length > 0)
            {
                var data = AssetDatabase.LoadAssetAtPath<LocalizationData>(AssetDatabase.GUIDToAssetPath(guids[0]));
                if (data != null && data.entries != null)
                {
                    availableKeys = data.entries.Select(e => e.key).Where(k => !string.IsNullOrEmpty(k)).ToArray();
                }
            }

            if (availableKeys == null || availableKeys.Length == 0)
            {
                availableKeys = new string[] { "No keys found" };
            }

            // Buscar el indice actual
            selectedIndex = System.Array.IndexOf(availableKeys, localizationKeyProp.stringValue);
            
            if (selectedIndex == -1)
            {
                if (!string.IsNullOrEmpty(localizationKeyProp.stringValue))
                {
                    // Si la key actual existe pero no está en la base de datos (Ej: un typo, o se borró), la mostramos igual
                    var tempList = availableKeys.ToList();
                    tempList.Insert(0, localizationKeyProp.stringValue + " (Missing)");
                    availableKeys = tempList.ToArray();
                    selectedIndex = 0;
                }
                else
                {
                    selectedIndex = 0;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Localization Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("helpbox");
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            
            int newIndex = EditorGUILayout.Popup("Key:", selectedIndex, availableKeys);
            if (newIndex != selectedIndex && newIndex >= 0 && newIndex < availableKeys.Length)
            {
                selectedIndex = newIndex;
                string newKey = availableKeys[selectedIndex];
                
                if (newKey.EndsWith(" (Missing)"))
                {
                    newKey = newKey.Substring(0, newKey.Length - 10);
                }
                
                if (newKey != "No keys found")
                {
                    localizationKeyProp.stringValue = newKey;
                }
            }

            if (GUILayout.Button(new GUIContent("↻", "Refresh Keys from Database"), GUILayout.Width(25)))
            {
                RefreshKeys();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            
            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.Space();
            if (GUILayout.Button("Force Refresh Text on Object", GUILayout.Height(30)))
            {
                foreach (LocalizeText script in targets)
                {
                    script.RefreshText();
                    EditorUtility.SetDirty(script);
                }
            }
        }
    }
}
