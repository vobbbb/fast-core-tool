using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace FCT.Device
{
    /// <summary>
    /// ScriptableObject container that references a master InputActionAsset.
    /// It dynamically registers all actions for use in the FCT generic Input system.
    /// </summary>
    [CreateAssetMenu(fileName = "FCT_InputConfig", menuName = "FCT/Input Config")]
    public class InputConfigSO : ScriptableObject
    {
        [Tooltip("The Unity InputActionAsset file containing your Action Maps and Actions.")]
        public InputActionAsset inputAsset;
        
        [HideInInspector]
        public List<string> registeredActions = new List<string>();
    }
}
