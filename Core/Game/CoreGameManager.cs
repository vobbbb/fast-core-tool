using UnityEngine;
using FCT.FSM;

namespace FCT.Game
{
    public interface ICoreGameManagerDebug
    {
        string ManagerName { get; }
        string CurrentStateName { get; }
    }

    /// <summary>
    /// Abstract base class for the Game Manager.
    /// Manages the Singleton instance and the main StateMachine.
    /// Derived classes must implement InitializeStates() to register their specific flow states.
    /// </summary>
    public abstract class CoreGameManager<T> : FCT.Utils.FCTSingleton<T>, ICoreGameManagerDebug where T : CoreGameManager<T>
    {
        public string ManagerName => typeof(T).Name;
        public string CurrentStateName => _stateMachine?.CurrentState?.GetType().Name ?? "None";

        protected StateMachine<T> _stateMachine;
        public StateMachine<T> StateMachine => _stateMachine;

        protected override void Awake()
        {
            base.Awake();
            if (this != Instance) return;

            DontDestroyOnLoad(gameObject);

            _stateMachine = new StateMachine<T>(Instance);
            InitializeStates();
        }

        protected virtual void Update()
        {
            _stateMachine?.Update();
        }

        protected virtual void FixedUpdate()
        {
            _stateMachine?.FixedUpdate();
        }

        protected virtual void LateUpdate()
        {
            _stateMachine?.LateUpdate();
        }

        /// <summary>
        /// Called during Awake to initialize and set the initial state of the StateMachine.
        /// </summary>
        protected abstract void InitializeStates();
    }
}
