using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace FCT.Tools.Editor
{
    public class FCTCoreWizard : EditorWindow
    {
        private string newStateName = "MyNewState";
        private string targetManagerName = "MyGameManager";
        private int selectedTab = 0;
        private int tutorialStep = 0;
        private Vector2 scrollPosition;
        private bool showLocalizationConfig = true;
        
        // Pestañas actualizadas con "Home" al principio
        private readonly string[] tabs = { "Home", "Setup Hub", "Generators", "Live Debugger", "Config" };

        [MenuItem("Window/FCT/Core Wizard", false, 10)]
        public static void ShowWindow()
        {
            var window = GetWindow<FCTCoreWizard>("FCT Core");
            window.minSize = new Vector2(400, 560);
            window.Show();
        }

        private void OnEnable() => EditorApplication.update += OnEditorUpdate;
        private void OnDisable() => EditorApplication.update -= OnEditorUpdate;

        private void OnEditorUpdate()
        {
            // El Live Debugger ahora está en el índice 3
            if (Application.isPlaying && selectedTab != 3)
            {
                selectedTab = 3;
                Repaint();
            }
            else if (Application.isPlaying)
            {
                Repaint();
            }
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawTabs();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(15, 15, 10, 10) });

            if (selectedTab == 0) DrawHomeMode();
            else if (selectedTab == 1) DrawSetupMode();
            else if (selectedTab == 2) DrawGeneratorMode();
            else if (selectedTab == 3) DrawDebugMode();
            else if (selectedTab == 4) DrawConfigMode();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            Rect headerRect = new Rect(0, 0, position.width, 60);
            EditorGUI.DrawRect(headerRect, new Color(0.12f, 0.12f, 0.12f));

            GUILayout.Space(15);
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel) 
            { 
                fontSize = 20, 
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.9f, 0.9f, 0.9f) }
            };
            GUILayout.Label("FAST CORE TOOL", titleStyle);

            GUIStyle subStyle = new GUIStyle(EditorStyles.label) 
            { 
                fontSize = 11, 
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.6f, 0.6f, 0.6f) }
            };
            GUILayout.Label("Professional Framework Initializer", subStyle);
            GUILayout.Space(25);
        }

        private void DrawTabs()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            for (int i = 0; i < tabs.Length; i++)
            {
                bool isSelected = (selectedTab == i);
                GUI.backgroundColor = isSelected ? new Color(0.224f, 0.549f, 0.427f) : Color.white;
                
                if (GUILayout.Button(tabs[i], EditorStyles.miniButtonMid, GUILayout.Width(85), GUILayout.Height(24)))
                {
                    selectedTab = i;
                    GUI.FocusControl(null);
                }
                GUI.backgroundColor = Color.white;
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            Rect lineRect = GUILayoutUtility.GetRect(1, 1);
            lineRect.width = position.width;
            lineRect.x = 0;
            EditorGUI.DrawRect(lineRect, new Color(0.15f, 0.15f, 0.15f));
            GUILayout.Space(10);
        }

        private void DrawHomeMode()
        {
            EditorGUILayout.LabelField("Welcome to FCT", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Fast Core Tool (FCT) is a professional, modular framework for Unity that jumpstarts your game development by handling boilerplate systems like Inputs, State Machines, Localization, and Object Pooling.", MessageType.Info);
            
            GUILayout.Space(20);
            
            GUI.backgroundColor = new Color(0.224f, 0.549f, 0.427f);
            if (GUILayout.Button("Start Quick Tutorial", GUILayout.Height(40)))
            {
                tutorialStep = 1;
                selectedTab = 1; // Mover a Setup Hub
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("The Quick Tutorial will guide you step-by-step through configuring all essential systems for a new project. Highly recommended for first-time users.", MessageType.None);
        }

        private void DrawSetupMode()
        {
            if (tutorialStep > 0)
            {
                EditorGUILayout.BeginVertical("helpbox");
                GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f);
                GUILayout.Button($"Tutorial Mode: Step {Mathf.Min(tutorialStep, 5)} of 5", EditorStyles.boldLabel, GUILayout.Height(30));
                GUI.backgroundColor = Color.white;
                
                string helpText = "";
                switch (tutorialStep)
                {
                    case 1: helpText = "First, let's create the Input Config. Click the 'Create Input Config' button below."; break;
                    case 2: helpText = "Great! Next, instantiate the object pool manager by clicking 'Setup SimplePool'."; break;
                    case 3: helpText = "Now, generate the boilerplate State Machine script by clicking 'Generate GameManager'."; break;
                    case 4: helpText = "Let's setup the camera. Click 'Setup Cinemachine' to add a virtual camera."; break;
                    case 5: helpText = "Finally, setup the Localization System. Click 'Setup Localization'."; break;
                    case 6: helpText = "Tutorial Complete! Your scene is now fully initialized with all FCT systems."; break;
                }
                
                EditorGUILayout.HelpBox(helpText, tutorialStep == 6 ? MessageType.Info : MessageType.Warning);
                
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (tutorialStep < 6)
                {
                    if (GUILayout.Button("Quit Tutorial", EditorStyles.miniButtonRight, GUILayout.Width(100))) 
                    {
                        tutorialStep = 0;
                    }
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                GUILayout.Space(10);
            }

            EditorGUILayout.LabelField("Project Initialization", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Follow these steps to initialize the core systems in your current scene.", MessageType.Info);
            GUILayout.Space(10);

            // Bloquear todos los botones que no sean el paso actual del tutorial (si está activo)
            GUI.enabled = tutorialStep == 0 || tutorialStep == 1;
            DrawSetupCard(1, "1. Input System", "Creates a ScriptableObject to map new Input System actions.", "Create Input Config", () => { CreateInputConfig(); AdvanceTutorial(1); });
            
            GUI.enabled = tutorialStep == 0 || tutorialStep == 2;
            DrawSetupCard(2, "2. Object Pooling", "Instantiates the SimplePool manager in the active scene.", "Setup SimplePool", () => { CreateSimplePool(); AdvanceTutorial(2); });
            
            GUI.enabled = tutorialStep == 0 || tutorialStep == 3;
            DrawSetupCard(3, "3. FSM Architecture", "Generates a boilerplate GameManager script in your project.", "Generate GameManager", () => { GenerateGameManager(); AdvanceTutorial(3); });
            
            GUI.enabled = tutorialStep == 0 || tutorialStep == 4;
            DrawSetupCard(4, "4. Camera System", "Adds a Cinemachine Brain and Virtual Camera to the scene.", "Setup Cinemachine", () => { SetupCamera(); AdvanceTutorial(4); });
            
            GUI.enabled = tutorialStep == 0 || tutorialStep == 5;
            DrawSetupCard(5, "5. Localization System", "Instantiates the Localization Manager and generates base Data.", "Setup Localization", () => { SetupLocalization(); AdvanceTutorial(5); });

            GUI.enabled = true;

            if (tutorialStep == 6)
            {
                GUILayout.Space(20);
                GUI.backgroundColor = new Color(0.224f, 0.549f, 0.427f); // #398C6D
                if (GUILayout.Button("Finish Tutorial", GUILayout.Height(40)))
                {
                    tutorialStep = 0;
                    selectedTab = 0; // Volver al Home
                }
                GUI.backgroundColor = Color.white;
            }
        }

        private void AdvanceTutorial(int fromStep)
        {
            if (tutorialStep == fromStep)
            {
                tutorialStep++;
            }
        }

        private void DrawSetupCard(int stepIndex, string title, string description, string buttonText, System.Action onButtonClick)
        {
            bool isCurrentStep = tutorialStep > 0 && tutorialStep == stepIndex;
            bool isCompleted = tutorialStep > stepIndex;

            if (isCurrentStep) GUI.color = new Color(0.133f, 0.133f, 0.133f); // #222222
            EditorGUILayout.BeginVertical("window");
            if (isCurrentStep) GUI.color = Color.white;

            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            if (isCompleted)
            {
                GUIStyle completedStyle = new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = new Color(0.2f, 0.8f, 0.2f) } };
                GUILayout.Label("✔ Completed", completedStyle, GUILayout.Width(90));
            }
            GUILayout.EndHorizontal();
            
            GUIStyle descStyle = new GUIStyle(EditorStyles.wordWrappedLabel) { fontSize = 11, normal = { textColor = new Color(0.7f, 0.7f, 0.7f) } };
            EditorGUILayout.LabelField(description, descStyle);
            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            // Botones por defecto color #398C6D
            GUI.backgroundColor = new Color(0.224f, 0.549f, 0.427f);
            if (GUILayout.Button(buttonText, GUILayout.Width(160), GUILayout.Height(26)))
            {
                onButtonClick?.Invoke();
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }

        private void DrawGeneratorMode()
        {
            EditorGUILayout.LabelField("FSM State Generator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Quickly scaffold new State classes for your GameManagers without writing boilerplate code.", MessageType.Info);
            GUILayout.Space(15);

            EditorGUILayout.BeginVertical("helpbox");
            GUILayout.Space(10);
            
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 120;

            newStateName = EditorGUILayout.TextField("State Class Name:", newStateName);
            GUILayout.Space(5);
            targetManagerName = EditorGUILayout.TextField("Target Manager:", targetManagerName);
            
            EditorGUIUtility.labelWidth = originalLabelWidth;

            GUILayout.Space(15);
            
            GUI.backgroundColor = new Color(0.224f, 0.549f, 0.427f);
            if (GUILayout.Button("Generate State Script", GUILayout.Height(35)))
            {
                GenerateStateScript(newStateName, targetManagerName);
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }

        private void DrawDebugMode()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Live Debugger is only active during Play Mode.", MessageType.Warning);
                return;
            }

            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = new Color(0.2f, 0.8f, 0.2f) }, alignment = TextAnchor.MiddleCenter, fontSize = 14 };
            GUILayout.Label("● LIVE DEBUGGING", headerStyle);
            GUILayout.Space(15);

            var managers = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                                 .OfType<FCT.Game.ICoreGameManagerDebug>()
                                 .ToList();

            if (managers.Count == 0)
            {
                EditorGUILayout.HelpBox("No active GameManagers found in scene.", MessageType.Info);
                return;
            }

            foreach (var manager in managers)
            {
                EditorGUILayout.BeginVertical("helpbox");
                
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Manager:", EditorStyles.boldLabel, GUILayout.Width(80));
                EditorGUILayout.LabelField(manager.ManagerName);
                GUILayout.EndHorizontal();
                
                GUILayout.Space(2);
                
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("State:", EditorStyles.boldLabel, GUILayout.Width(80));
                GUIStyle stateStyle = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold, normal = { textColor = new Color(0.1f, 0.6f, 0.9f) } };
                EditorGUILayout.LabelField(manager.CurrentStateName, stateStyle);
                GUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                GUILayout.Space(8);
            }
        }

        private void DrawConfigMode()
        {
            EditorGUILayout.LabelField("Global Configuration", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Configure external integrations and global settings for FCT Modules.", MessageType.Info);
            GUILayout.Space(15);

            EditorGUILayout.BeginVertical("helpbox");
            
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
            showLocalizationConfig = EditorGUILayout.Foldout(showLocalizationConfig, "Localization Module", true, foldoutStyle);
            
            if (showLocalizationConfig)
            {
                GUILayout.Space(10);

                // Path Selector
                EditorGUILayout.LabelField("LocalizationData Asset Path:");
                GUILayout.BeginHorizontal();
                string currentDataPath = EditorPrefs.GetString("FCT_LocAssetPath", "Assets/Resources/Database/LocalizationData.asset");
                string newDataPath = EditorGUILayout.TextField(currentDataPath);
                
                GUI.backgroundColor = new Color(0.224f, 0.549f, 0.427f);
                if (GUILayout.Button("Browse", GUILayout.Width(70)))
                {
                    string selected = EditorUtility.SaveFilePanelInProject("Select LocalizationData", "LocalizationData", "asset", "Select where to save or find LocalizationData.");
                    if (!string.IsNullOrEmpty(selected)) newDataPath = selected;
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();

                if (newDataPath != currentDataPath)
                {
                    EditorPrefs.SetString("FCT_LocAssetPath", newDataPath);
                }

                if (!File.Exists(newDataPath))
                {
                    GUI.backgroundColor = new Color(0.9f, 0.4f, 0.4f);
                    GUILayout.Space(5);
                    if (GUILayout.Button("File not found! Create Data Asset Here", GUILayout.Height(28)))
                    {
                        string dir = Path.GetDirectoryName(newDataPath);
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                        var data = ScriptableObject.CreateInstance<FCT.Localization.LocalizationData>();
                        AssetDatabase.CreateAsset(data, newDataPath);
                        AssetDatabase.SaveAssets();
                        Debug.Log("[FCT] LocalizationData created at " + newDataPath);
                    }
                    GUI.backgroundColor = Color.white;
                }
                
                GUILayout.Space(15);
                
                string currentUrl = EditorPrefs.GetString("FCT_LocalizationSheetUrl", "");
                
                EditorGUILayout.LabelField("Google Sheets CSV Export URL:");
                string newUrl = EditorGUILayout.TextArea(currentUrl, GUILayout.Height(50));
                
                if (newUrl != currentUrl)
                {
                    EditorPrefs.SetString("FCT_LocalizationSheetUrl", newUrl);
                }
                
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                
                GUI.backgroundColor = new Color(0.224f, 0.549f, 0.427f);
                if (GUILayout.Button("View Example CSV", GUILayout.Width(130), GUILayout.Height(24)))
                {
                    var exampleAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/FCT/Resources/Localization_Example.csv");
                    if (exampleAsset != null)
                    {
                        EditorGUIUtility.PingObject(exampleAsset);
                        Selection.activeObject = exampleAsset;
                    }
                    else
                    {
                        Debug.LogWarning("[FCT] Localization_Example.csv not found at Assets/FCT/Resources/Localization_Example.csv");
                    }
                }
                
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Sync Now", GUILayout.Width(100), GUILayout.Height(24)))
                {
                    FCT.Localization.Editor.LocalizationImporter.Import();
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
            
            EditorGUILayout.EndVertical();
        }

        // ------------------ LOGIC ------------------

        private void SetupLocalization()
        {
#if UNITY_2023_1_OR_NEWER
            var locManager = Object.FindFirstObjectByType<FCT.Localization.LocalizationManager>();
#else
            var locManager = Object.FindObjectOfType<FCT.Localization.LocalizationManager>();
#endif
            if (locManager == null)
            {
                GameObject go = new GameObject("LocalizationManager");
                locManager = go.AddComponent<FCT.Localization.LocalizationManager>();
                Debug.Log("[FCT] LocalizationManager added to the scene.");
            }

            string dataPath = EditorPrefs.GetString("FCT_LocAssetPath", "Assets/Resources/Database/LocalizationData.asset");
            
            if (!File.Exists(dataPath))
            {
                string dirPath = Path.GetDirectoryName(dataPath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var data = ScriptableObject.CreateInstance<FCT.Localization.LocalizationData>();
                AssetDatabase.CreateAsset(data, dataPath);
                AssetDatabase.SaveAssets();
                Debug.Log("[FCT] Base LocalizationData created at " + dataPath);
            }
            
            var asset = AssetDatabase.LoadAssetAtPath<FCT.Localization.LocalizationData>(dataPath);
            locManager.data = asset;
            EditorUtility.SetDirty(locManager);

            Selection.activeGameObject = locManager.gameObject;
        }

        private void GenerateStateScript(string stateName, string managerName)
        {
            if (string.IsNullOrWhiteSpace(stateName) || string.IsNullOrWhiteSpace(managerName))
            {
                Debug.LogError("[FCT] State Name and Manager Name cannot be empty.");
                return;
            }

            string folderPath = "Assets/FCT_Samples/States";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string code = $@"using UnityEngine;
using FCT.FSM;

public class {stateName} : IState<{managerName}>
{{
    public void OnEnter({managerName} owner)
    {{
        Debug.Log($""Entered {{this.GetType().Name}}"");
    }}

    public void OnUpdate() {{ }}
    public void OnFixedUpdate() {{ }}
    public void OnLateUpdate() {{ }}

    public void OnExit()
    {{
        Debug.Log($""Exited {{this.GetType().Name}}"");
    }}
}}
";
            string filePath = $"{folderPath}/{stateName}.cs";
            if (File.Exists(filePath))
            {
                Debug.LogWarning($"[FCT] State script {stateName} already exists.");
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(filePath);
                return;
            }

            File.WriteAllText(filePath, code);
            AssetDatabase.Refresh();
            Debug.Log($"[FCT] State {stateName} generated successfully at {filePath}");
        }

        private void CreateInputConfig()
        {
            string path = "Assets/FCT_InputConfig.asset";
            if (File.Exists(path))
            {
                Debug.LogWarning("[FCT] InputConfig already exists at " + path);
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(path);
                return;
            }

            var so = ScriptableObject.CreateInstance<FCT.Device.InputConfigSO>();
            AssetDatabase.CreateAsset(so, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = so;
            Debug.Log("[FCT] InputConfig created successfully at " + path);
        }

        private void CreateSimplePool()
        {
#if UNITY_2023_1_OR_NEWER
            var pool = Object.FindFirstObjectByType<FCT.Gameplay.SimplePool>();
#else
            var pool = Object.FindObjectOfType<FCT.Gameplay.SimplePool>();
#endif
            if (pool != null)
            {
                Debug.LogWarning("[FCT] SimplePool already exists in the scene.");
                Selection.activeGameObject = pool.gameObject;
                return;
            }

            GameObject go = new GameObject("SimplePool");
            go.AddComponent<FCT.Gameplay.SimplePool>();
            Selection.activeGameObject = go;
            Debug.Log("[FCT] SimplePool added to the scene.");
        }

        private void GenerateGameManager()
        {
            string folderPath = "Assets/FCT_Samples";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string code = @"using UnityEngine;
using FCT.Game;
using FCT.FSM;

public class MyGameManager : CoreGameManager<MyGameManager>
{
    protected override void InitializeStates()
    {
        // Example: StateMachine.ChangeState(new MyGameState());
        Debug.Log(""MyGameManager initialized. Ready for custom states!"");
    }
}
";
            string filePath = folderPath + "/MyGameManager.cs";
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, code);
                AssetDatabase.Refresh();
                Debug.Log("[FCT] Example MyGameManager generated at " + filePath);
            }
            else
            {
                Debug.LogWarning("[FCT] MyGameManager already exists.");
            }
        }

        private void SetupCamera()
        {
            System.Type vcamType = System.Type.GetType("Unity.Cinemachine.CinemachineCamera, Unity.Cinemachine");
            if (vcamType == null)
            {
                vcamType = System.Type.GetType("Cinemachine.CinemachineVirtualCamera, Cinemachine");
            }

            if (vcamType == null)
            {
                Debug.LogError("[FCT] Cinemachine not found. Install via Package Manager.");
                return;
            }

            GameObject camGo = GameObject.Find("FCT_MainCamera");
            if (camGo == null) camGo = new GameObject("FCT_MainCamera");

            if (camGo.GetComponent(vcamType) == null)
            {
                camGo.AddComponent(vcamType);
            }

            Selection.activeGameObject = camGo;
            Debug.Log("[FCT] Camera created. Don't forget to assign a target.");
        }
    }
}
