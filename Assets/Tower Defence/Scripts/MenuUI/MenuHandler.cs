using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

namespace Menu
{
    public static class TheMenuHandler
    {
        public static MenuHandler theMenuHandler;
    }

    public class MenuHandler : MonoBehaviour
    {
        [Header("-- Scene Name Variables --")]
        [SerializeField]
        private string menuSceneName = "Menu";
        [SerializeField]
        private string gameSceneName = "Main";

        [Header("-- Resolution Settings --")]
        public TMP_Dropdown resolutionDropdown;
        private Resolution[] resolutions;

        [Header("-- Audio Objects --")]
        [SerializeField]
        private AudioMixer sFXAudio;
        [SerializeField]
        private AudioMixer musicAudio;

        [Header("-- Menu Objects --")]
        [SerializeField]
        private GameObject OptionsMenu;
        [SerializeField]
        private GameObject HomeMenu;

        private bool paused = false;

        #region Pausing
        public void Pause()
        {
            Time.timeScale = 0;
            if (HomeMenu)
                HomeMenu.SetActive(true);
            paused = true;
        }

        public void Unpause()
        {
            paused = false;
            if (HomeMenu)
                HomeMenu.SetActive(false);
            Time.timeScale = 1;
        }

        public void MenuGoBack()
        {
            if (OptionsMenu.activeInHierarchy)
                CloseOptionsMenu();
            else
            {
                if (!paused)
                    Pause();
                else
                    Unpause();
            }
        }
        #endregion

        #region Options

        public void ToggleOptionsMenu()
        {
            if (HomeMenu.activeInHierarchy && !OptionsMenu.activeInHierarchy)
            {
                OpenOptionsMenu();
            }
            else if (!HomeMenu.activeInHierarchy && OptionsMenu.activeInHierarchy)
            {
                CloseOptionsMenu();
            }
        }

        public void OpenOptionsMenu()
        {
            if (OptionsMenu && HomeMenu)
            {
                OptionsMenu.SetActive(true);
                HomeMenu.SetActive(false);
            }
        }

        public void CloseOptionsMenu()
        {
            if (OptionsMenu && HomeMenu)
            {
                OptionsMenu.SetActive(false);
                HomeMenu.SetActive(true);
            }
        }

        #region Volume
        public void ChangeSFXVolume(float _volume)
        {
            if (sFXAudio)
            {
                sFXAudio.SetFloat("volume", _volume);
                PlayerPrefs.SetFloat("SFXVolume", _volume);
                PlayerPrefs.Save();
            }
            else Debug.Log(_volume);
        }

        public void ChangeMusicVolume(float _volume)
        {
            if (musicAudio)
            {
                musicAudio.SetFloat("volume", _volume);
                PlayerPrefs.SetFloat("MusicVolume", _volume);
                PlayerPrefs.Save();
            }
            else Debug.Log(_volume);
        }

        public void SetMute(bool isMuted)
        {
            if (musicAudio && sFXAudio)
            {
                if (isMuted)
                {
                    musicAudio.SetFloat("isMutedVolume", -80);
                    sFXAudio.SetFloat("isMutedVolume", -80);
                    PlayerPrefs.SetInt("Muted", 1);
                    PlayerPrefs.Save();
                }
                else
                {
                    musicAudio.SetFloat("isMutedVolume", 0);
                    sFXAudio.SetFloat("isMutedVolume", 0);
                    PlayerPrefs.SetInt("Muted", 0);
                    PlayerPrefs.Save();
                }
            }
            else Debug.Log("IsMuted: " + isMuted);
        }
        #endregion

        #region Resolution
        public void SetResolution(int ResolutionIndex)
        {
            Resolution res = resolutions[ResolutionIndex];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            if (Application.isEditor)
            {
                Screen.fullScreen = isFullscreen;
            }
        }

        #endregion

        #endregion

        #region SceneSwitching
        public void StartGame() => SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        public void ReturnToMainMenu() => SceneManager.LoadScene(menuSceneName, LoadSceneMode.Single);
        #endregion

        #region Initialization
        private void InitializeResolutions()
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            List<string> resolutionOptions = new List<string>();
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + "x" + resolutions[i].height;
                resolutionOptions.Add(option);
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    //this was previously currentResolutionIndex = 1;, which would set it to the lowest resolution every time you went to the main menu - John.
                    currentResolutionIndex = i;
                }
            }
            resolutionDropdown.AddOptions(resolutionOptions);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        private void InitializeVolume()
        {
            if (PlayerPrefs.HasKey("MusicVolume"))
                ChangeMusicVolume(PlayerPrefs.GetFloat("Volume"));
            if (PlayerPrefs.HasKey("SFXVolume"))
                ChangeSFXVolume(PlayerPrefs.GetFloat("Volume"));
            if (PlayerPrefs.HasKey("IsMuted"))
                SetMute(PlayerPrefs.GetInt("IsMuted") != 0);
        }
        #endregion

        #region Saving
        public void Save()
        {
            Debug.Log("Not Yet Implemented");
        }

        public void Load()
        {
            Debug.Log("Not Yet Implemented");
        }

        #endregion

        public void Quit()
        {
            Time.timeScale = 1;
            if (gameObject.scene.name == gameSceneName)
                ReturnToMainMenu();
            else
            {
                Application.Quit();
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
                MenuGoBack();
        }

        private void Awake()
        {
            InitializeResolutions();
            InitializeVolume();
            TheMenuHandler.theMenuHandler = this;
        }
    }
}
