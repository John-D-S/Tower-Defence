using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace UserInterface
{
    public enum MenuButtonType
    {
        Start,
        Return,
        LoadGame,
        SaveGame,
        Options,
        MainMenu,
        SaveAndQuit
    }

    [CreateAssetMenu(fileName = "MenuButtonSettings", menuName = "Menu", order = 100)]
    public class MenuButtonSettings: ScriptableObject
    {
        [Header("Scene Name Variables")]
        [SerializeField]
        private string menuSceneName = "Menu";
        public string MenuSceneName { get { return menuSceneName; } set { } }
        [SerializeField]
        private string gameSceneName = "Main";
        public string GameSceneName { get { return gameSceneName; } set { } }

    }

    public class MenuButton : MonoBehaviour
    {
        [SerializeField]
        private MenuButtonType buttonType = MenuButtonType.Start;
        [SerializeField, HideInInspector]
        private Button button;
        [SerializeField, HideInInspector]
        private TextMeshProUGUI buttonText;
        private MenuButtonSettings heyoo;


        private void OnValidate()
        {
            if (!button)
                button = GetComponent<Button>();
            if (!buttonText)
                buttonText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SwitchScene(string _SceneName)
        {
            string text = heyoo.th
        }


    }
}
