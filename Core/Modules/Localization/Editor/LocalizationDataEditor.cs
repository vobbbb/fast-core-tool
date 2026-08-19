using UnityEngine;
using UnityEditor;

using FCT.Localization;

namespace FCT.Localization.Editor
{
    [CustomEditor(typeof(LocalizationData))]
    public class LocalizationDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw a big button at the top
            GUI.backgroundColor = new Color(0.7f, 1f, 0.7f); // Light green
            if (GUILayout.Button("▼ IMPORT FROM GOOGLE SHEETS ▼", GUILayout.Height(40)))
            {
                LocalizationImporter.Import();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(10);
            
            // Draw the default inspector (the list of entries)
            base.OnInspectorGUI();
        }
    }
}
