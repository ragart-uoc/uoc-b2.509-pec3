using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PEC3.Managers
{
    /// <summary>
    ///  Class <c>GlobalGameManager</c> contains the methods and properties needed for the game management.
    /// </summary>
    public class GlobalGameManager : MonoBehaviour
    {
        /// <value>Property <c>Instance</c> represents the class instance.</value>
        public static GlobalGameManager Instance;
        
        /// <value>Property <c>mainMenuScene</c> represents the main menu scene name.</value>
        public string mainMenuScene = "MainMenu";
        
        /// <value>Property <c>mainMenuScene</c> represents the main menu scene name.</value>
        public string gameScene = "Game";

        /// <value>Property <c>defaultLevels</c> represents the list of default levels.</value>
        public List<string> defaultLevels = new List<string>();
        
        /// <value>Property <c>customLevels</c> represents the list of default levels.</value>
        public List<string> customLevels = new List<string>();
        
        /// <value>Property <c>levelPlayList</c> represents the list of levels to play.</value>
        public List<string> levelPlayList = new List<string>();
        
        /// <value>Property <c>currentLevelIndex</c> represents the current level index.</value>
        public int currentLevelIndex;
        
        /// <value>Property <c>themes</c> represents the list of themes.</value>
        public List<string> themes = new List<string>();
        
        /// <value>Property <c>theme</c> represents the current theme.</value>
        public int currentThemeIndex;

        /// <summary>
        /// Method <c>Awake</c> is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
            
            GetDefaultLevels();
            GetThemes();
        }

        /// <summary>
        /// Method <c>OnEnable</c> is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            GetCustomLevels();
        }
        
        /// <summary>
        /// Method <c>GetDefaultLevels</c> gets the default levels.
        /// </summary>
        private void GetDefaultLevels()
        {
            // Check levels in resources folder
            var levelMaps = Resources.LoadAll("Levels", typeof(TextAsset));
            foreach (var levelMap in levelMaps)
            {
                defaultLevels.Add(levelMap.name);
            }
        }
        
        /// <summary>
        /// Method <c>GetCustomLevels</c> gets the custom levels.
        /// </summary>
        private void GetCustomLevels()
        {
            // Check levels in persistent data path
            var levelMaps = Directory.GetFiles(Application.persistentDataPath, "*.json");
            foreach (var levelMap in levelMaps)
            {
                customLevels.Add(levelMap);
            }
        }
        
        /// <summary>
        /// Method <c>AddLevelToPlayList</c> adds a level to the play list.
        /// </summary>
        public void AddLevelToPlayList(string levelName)
        {
            levelPlayList.Add(levelName);
        }
        
        /// <summary>
        /// Method <c>RemoveLevelFromPlayList</c> removes a level from the play list.
        /// </summary>
        public void RemoveLevelFromPlayList(string levelName)
        {
            levelPlayList.Remove(levelName);
        }
        
        /// <summary>
        /// Method <c>ClearLevelPlayList</c> clears the play list.
        /// </summary>
        public void ClearLevelPlayList()
        {
            levelPlayList.Clear();
        }
        
        /// <summary>
        /// Method <c>NextLevel</c> gets the next level.
        /// </summary>
        public int NextLevel()
        {
            currentLevelIndex = (currentLevelIndex + 1) % levelPlayList.Count;
            return currentLevelIndex;
        }

        /// <summary>
        /// Method <c>GetThemes</c> gets a list of available themes.
        /// </summary>
        private void GetThemes()
        {
            // Add default theme first if exists
            if (Directory.Exists(Application.dataPath + "/Themes/Default"))
                themes.Add("Default");
            // Check themes in project
            var projectThemes = Directory.GetDirectories(Application.dataPath + "/Themes", "*", SearchOption.TopDirectoryOnly);
            foreach (var projectTheme in projectThemes)
            {
                var projectThemeName = new DirectoryInfo(projectTheme).Name;
                if (!projectThemeName.Equals("Default"))
                    themes.Add(projectThemeName);
            }
        }
        
        /// <summary>
        /// Method <c>NextTheme</c> gets the next theme.
        /// </summary>
        public string NextTheme()
        {
            currentThemeIndex = (currentThemeIndex + 1) % themes.Count;
            return themes[currentThemeIndex];
        }
        
        /// <summary>
        /// Method <c>GetCurrentThemeName</c> gets the current theme name.
        /// </summary>
        public string GetCurrentThemeName()
        {
            return themes[currentThemeIndex];
        }
        
        /// <summary>
        /// Method <c>GetCurrentThemeScenePath</c> gets the current theme scene path.
        /// </summary>
        public string GetCurrentThemeScenePath()
        {
            return "Themes/" + themes[currentThemeIndex] + "/Scenes/" + gameScene;
        }
        
        /// <summary>
        /// Method <c>LoadMainMenu</c> is used to load the main menu.
        /// </summary>
        public void LoadMainMenu()
        {
            Destroy(gameObject);
            SceneManager.LoadScene(mainMenuScene);
        }
        
        /// <summary>
        /// Method <c>QuitGame</c> is used to quit the game.
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}