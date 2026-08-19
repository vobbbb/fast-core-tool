using UnityEngine;
using UnityEngine.Animations;
using Unity.Cinemachine;
using FCT.Device;
using UnityEngine.Assertions;

namespace FCT.Camera
{
    [DefaultExecutionOrder(-100)]
    public class CameraCore : MonoBehaviour
    {
        [Header("Cinemachine")]
        public CinemachineCamera virtualCamera;

        [Header("Settings")]
        public CameraSettings settings;
        public Transform target;
        public bool isEnabled = true;

        private CameraMode _currentMode = CameraMode.TopDown;
        private CinemachineThirdPersonFollow _tpFollow;
        private CinemachineDeoccluder _deoccluder;

        private float _pivotPitch;
        private float _pivotYaw;
        private Transform _proxyTarget;

        private void OnEnable()
        {
            if (settings != null)
            {
                settings.OnSettingsChanged += ApplySettingsToCamera;
            }
        }

        private void OnDisable()
        {
            if (settings != null)
            {
                settings.OnSettingsChanged -= ApplySettingsToCamera;
            }
        }

        private void Start()
        {
            if (settings != null)
            {
                _currentMode = settings.currentMode;
            }

            if (target != null)
            {
                _proxyTarget = new GameObject("CameraProxyTarget").transform;
                _pivotPitch = target.eulerAngles.x;
                _pivotYaw = target.eulerAngles.y;

                SetTarget(target);
            }

            if (virtualCamera != null)
            {
                _tpFollow = virtualCamera.GetComponent<CinemachineThirdPersonFollow>();
                _deoccluder = virtualCamera.GetComponent<CinemachineDeoccluder>();
                
                if (_proxyTarget != null)
                {
                    virtualCamera.Follow = _proxyTarget;
                    virtualCamera.LookAt = null;
                }
            }

            ApplySettingsToCamera();
        }

        public void SetCameraMode(CameraMode mode)
        {
            _currentMode = mode;
            ApplySettingsToCamera();
        }

        public void ToggleCameraMode()
        {
            SetCameraMode(_currentMode == CameraMode.TopDown ? CameraMode.ThirdPerson : CameraMode.TopDown);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;

            if (target != null)
            {
                if (_proxyTarget == null)
                {
                    _proxyTarget = new GameObject("CameraProxyTarget").transform;
                    _pivotPitch = target.eulerAngles.x;
                    _pivotYaw = target.eulerAngles.y;
                }

                _proxyTarget.position = target.position;
                _proxyTarget.rotation = target.rotation;

                if (virtualCamera != null)
                {
                    virtualCamera.Follow = _proxyTarget;
                    virtualCamera.LookAt = null;
                }
            }
        }
        
        private void ApplySettingsToCamera()
        {
            if (settings == null || virtualCamera == null || _tpFollow == null) return;
            
            _currentMode = settings.currentMode;

            if (_currentMode == CameraMode.TopDown)
            {
                _tpFollow.CameraDistance = Mathf.Abs(settings.topDown.offset.z);
                _tpFollow.ShoulderOffset = new Vector3(settings.topDown.offset.x, settings.topDown.offset.y, 0f);
                _tpFollow.Damping = new Vector3(settings.topDown.followDamping, settings.topDown.followDamping, settings.topDown.followDamping);
                
                if (_deoccluder != null)
                {
                    _deoccluder.enabled = settings.topDown.useCollision;
                }

                if (_proxyTarget != null)
                {
                    _proxyTarget.rotation = Quaternion.Euler(settings.topDown.rotation.x, settings.topDown.rotation.y, 0f);
                }
            }
            else if (_currentMode == CameraMode.ThirdPerson)
            {
                _tpFollow.CameraDistance = settings.thirdPerson.distance;
                _tpFollow.ShoulderOffset = settings.thirdPerson.offset;
                _tpFollow.Damping = new Vector3(settings.thirdPerson.followDamping, settings.thirdPerson.followDamping, settings.thirdPerson.followDamping);

                if (_deoccluder != null)
                {
                    _deoccluder.enabled = settings.thirdPerson.useCollision;
                    _deoccluder.CollideAgainst = settings.thirdPerson.collisionMask;
                    
                    var avoidance = _deoccluder.AvoidObstacles;
                    avoidance.CameraRadius = settings.thirdPerson.collisionRadius;
                    _deoccluder.AvoidObstacles = avoidance;
                }
            }
        }

        private void FixedUpdate()
        {
            if (!isEnabled || target == null || _proxyTarget == null || settings == null) return;

            _proxyTarget.position = target.position;
            
            if (_currentMode == CameraMode.ThirdPerson)
            {
                _proxyTarget.rotation = Quaternion.Euler(_pivotPitch, _pivotYaw, 0f);
            }
            else if (_currentMode == CameraMode.TopDown)
            {
                _proxyTarget.rotation = Quaternion.Euler(settings.topDown.rotation.x, settings.topDown.rotation.y, 0f);
            }
        }

        private void LateUpdate()
        {
            if (!isEnabled || target == null || _tpFollow == null || settings == null || _proxyTarget == null) return;

            var input = GameInput.Instance;
            Assert.IsNotNull(input, "GameInput instance is null. Ensure that a GameInput component exists in the scene.");

            if (_currentMode == CameraMode.ThirdPerson)
            {

                float zoomDelta = input.GetVector2("Zoom").y;
                if (zoomDelta != 0)
                {
                    _tpFollow.CameraDistance -= zoomDelta * settings.thirdPerson.zoomSensitivity * 0.01f;
                    _tpFollow.CameraDistance = Mathf.Clamp(_tpFollow.CameraDistance, settings.thirdPerson.minDistance, settings.thirdPerson.maxDistance);
                    settings.thirdPerson.distance = _tpFollow.CameraDistance;
                }

                if (input.GetButton("LookHeld"))
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Confined;

                    Vector2 lookDelta = input.GetVector2("Look");
                    
                    _pivotYaw   += lookDelta.x * settings.thirdPerson.sensitivity.x * 0.5f;
                    _pivotPitch -= lookDelta.y * settings.thirdPerson.sensitivity.y * 0.5f; 

                    _pivotPitch = Mathf.Clamp(
                        _pivotPitch, 
                        settings.thirdPerson.pitchLimits.x, 
                        settings.thirdPerson.pitchLimits.y
                    );
                }
                else
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
    }
}
