using FCT.Device;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FCT.Device.UI
{
    /// <summary>
    /// A robust virtual joystick optimized for floating and mobile modes.
    /// It works based on the initial touch position on the screen.
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Configuration")]
        [SerializeField] private float handleRange = 1f;
        [SerializeField] private float deadZone = 0.1f;
        [SerializeField] private bool isFloating = true;
        [SerializeField] private bool hideOnRelease = true;
        
        [Header("UI Components")]
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;

        private Vector2 input = Vector2.zero;
        private Vector2 startPosition = Vector2.zero;
        private Canvas canvas;
        private Vector2 initialBackgroundPos;

        void Awake()
        {
#if !UNITY_ANDROID && !UNITY_IOS
            gameObject.SetActive(false);
#endif
            canvas = GetComponentInParent<Canvas>();
            
            if (background == null) background = GetComponent<RectTransform>();
            if (handle == null && background != null && background.childCount > 0) 
                handle = background.GetChild(0) as RectTransform;

            initialBackgroundPos = background.anchoredPosition;

            var img = GetComponent<Image>();
            if (img == null)
            {
                img = gameObject.AddComponent<Image>();
                img.color = new Color(0, 0, 0, 0); 
            }
            img.raycastTarget = true;

            if (hideOnRelease)
            {
                background.gameObject.SetActive(false);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            startPosition = eventData.position;

            if (isFloating)
            {
                background.gameObject.SetActive(true);
                
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform)transform, 
                    eventData.position, 
                    eventData.pressEventCamera, 
                    out Vector2 localPoint);
                
                background.anchoredPosition = localPoint;
            }
            else if (hideOnRelease)
            {
                background.gameObject.SetActive(true);
            }

            handle.anchoredPosition = Vector2.zero;
            input = Vector2.zero;
            UpdateInputSystem();
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 diff = eventData.position - startPosition;
            float radius = (background.sizeDelta.x / 2) * canvas.scaleFactor;
            
            input = diff / radius;

            if (input.magnitude > 1f)
            {
                input = input.normalized;
            }

            if (input.magnitude < deadZone)
            {
                input = Vector2.zero;
            }

            float visualRadius = background.sizeDelta.x / 2;
            handle.anchoredPosition = input * visualRadius * handleRange;

            UpdateInputSystem();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
            UpdateInputSystem();

            if (hideOnRelease)
            {
                background.gameObject.SetActive(false);
            }

            if (!isFloating)
            {
                background.anchoredPosition = initialBackgroundPos;
            }
        }

        private void UpdateInputSystem()
        {
            if (GameInput.Instance != null)
            {
                GameInput.Instance.VirtualJoystickValue = input;
            }
        }
    }
}
