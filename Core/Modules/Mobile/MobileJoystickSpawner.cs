using UnityEngine;

namespace FCT.Device.Mobile
{
    /// <summary>
    /// Spawns the mobile joystick UI when on a mobile platform.
    /// </summary>
    public class MobileJoystickSpawner : MonoBehaviour
    {
        [SerializeField] private Transform canvasContent;
        [SerializeField] private GameObject joystick;

        void Start()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (joystick != null && canvasContent != null)
            {
                GameObject joystickPad = Instantiate(joystick, canvasContent, false);
                joystickPad.SetActive(true);
            }
#endif
        }
    }
}
