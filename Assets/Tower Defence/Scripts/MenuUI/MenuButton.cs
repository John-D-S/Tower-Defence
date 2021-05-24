using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace Menu
{
    public enum MenuButtonType
    {
        Start,
        Return,
        Load_Game,
        Save_Game,
        Options,
        Save_And_Quit,
        Quit
    }

    public class MenuButton : MonoBehaviour
    {
        [SerializeField]
        private MenuButtonType buttonType = MenuButtonType.Start;
        
        [SerializeField, HideInInspector]
        private Button button;
        [SerializeField, HideInInspector]
        private TextMeshProUGUI toggleText;

        private MenuHandler menuHandler;

        private void OnValidate()
        {
            if (!button)
                button = GetComponent<Button>();
            if (!toggleText)
                toggleText = GetComponentInChildren<TextMeshProUGUI>();

            toggleText.text = buttonType.ToString().Replace("_", " ");
            button.onClick.RemoveAllListeners();
        }

        #region FindParentObjectWithTag()
        GameObject FindParentObjectWithTag(string _tag)
        {
            if (transform.parent == null)
                return null;
            else if (transform.parent.tag == _tag)
                return transform.parent.gameObject;
            else
                return FindParentObjectWithTag(_tag, transform.parent);
        }
        
        GameObject FindParentObjectWithTag(string _tag, Transform _transform)
        {
            if (_transform.parent == null)
                return null;
            else if (_transform.parent.tag == _tag)
                return _transform.parent.gameObject;
            else
                return FindParentObjectWithTag(_tag, _transform.parent);
        }
        #endregion

        private void Start()
        {
            menuHandler = TheMenuHandler.theMenuHandler;
            button.onClick.AddListener(PerformFunction);
        }

        private void PerformFunction()
        {
            switch (buttonType)
            {
                case MenuButtonType.Start:
                    menuHandler.StartGame();
                    break;
                case MenuButtonType.Return:
                    menuHandler.MenuGoBack();
                    break;
                case MenuButtonType.Load_Game:
                    menuHandler.Load();
                    break;
                case MenuButtonType.Save_Game:
                    menuHandler.Save();
                    break;
                case MenuButtonType.Options:
                    menuHandler.OpenOptionsMenu();
                    break;
                case MenuButtonType.Save_And_Quit:
                    menuHandler.Save();
                    menuHandler.Quit();
                    break;
                case MenuButtonType.Quit:
                    menuHandler.Quit();
                    break;
                default:
                    break;
            }
        }
    }
}