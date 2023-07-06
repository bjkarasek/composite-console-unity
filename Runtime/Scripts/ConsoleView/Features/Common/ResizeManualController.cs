using CompositeArchitecture;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace CompositeConsole
{
    public class ResizeManualController : MonoCompositeElement, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public Observable<Vector2> OnResizing = new();
        
        [SerializeField] private Image Image;
        
        [SerializeField] private Color NormalColor;
        [SerializeField] private Color HoveringColor;
        [SerializeField] private Color ResizingColor;

        [SerializeField] private bool MoveX;
        [SerializeField] private bool MoveY;
        
        [SerializeField] private RectTransform Container;
        
        private ResizeController _resizeController;

        private bool _isResizing = false;
        private bool _isHovering = false;

        private RectTransform _rectTransform;
        private Canvas _canvas;

        private Vector2 _scaledPosition;

        protected override void OnInstall(DependencyInjectionContainer container)
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        protected override void OnInject()
        {
            Resolve(out _canvas);
        }

        public void SetInitialPosition(Vector2 mousePos)
        {
            SetAnchoredPosition(mousePos);
        }

        protected override void OnRefresh()
        {
            if (_isResizing)
            {
                var position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                position.x = Mathf.Clamp(position.x, 0, Screen.width);
                var scaleFactor = _canvas.scaleFactor;
                position.y = Mathf.Clamp(position.y, 10 * scaleFactor, Screen.height - 150 * scaleFactor);

                SetAnchoredPosition(position);
                
                OnResizing.Invoke(position);
            }
            else
            {
                var mousePosition = new Vector2(_scaledPosition.x * Screen.width, _scaledPosition.y * Screen.height);
                SetAnchoredPosition(mousePosition);
            }
        }
        
        private void SetAnchoredPosition(Vector2 mousePosition)
        {
            _scaledPosition = new Vector2(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
            mousePosition.x = Mathf.Clamp(mousePosition.x, 0, Screen.width);
            mousePosition.y = Mathf.Clamp(mousePosition.y, 10, Screen.height - 150);

            var size = Container.rect.size;
            _rectTransform.anchoredPosition = new Vector2(MoveX ? mousePosition.x / Screen.width * size.x : 0, MoveY ? mousePosition.y / Screen.height * size.y : 0);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isResizing = false;
            UpdateColor();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _isResizing = true;
            UpdateColor();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;
            UpdateColor();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;
            UpdateColor();
        }

        private void UpdateColor()
        {
            if (_isResizing)
            {
                Image.color = ResizingColor;
            }
            else if (_isHovering)
            {
                Image.color = HoveringColor;
            }
            else
            {
                Image.color = NormalColor;
            }
        }
    }
}