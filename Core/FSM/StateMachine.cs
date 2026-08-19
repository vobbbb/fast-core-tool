using UnityEngine;

namespace FCT.FSM
{
    /// <summary>
    /// Generic state interface that knows its owner.
    /// </summary>
    public interface IState<T>
    {
        void OnEnter(T owner);
        void OnUpdate();
        void OnFixedUpdate();
        void OnLateUpdate();
        void OnExit();
    }

    /// <summary>
    /// Generic state machine, scalable and safe.
    /// </summary>
    public class StateMachine<T>
    {
        public IState<T> CurrentState { get; private set; }
        private T _owner;

        public StateMachine(T owner)
        {
            _owner = owner;
        }

        public void ChangeState(IState<T> newState)
        {
            if (newState == CurrentState || newState == null)
                return;

            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState?.OnEnter(_owner);
        }

        public void Update() => CurrentState?.OnUpdate();
        public void FixedUpdate() => CurrentState?.OnFixedUpdate();
        public void LateUpdate() => CurrentState?.OnLateUpdate();
    }
}
