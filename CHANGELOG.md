# Changelog
All notable changes to the Fast Core Tool (FCT) package will be documented in this file.

## [1.0.0] - Initial Release
### Added
- **FCT Core Wizard:** A centralized Editor Hub for project initialization (`FCT -> Core Wizard`).
- **Interactive Quick Tutorial:** Step-by-step guided setup process for new developers.
- **FCTSingleton<T>:** Generic, thread-safe base class for all manager singletons to eliminate boilerplate.
- **Generic Input System:** Dictionary-based Input wrapper (`GameInput`) compatible with Unity's New Input System.
- **InputConfigSO Editor:** Custom inspector with a "Sync Actions" button for automatic input binding registration.
- **Localization System:** Google Sheets CSV syncing integration, saving data directly to a local `LocalizationData.asset`.
- **LocalizeText Editor:** Custom UI text inspector featuring a smart dropdown for translation key selection.
- **State Machine (FSM):** Boilerplate state machine generators and live debugger in the Wizard.
- **SimplePool:** Optimized object pooling manager with pre-warming capabilities.
