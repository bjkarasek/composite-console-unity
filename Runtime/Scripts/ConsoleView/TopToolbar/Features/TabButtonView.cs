using CompositeArchitecture;
using UnityEngine;
using UnityEngine.UI;

namespace CompositeConsole
{
    public class TabButtonView : MonoCompositeElement
    {
        [SerializeField] public Button Button;

        [SerializeField] private GameObject SelectedImage;

        public void ToggleSelected(bool isSelected)
        {
            SelectedImage.SetActive(isSelected);
        }
    }
}