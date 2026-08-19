using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace FCT.Device
{
    /// <summary>
    /// Connects the FCT InputProvider interface to Unity's new Input System using dynamic action names.
    /// </summary>
    public sealed class NewInputSystemProvider : IInputProvider
    {
        private InputActionAsset _assetInstance;
        private Dictionary<string, InputAction> _actions = new Dictionary<string, InputAction>();

        public NewInputSystemProvider(InputConfigSO config)
        {
            if (config == null || config.inputAsset == null)
            {
                Debug.LogError("[FCT] InputConfigSO or InputActionAsset is missing in GameInput!");
                return;
            }

            // Instantiate to avoid modifying the original asset data at runtime
            _assetInstance = Object.Instantiate(config.inputAsset);
            _assetInstance.Enable();

            // Store all actions in a dictionary by name for O(1) lookups
            foreach (var action in _assetInstance)
            {
                _actions[action.name] = action;
            }
        }

        public void Tick()
        {
            // Unity's new Input System handles state automatically per-frame
        }

        public bool GetButtonDown(string actionName)
        {
            if (_actions.TryGetValue(actionName, out var action))
                return action.WasPressedThisFrame();
            
            return false;
        }

        public bool GetButton(string actionName)
        {
            if (_actions.TryGetValue(actionName, out var action))
                return action.IsPressed();
            
            return false;
        }

        public bool GetButtonUp(string actionName)
        {
            if (_actions.TryGetValue(actionName, out var action))
                return action.WasReleasedThisFrame();
            
            return false;
        }

        public float GetFloat(string actionName)
        {
            if (_actions.TryGetValue(actionName, out var action))
                return action.ReadValue<float>();
            
            return 0f;
        }

        public Vector2 GetVector2(string actionName)
        {
            if (_actions.TryGetValue(actionName, out var action))
                return action.ReadValue<Vector2>();
            
            return Vector2.zero;
        }

        public void Dispose()
        {
            if (_assetInstance != null)
            {
                _assetInstance.Disable();
                Object.Destroy(_assetInstance);
            }
        }
    }
}
