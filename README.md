# Fast Core Tool (FCT)

FCT is a professional, modular framework for Unity that jumpstarts your game development by handling boilerplate systems like Inputs, State Machines, Localization, and Object Pooling.

## Getting Started

### 1. Open the Hub
Navigate to the top Unity menu and click **`FCT -> Core Wizard`**. This is your central command center.

### 2. Initialize Your Scene
In the **Setup Hub** tab of the wizard, you will find 5 setup buttons. Click them to automatically inject the necessary managers into your active scene:
- **Setup SimplePool**: Instantiates the object pooling manager.
- **Generate GameManager**: Creates a boilerplate State Machine script ready for your game logic.
- **Setup Localization**: Creates the `LocalizationManager` and its required database file.

### 3. Configure Inputs
1. Double-click your `.inputactions` file and define your actions (e.g., "Jump", "Fire").
2. Create an `InputConfig` (Right click in Project -> Create -> FCT -> Input Config).
3. Assign your `.inputactions` file to it and click **"Sync Actions"**.
4. In your code, read inputs easily: `GameInput.Instance.GetButtonDown("Jump")`.

### 4. Configure Localization (Google Sheets)
1. In the FCT Wizard, go to the **Config** tab.
2. Paste your Google Sheets CSV Export URL.
3. Click **"Sync Now"**. All your spreadsheet translations will be downloaded and saved into the project.
4. Add the `LocalizeText` component to any TextMeshPro UI element and pick your translation key from the dropdown!

## Requirements
- Unity 2022.3 or newer.
- TextMeshPro (included in Unity by default).
- Unity New Input System package.
- Cinemachine (optional, for camera setups).
