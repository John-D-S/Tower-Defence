using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace Menu
{
    public enum MenuSliderType
    {
        Music,
        Sound_Effects    
    }

    public class MenuSlider : MonoBehaviour
    {
        [SerializeField]
        private MenuSliderType sliderType = MenuSliderType.Music;

        [SerializeField, HideInInspector]
        private Slider slider;
        [SerializeField, HideInInspector]
        private TextMeshProUGUI sliderText;

        private MenuHandler menuHandler;

        private void OnValidate()
        {
            if (!slider)
                slider = GetComponent<Slider>();
            if (!sliderText)
                sliderText = GetComponentInChildren<TextMeshProUGUI>();

            sliderText.text = sliderType.ToString().Replace("_", " ");
            slider.onValueChanged.RemoveAllListeners();
        }

        private void Start()
        {
            menuHandler = TheMenuHandler.theMenuHandler;
            slider.onValueChanged.AddListener(PerformFunction);
        }

        private void PerformFunction(float _value)
        {
            switch (sliderType)
            {
                case MenuSliderType.Music:
                    menuHandler.ChangeMusicVolume(_value);
                    break;
                case MenuSliderType.Sound_Effects:
                    menuHandler.ChangeSFXVolume(_value);
                    break;
                default:
                    break;
            }
        }
    }
}