#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Heron.Core.Editor
{
    public class PlatformSwitcher : EditorWindow
    {
        [MenuItem("FCT/Platform Switcher/Open Switcher Dashboard", false, 0)]
        public static void OpenWindow()
        {
            var window = GetWindow<PlatformSwitcher>("Platform Switcher");
            window.minSize = new Vector2(350, 280);
            window.maxSize = new Vector2(350, 280);
            window.Show();
        }

        private void OnGUI()
        {
            // Title and headers
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(0, 0, 15, 15)
            };

            GUIStyle statusLabelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Normal
            };

            GUIStyle activePlatformStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            };

            // Main spacing
            EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(20, 20, 15, 15) });

            GUILayout.Label("Heron Games Platform Switcher", titleStyle);
            
            DrawLine(new Color(0.35f, 0.35f, 0.35f), 1, 10);

            // Active build target detection
            BuildTarget activeTarget = EditorUserBuildSettings.activeBuildTarget;
            string platformName = "Unknown";
            Color statusColor = Color.gray;

            if (activeTarget == BuildTarget.StandaloneWindows || activeTarget == BuildTarget.StandaloneWindows64)
            {
                platformName = "Windows Standalone PC";
                statusColor = new Color(0.2f, 0.65f, 0.3f); // Emerald green
            }
            else if (activeTarget == BuildTarget.Android)
            {
                platformName = "Android";
                statusColor = new Color(0.9f, 0.45f, 0.1f); // Vibrant orange
            }
            else
            {
                platformName = activeTarget.ToString();
            }

            // Current Platform Card
            EditorGUILayout.LabelField("Current Platform Configuration", statusLabelStyle);
            GUILayout.Space(2);
            
            // Draw a neat status card
            Rect cardRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.Space(8);
                activePlatformStyle.normal.textColor = statusColor;
                GUILayout.Label(platformName.ToUpper(), activePlatformStyle);
                GUILayout.Space(8);
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(15);

            // Large Styled Switch Buttons
            GUI.enabled = (activeTarget != BuildTarget.StandaloneWindows64 && activeTarget != BuildTarget.StandaloneWindows);
            if (GUILayout.Button("Switch to Standalone PC (Windows)", GUILayout.Height(38)))
            {
                SwitchToPC();
            }
            
            GUI.enabled = (activeTarget != BuildTarget.Android);
            if (GUILayout.Button("Switch to Android", GUILayout.Height(38)))
            {
                SwitchToAndroid();
            }
            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }

        [MenuItem("FCT/Platform Switcher/Quick Switch to PC (Windows)", false, 20)]
        public static void SwitchToPC()
        {
            SwitchPlatform(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64, "Windows Standalone PC");
        }

        [MenuItem("FCT/Platform Switcher/Quick Switch to Android", false, 21)]
        public static void SwitchToAndroid()
        {
            SwitchPlatform(BuildTargetGroup.Android, BuildTarget.Android, "Android");
        }

        private static void SwitchPlatform(BuildTargetGroup targetGroup, BuildTarget target, string platformName)
        {
            BuildTarget activeTarget = EditorUserBuildSettings.activeBuildTarget;
            if (activeTarget == target)
            {
                EditorUtility.DisplayDialog("Already on Platform", $"The active platform is already {platformName}.", "OK");
                return;
            }

            bool proceed = EditorUtility.DisplayDialog("Switch Build Platform",
                $"Are you sure you want to switch the active build platform to {platformName}?\nThis will compile scripts and reimport platform-specific assets.",
                "Yes, Switch", "Cancel");

            if (proceed)
            {
                Debug.Log($"[PlatformSwitcher] Switching active build platform to {platformName} ({target})...");
                
                // Show progress bar
                EditorUtility.DisplayProgressBar("Platform Switcher", $"Switching target to {platformName}...", 0.5f);
                
                try
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);
                    Debug.Log($"[PlatformSwitcher] Successfully switched platform to {platformName}.");
                }
                finally
                {
                    EditorUtility.ClearProgressBar();
                }
            }
        }

        private void DrawLine(Color color, int thickness = 1, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
    }
}
#endif
