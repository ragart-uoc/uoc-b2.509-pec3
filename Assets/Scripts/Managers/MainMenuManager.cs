using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PEC3.Managers
{
    /// <summary>
    /// Class <c>MainMenuManager</c> contains the methods and properties needed for the main menu.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        /// <value>Property <c>Instance</c> represents the class instance.</value>
        private static MainMenuManager Instance;
        
        /// <value>Property <c>mainMenuOptions</c> represents the UI element containing the main menu options.</value>
        public GameObject mainMenuOptions;

        /// <value>Property <c>mainMenuThemeSelect</c> represents the UI element containing the theme selection.</value>
        public TextMeshProUGUI mainMenuThemeSelect;
        
        /// <value>Property <c>mainMenuLevels</c> represents the UI element containing the level selection.</value>
        public GameObject mainMenuLevelSelect;

        /// <value>Property <c>mainMenuDefaultLevels</c> represents the UI element containing the default levels.</value>
        public GameObject mainMenuDefaultLevels;
        
        /// <value>Property <c>mainMenuCustomLevels</c> represents the UI element containing the custom levels.</value>
        public GameObject mainMenuCustomLevels;

        /// <value>Property <c>levelButtonPrefab</c> represents the level button prefab.</value>
        public GameObject levelButtonPrefab;

        /// <summary>
        /// Method <c>Awake</c> is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        
        /// <summary>
        /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            FillLevelList();
            ChangeThemeName();
        }
        
        /// <summary>
        /// Method <c>FillLevelList</c> fills the level list.
        /// </summary>
        private void FillLevelList()
        {
            // Destroy existing level buttons
            foreach (Transform child in mainMenuDefaultLevels.transform)
                Destroy(child.gameObject);
            foreach (Transform child in mainMenuCustomLevels.transform)
                Destroy(child.gameObject);
            
            // Create the default level buttons
            foreach (var level in GlobalGameManager.Instance.defaultLevels)
            {
                var levelButton = Instantiate(levelButtonPrefab, mainMenuDefaultLevels.transform);
                levelButton.GetComponent<TextMeshProUGUI>().text = level;
                levelButton.GetComponent<Button>().onClick.AddListener(() => PlayLevel(level));
            }
            
            // Create the custom level buttons
            foreach (var level in GlobalGameManager.Instance.customLevels)
            {
                var levelButton = Instantiate(levelButtonPrefab, mainMenuCustomLevels.transform);
                levelButton.GetComponent<TextMeshProUGUI>().text = level;
                levelButton.GetComponent<Button>().onClick.AddListener(() => PlayLevel(level));
            }
        }
        
        /// <summary>
        /// Method <c>PlayDefaultLevels</c> loads the default levels.
        /// </summary>
        public void PlayDefaultLevels()
        {
            GlobalGameManager.Instance.levelPlayList = GlobalGameManager.Instance.defaultLevels;
            SceneManager.LoadScene(GlobalGameManager.Instance.GetCurrentThemeScenePath());
        }
        
        /// <summary>
        /// Method <c>ToggleLevelSelection</c> toggles the level selection.
        /// </summary>
        public void ToggleLevelSelection()
        {
            mainMenuOptions.SetActive(!mainMenuOptions.activeSelf);
            mainMenuLevelSelect.SetActive(!mainMenuLevelSelect.activeSelf);
        }
        
        /// <summary>
        /// Method <c>PlayLevel</c> plays a particular level.
        /// </summary>
        /// <param name="levelName">The name of the level to play.</param>
        public void PlayLevel(string levelName)
        {
            GlobalGameManager.Instance.ClearLevelPlayList();
            GlobalGameManager.Instance.AddLevelToPlayList(levelName);
            SceneManager.LoadScene(GlobalGameManager.Instance.GetCurrentThemeScenePath());
        }
        
        /// <summary>
        /// Method <c>ToggleThemeSelection</c> toggles the theme selection.
        /// </summary>
        public void ToggleThemeSelection()
        {
            var themeName = GlobalGameManager.Instance.NextTheme();
            ChangeThemeName();
        }
        
        /// <summary>
        /// Method <c>ChangeThemeName</c> changes the theme name in the UI.
        /// </summary>
        private void ChangeThemeName()
        {
            mainMenuThemeSelect.text = GlobalGameManager.Instance.GetCurrentThemeName();
        }
    }
}
