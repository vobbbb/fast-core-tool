using UnityEngine;

namespace FCT.Device
{
    /// <summary>
    /// Centralized input hub for the FCT framework.
    /// Provides dynamic access to all active input values and handles input provider switching.
    /// </summary>
    public class GameInput : FCT.Utils.FCTSingleton<GameInput>
    {
        [Header("FCT Input Settings")]
        [Tooltip("The ScriptableObject mapping the InputActionAsset.")]
        [SerializeField] private InputConfigSO inputConfig;

        private IInputProvider _provider;
        
        public Vector2 VirtualJoystickValue { get; set; } = Vector2.zero;

        protected override void Awake()
        {
            base.Awake();
            if (this != Instance) return;

            SetProvider(new NewInputSystemProvider(inputConfig));
        }

        private void Update()
        {
            _provider?.Tick();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _provider?.Dispose();
            _provider = null;
        }

        public void SetProvider(IInputProvider newProvider)
        {
            _provider?.Dispose();
            _provider = newProvider;
        }

        public IInputProvider GetProvider() => _provider;

        // ---- Generic Accessors ---- //

        public bool GetButtonDown(string actionName) => _provider?.GetButtonDown(actionName) ?? false;
        public bool GetButton(string actionName) => _provider?.GetButton(actionName) ?? false;
        public bool GetButtonUp(string actionName) => _provider?.GetButtonUp(actionName) ?? false;
        public float GetFloat(string actionName) => _provider?.GetFloat(actionName) ?? 0f;
        
        public Vector2 GetVector2(string actionName) 
        {
            // Inject Virtual Joystick if we're asking for "Move"
            if (actionName.Equals("Move", System.StringComparison.OrdinalIgnoreCase) && VirtualJoystickValue.sqrMagnitude > 0.01f)
            {
                return VirtualJoystickValue;
            }
            return _provider?.GetVector2(actionName) ?? Vector2.zero;
        }
    }
}
