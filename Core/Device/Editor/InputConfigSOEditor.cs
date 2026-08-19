using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

namespace FCT.Device.Editor
{
    [CustomEditor(typeof(InputConfigSO))]
    [CanEditMultipleObjects]
    public class InputConfigSOEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var config = (InputConfigSO)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("FCT Input System Linker", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Assign your main InputActionAsset here. Click 'Sync Actions' to analyze the asset and register its actions.", MessageType.Info);
            
            GUILayout.Space(10);
            
            EditorGUI.BeginChangeCheck();
            var newAsset = (InputActionAsset)EditorGUILayout.ObjectField("Input Action Asset", config.inputAsset, typeof(InputActionAsset), false);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(config, "Change Input Asset");
                config.inputAsset = newAsset;
                EditorUtility.SetDirty(config);
            }

            GUILayout.Space(15);

            GUI.enabled = config.inputAsset != null;

            if (GUILayout.Button("Sync Actions", GUILayout.Height(35)))
            {
                Undo.RecordObject(config, "Sync Actions");
                config.registeredActions.Clear();
                
                foreach (var action in config.inputAsset)
                {
                    config.registeredActions.Add(action.name);
                }
                
                EditorUtility.SetDirty(config);
                Debug.Log($"[FCT] Synced {config.registeredActions.Count} actions from {config.inputAsset.name}.");
            }
            
            GUI.enabled = true;
            GUILayout.Space(15);
            
            if (config.inputAsset != null && config.registeredActions != null && config.registeredActions.Count > 0)
            {
                EditorGUILayout.LabelField($"Registered Actions ({config.registeredActions.Count}):", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("helpbox");
                foreach (var actionName in config.registeredActions)
                {
                    EditorGUILayout.LabelField("• " + actionName);
                }
                EditorGUILayout.EndVertical();
            }
            else if (config.inputAsset == null)
            {
                EditorGUILayout.HelpBox("Please assign an InputActionAsset to sync.", MessageType.Warning);
            }
        }
    }
}
