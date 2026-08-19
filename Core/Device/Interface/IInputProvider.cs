using UnityEngine;

namespace FCT.Device
{
    public interface IInputProvider
    {
        bool GetButtonDown(string actionName);
        bool GetButton(string actionName);
        bool GetButtonUp(string actionName);
        float GetFloat(string actionName);
        Vector2 GetVector2(string actionName);

        void Tick();
        void Dispose();
    }
}
