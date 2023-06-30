using CompositeArchitecture;
using UnityEngine;
using UnityEngine.EventSystems;
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
        
        private ResizeController _resizeController;

        private bool _isResizing = false;
        private bool _isHovering = false;

        private RectTransform _rectTransform;

        protected override void OnInstall(DependencyInjectionContainer container)
        {
            _rectTransform = GetComponent<RectTransform>();
        }
        
        public void SetInitialPosition(Vector2 pos)
        {
            _rectTransform.anchoredPosition = pos;
        }

        protected override void OnRefresh()
        {
            if (_isResizing)
            {
                var position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                position.x = Mathf.Clamp(position.x, 0, Screen.width);
                position.y = Mathf.Clamp(position.y, 10, Screen.height - 100);
                _rectTransform.anchoredPosition = new Vector2(MoveX ? position.x : 0, MoveY ? position.y : 0);
                OnResizing.Invoke(position);
            }
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