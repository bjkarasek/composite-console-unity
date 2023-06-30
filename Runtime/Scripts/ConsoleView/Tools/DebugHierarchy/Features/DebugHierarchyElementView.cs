using CompositeArchitecture;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CompositeConsole
{
    public class DebugHierarchyElementView : MonoCompositeElement, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Observable<DebugHierarchyElementView> OnClicked = new();

        [SerializeField] public RectTransform Container;
        [SerializeField] public TextMeshProUGUI Text;
        [SerializeField] public Image Background;
        [SerializeField] public Button ToggleExpandButton;
        
        [SerializeField] private Sprite ExpandedSprite;
        [SerializeField] private Sprite CollapsedSprite;


        [SerializeField] private Color RegularColor;
        [SerializeField] private Color HoveredColor;
        [SerializeField] private Color SelectedColor;
        [SerializeField] private float Margin = 50;

        [SerializeField] private Image Icon;

        [SerializeField] private Sprite SceneSprite;
        [SerializeField] private Sprite GameObjectSprite;
        
        public HierarchyNode RepresentedObject;

        private bool _isExpanded;
        private int _parentsCollapsedCount;
        
        private bool _isSelected;
        private bool _isHovered;

        public void BeforeInstallSetup(HierarchyNode representedObject)
        {
            RepresentedObject = representedObject;
            representedObject.View = this;
            Icon.sprite = representedObject.Scene.HasValue ? SceneSprite : GameObjectSprite;
            _isExpanded = true;
        }
        
        protected override void OnActivate()
        {
            ToggleExpandButton.onClick.AddListener(ToggleExpand);
            
            if (RepresentedObject != null)
            {
                ToggleExpandButton.gameObject.SetActive(RepresentedObject.Children.Count > 0);

                var anchoredPosition = Container.anchoredPosition;
                anchoredPosition = new Vector2(anchoredPosition.x + Margin * RepresentedObject.CalculateDepth(), anchoredPosition.y);
                Container.anchoredPosition = anchoredPosition;
            }
        }

        private void ToggleExpand()
        {
            _isExpanded = !_isExpanded;
            ToggleExpandButton.image.sprite = _isExpanded ? ExpandedSprite : CollapsedSprite;
            UpdateExpand(_parentsCollapsedCount);
        }

        private void UpdateExpand(int parentsCollapsed)
        {
            _parentsCollapsedCount = parentsCollapsed;
            gameObject.SetActive(parentsCollapsed == 0);

            var nextParentsCollapsed = parentsCollapsed + (_isExpanded ? 0 : 1);

            if (RepresentedObject != null)
            {
                foreach (var child in RepresentedObject.Children)
                {
                    child.View.UpdateExpand(nextParentsCollapsed);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClicked.Invoke(this);
        }

        public void Select()
        {
            _isSelected = true;
            UpdateColor();
        }

        public void Unselect()
        {
            _isSelected = false;
            UpdateColor();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            UpdateColor();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            UpdateColor();
        }

        private void UpdateColor()
        {
            if (_isSelected)
            {
                Background.color = SelectedColor;
            }
            else if (_isHovered)
            {
                Background.color = HoveredColor;
            }
            else
            {
                Background.color = RegularColor;
            }
        }

        protected override void OnDeactivate()
        {
            ToggleExpandButton.onClick.RemoveListener(ToggleExpand);
        }
    }
}