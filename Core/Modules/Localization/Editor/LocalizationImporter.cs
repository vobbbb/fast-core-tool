using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.IO;

using FCT.Localization;

namespace FCT.Localization.Editor
{
    public static class LocalizationImporter
    {
        [MenuItem("Window/FCT/Localization/Sync From Google Sheets")]
        public static void Import()
        {
            string SheetUrl = EditorPrefs.GetString("FCT_LocalizationSheetUrl", "");
            if (string.IsNullOrEmpty(SheetUrl))
            {
                Debug.LogError("[Localization] Sheet URL is not configured. Please set it in FCT Core Wizard -> Config.");
                return;
            }

            // Automatically format the URL if the user pastes the standard viewing link
            if (SheetUrl.Contains("/edit"))
            {
                SheetUrl = SheetUrl.Substring(0, SheetUrl.IndexOf("/edit")) + "/export?format=csv";
            }

            Debug.Log("[Localization] Starting import from: " + SheetUrl);
            var www = UnityWebRequest.Get(SheetUrl);
            var operation = www.SendWebRequest();

            while (!operation.isDone) { /* Waiting for download */ }

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[Localization] Error downloading sheet: {www.error}");
                return;
            }

            ProcessCSV(www.downloadHandler.text);
        }

        private static void ProcessCSV(string csvText)
        {
            LocalizationData data = null;
            var guids = AssetDatabase.FindAssets("t:LocalizationData");
            if (guids.Length > 0)
            {
                data = AssetDatabase.LoadAssetAtPath<LocalizationData>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }

            if (data == null)
            {
                Debug.LogError("[Localization] Data asset not found. Please create it first via FCT Core Wizard.");
                return;
            }

            Undo.RecordObject(data, "Import Localization Data");
            data.entries.Clear();

            string[] lines = csvText.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
            
            // Skip header (Line 0)
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] columns = ParseCSVLine(lines[i]);
                if (columns.Length < 4) continue;

                string key = columns[0].Trim();
                if (string.IsNullOrEmpty(key)) continue;

                string platform = columns[1].Trim().ToUpper();
                bool isMobile = platform.Contains("MOBILE") || platform.Contains("GAMEPAD");

                var existingEntry = data.entries.Find(e => e.key == key);
                if (existingEntry == null)
                {
                    existingEntry = new LocalizationEntry { key = key };
                    data.entries.Add(existingEntry);
                }

                if (isMobile)
                {
                    existingEntry.hasMobileOverride = true;
                    existingEntry.englishMobile = columns[2].Trim();
                    existingEntry.spanishMobile = columns[3].Trim();
                    if (columns.Length > 4) existingEntry.portugueseMobile = columns[4].Trim();
                }
                else
                {
                    existingEntry.english = columns[2].Trim();
                    existingEntry.spanish = columns[3].Trim();
                    if (columns.Length > 4) existingEntry.portuguese = columns[4].Trim();
                }
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Localization] Successfully imported {data.entries.Count} entries.");
        }

        // Simple CSV parser that handles quotes
        private static string[] ParseCSVLine(string line)
        {
            List<string> parts = new List<string>();
            bool inQuotes = false;
            string current = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '\"') inQuotes = !inQuotes;
                else if (c == ',' && !inQuotes)
                {
                    parts.Add(current);
                    current = "";
                }
                else current += c;
            }
            parts.Add(current);
            return parts.ToArray();
        }
    }
}
