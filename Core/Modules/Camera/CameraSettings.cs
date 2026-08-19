using UnityEngine;
using System;

namespace FCT.Camera
{
    public enum CameraMode { TopDown, ThirdPerson }

    [Serializable]
    public struct CameraViewSettings
    {
        [Header("Position/Follow")]
        public Vector3 offset;
        public float distance;
        public float followDamping;
        
        [Header("Rotation")]
        public Vector3 rotation; 
        public Vector2 sensitivity; 
        public Vector2 pitchLimits; 

        [Header("Collision")]
        public bool useCollision;
        public float collisionRadius;
        public LayerMask collisionMask;

        [Header("Zoom")]
        public float zoomSensitivity;
        public float minDistance;
        public float maxDistance;
    }

    [CreateAssetMenu(fileName = "CameraSettings", menuName = "FCT/Camera/Settings")]
    public class CameraSettings : ScriptableObject
    {
        public Action OnSettingsChanged;

        public CameraMode currentMode = CameraMode.TopDown;

        public CameraViewSettings topDown = new CameraViewSettings
        {
            offset = new Vector3(0, 13, -7),
            rotation = new Vector3(65, 0, 0),
            followDamping = 0.5f,
            useCollision = false
        };

        public CameraViewSettings thirdPerson = new CameraViewSettings
        {
            offset = new Vector3(0, 1.5f, 0),
            distance = 5.0f,
            sensitivity = new Vector2(0.3f, 0.3f),
            pitchLimits = new Vector2(-40f, 80f),
            useCollision = true,
            collisionRadius = 0.5f,
            zoomSensitivity = 1.0f,
            minDistance = 2.0f,
            maxDistance = 15.0f,
            followDamping = 0.1f
        };

        private void OnValidate()
        {
            OnSettingsChanged?.Invoke();
        }
    }
}
