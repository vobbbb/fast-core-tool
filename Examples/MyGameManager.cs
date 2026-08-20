using UnityEngine;
using FCT.Game;
using FCT.FSM;

namespace FCT.Examples
{
    public class MyGameManager : CoreGameManager<MyGameManager>
    {
        protected override void InitializeStates()
        {
            // Example: StateMachine.ChangeState(new MyGameState());
            Debug.Log("MyGameManager initialized. Ready for custom states!");
        }
    }
}
