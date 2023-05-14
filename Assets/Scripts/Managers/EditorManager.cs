using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using PEC3.Entities;

namespace PEC3.Managers
{
    /// <summary>
    /// Class <c>EditorManager</c> controls the editor.
    /// </summary>
    public class EditorManager : MonoBehaviour
    {
        /// <value>Property <c>Instance</c> represents the class instance.</value>
        public static EditorManager Instance;

        /// <value>Property <c>Name</c> represents the name of the level.</value>
        private string Name { get; set; }
        
        /// <value>Property <c>Level</c> represents the level.</value>
        private Level Level { get; set; } = new Level();
        
        /// <value>Property <c>baseMap</c> represents the base map.</value>
        public Tilemap editMap;
        
        /// <value>Property <c>hoverMap</c> represents the hover map.</value>
        public Tilemap hoverMap;
        
        /// <value>Property <c>hoverTile</c> represents the hover tile.</value>
        public Tile hoverTile;
        
        /// <value>Property <c>availableTiles</c> represents the list of available tiles.</value>
        public List<Tile> availableTiles;
        
        /// <value>Property <c>levelName</c> represents the UI element containing the level name.</value>
        public TMP_InputField levelName;

        /// <value>Property <c>warningText</c> represents the UI element containing the warning text.</value>
        public TextMeshProUGUI warningText;
        
        /// <value>Property <c>savedLevels</c> represents the UI element containing the saved levels.</value>
        public GameObject savedLevels;

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
            warningText.text = String.Empty;
        }
        
        /// <summary>
        /// Method <c>FillLevelList</c> fills the level list.
        /// </summary>
        private void FillLevelList()
        {
            // Destroy existing level buttons
            foreach (Transform child in savedLevels.transform)
                Destroy(child.gameObject);
            
            // Create the custom level buttons
            foreach (var level in GlobalGameManager.Instance.CustomLevels)
            {
                var levelButton = Instantiate(levelButtonPrefab, savedLevels.transform);
                levelButton.GetComponent<TextMeshProUGUI>().text = level.Key;
                levelButton.GetComponent<Button>().onClick.AddListener(() => LoadLevel(level.Value));
            }
        }
        
        /// <summary>
        /// Method <c>LoadLevel</c> loads the level.
        /// </summary>
        /// <param name="levelMap">The JSON containing the level map.</param>
        public void LoadLevel(string levelMap)
        {
            Level = new Level();
            Level.ImportLevelStructure(levelMap);
            Name = Level.Name;
            levelName.text = Name;
            DrawLevel();
        }

        /// <summary>
        /// Method <c>DrawLevel</c> draws the level.
        /// </summary>
        private void DrawLevel()
        {
            // Clear the current tiles
            editMap.ClearAllTiles();
            
            // Draw the new tiles
            foreach (var position in Level.Structure)
            {
                var positionVector = new Vector3Int(position.x, position.y);
                switch (position.type)
                {
                    case TileTypes.Type.Wall:
                        editMap.SetTile(positionVector, availableTiles[1]);
                        break;
                    case TileTypes.Type.Goal:
                        editMap.SetTile(positionVector, availableTiles[3]);
                        break;
                    case TileTypes.Type.Box:
                        editMap.SetTile(positionVector, availableTiles[2]);
                        break;
                    case TileTypes.Type.Empty:
                        break;
                    case TileTypes.Type.Player:
                        editMap.SetTile(positionVector, availableTiles[4]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            // Fill the rest of the map with empty tiles
            var bounds = editMap.cellBounds;
            for (var x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (var y = bounds.yMin; y < bounds.yMax; y++)
                {
                    var position = new Vector3Int(x, y, 0);
                    if (editMap.GetTile(position) == null)
                        editMap.SetTile(position, availableTiles[0]);
                }
            }
        }

        /// <summary>
        /// Method <c>SaveLevel</c> saves the level.
        /// </summary>
        public void SaveLevel()
        {
            // Map the level
            MapLevel();

            // Validate the level
            if (!Validate())
                return;
            
            // Save the level
            var savePath = Application.persistentDataPath + "/Levels/" + Name + ".json";
            Level.ExportLevelStructure(savePath);
            
            // Refresh the custom levels
            GlobalGameManager.Instance.GetCustomLevels();
            FillLevelList();
            
            // Show the success message
            StartCoroutine(PrintWarning("Saved", 2f));
        }
        
        /// <summary>
        /// Method <c>MapLevel</c> maps the level. 
        /// </summary>
        private void MapLevel()
        {
            // Clear the current structure
            Level.Structure.Clear();
            
            // Map the new structure
            var bounds = editMap.cellBounds;
            for (var x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (var y = bounds.yMin; y < bounds.yMax; y++)
                {
                    var position = new Vector3Int(x, y, 0);
                    var tile = editMap.GetTile(position);
                    if (tile == availableTiles[0])
                        Level.Structure.Add(new LevelPosition(x, y, TileTypes.Type.Empty));
                    else if (tile == availableTiles[1])
                        Level.Structure.Add(new LevelPosition(x, y, TileTypes.Type.Wall));
                    else if (tile == availableTiles[2])
                        Level.Structure.Add(new LevelPosition(x, y, TileTypes.Type.Box));
                    else if (tile == availableTiles[3])
                        Level.Structure.Add(new LevelPosition(x, y, TileTypes.Type.Goal));
                    else if (tile == availableTiles[4])
                        Level.Structure.Add(new LevelPosition(x, y, TileTypes.Type.Player));
                }
            }
        }

        /// <summary>
        /// Method <c>Validate</c> validates the level.
        /// </summary>
        private bool Validate()
        {
            // Check if name is empty
            if (string.IsNullOrEmpty(Name))
            {
                StartCoroutine(PrintWarning("Level name is empty", 2f));
                return false;
            }
            
            // Get defined tiles
            var boxes = Level.Structure.Where(position => position.type == TileTypes.Type.Box).ToList();
            var goals = Level.Structure.Where(position => position.type == TileTypes.Type.Goal).ToList();
            var players = Level.Structure.Where(position => position.type == TileTypes.Type.Player).ToList();
            
            // Check if there are no boxes or goals
            if (boxes.Count < 1 || goals.Count < 1)
            {
                StartCoroutine(PrintWarning("At least one box and goal needed", 2f));
                return false;
            }
            
            // Check if boxes and goals are the same
            if (boxes.Count != goals.Count)
            {
                StartCoroutine(PrintWarning("Different number of boxes and goals", 2f));
                return false;
            }
            
            // Check if player is present or there's more than one
            switch (players.Count)
            {
                case < 1:
                    StartCoroutine(PrintWarning("Player is not present", 2f));
                    return false;
                case > 1:
                    StartCoroutine(PrintWarning("There's more than one player", 2f));
                    return false;
                default:
                    return true;
            }
        }
        
        /// <summary>
        /// Method <c>PrintWarning</c> prints a warning message for a certain duration.
        /// </summary>
        /// <param name="message">The message to print.</param>
        /// <param name="duration">The duration to print the message.</param>
        private IEnumerator PrintWarning(string message, float duration)
        {
            warningText.text = message;
            yield return new WaitForSeconds(duration);
            warningText.text = String.Empty;
        }
        
        /// <summary>
        /// Method <c>HandleChangeName</c> changes the name of the level.
        /// </summary>
        public void HandleChangeName(string newName)
        {
            Name = newName;
        }

        /// <summary>
        /// Method <c>HandleLoadMainMenu</c> loads the main menu through the global game manager.
        /// </summary>
        public void HandleLoadMainMenu()
        {
            GlobalGameManager.Instance.LoadMainMenu();
        }
        
        /// <summary>
        /// Method <c>HandleQuitGame</c> quits the game through the global game manager.
        /// </summary>
        public void HandleQuitGame()
        {
            GlobalGameManager.Instance.QuitGame();
        }
    }
}
