using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace Menu
{
    public enum MenuToggleType
    {
        Fullscreen,
        Mute
    }

    public class MenuToggle : MonoBehaviour
    {
        [SerializeField]
        private MenuToggleType toggleType = MenuToggleType.Fullscreen;

        [SerializeField, HideInInspector]
        private Toggle toggle;
        [SerializeField, HideInInspector]
        private TextMeshProUGUI toggleText;

        private MenuHandler menuHandler;

        private void OnValidate()
        {
            if (!toggle)
                toggle = GetComponent<Toggle>();
            if (!toggleText)
                toggleText = GetComponentInChildren<TextMeshProUGUI>();

            toggleText.text = toggleType.ToString().Replace("_", " ");
            toggle.onValueChanged.RemoveAllListeners();
        }

        private void Start()
        {
            menuHandler = TheMenuHandler.theMenuHandler;
            toggle.onValueChanged.AddListener(delegate { PerformFunction( toggle ); } );
        }

        private void PerformFunction(bool _value)
        {
            switch (toggleType)
            {
                case MenuToggleType.Fullscreen:
                    menuHandler.SetFullscreen(_value);
                    break;
                case MenuToggleType.Mute:
                    menuHandler.SetMute(_value);
                    break;
                default:
                    break;
            }
        }
    }
}