using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace PEC3.Managers
{
    /// <summary>
    /// Class <c>GameManager</c> controls the game.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <value>Property <c>Instance</c> represents the class instance.</value>
        public static GameManager Instance;

        /// <value>Property <c>_moves</c> represents the number of moves.</value>
        private int _moves;
        
        /// <value>Property <c>levelNameText</c> represents the level name text.</value>
        public TextMeshProUGUI levelNameText;
        
        /// <value>Property <c>moveCounterText</c> represents the move counter text.</value>
        public TextMeshProUGUI moveCounterText;
        
        /// <value>Property <c>messagesText</c> represents the messages text.</value>
        public TextMeshProUGUI messagesText;

        /// <value>Property <c>nextLevelButton</c> represents the next level button.</value>
        public GameObject nextLevelButton;
        
        /// <value>Property <c>mainMenuButton</c> represents the main menu button.</value>
        public GameObject mainMenuButton;

        /// <value>Property <c>_playerInput</c> represents the player input.</value>
        private PlayerInput _playerInput;

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
            // Get the player
            var player = GameObject.FindGameObjectWithTag("Player");
            _playerInput = player.GetComponent<PlayerInput>();
            
            // Load the level
            var levelMap = GlobalGameManager.Instance.GetCurrentLevelContent();
            LevelManager.Instance.LoadLevel(levelMap);
            
            // Start the level
            StartLevel();
        }
        
        /// <summary>
        /// Method <c>StartLevel</c> starts the level.
        /// </summary>
        private void StartLevel()
        {
            // Set the moves counter
            _moves = 0;
            UpdateMoveCounter();
            
            // Set the level name
            UpdateLevelName();
            
            // Hide level complete message and buttons
            UpdateMessage(String.Empty);
            nextLevelButton.SetActive(false);
            mainMenuButton.SetActive(false);

            // Draw the level
            LevelManager.Instance.DrawLevel();
            
            // Enable the player input
            _playerInput.enabled = true;
        }
        
        /// <summary>
        /// Method <c>RestartLevel</c> restarts the level.
        /// </summary>
        public void RestartLevel()
        {
            // Clear the level
            LevelManager.Instance.ClearLevel();
            
            // Start the level
            StartLevel();
        }

        /// <summary>
        /// Method <c>NextLevel</c> loads the next level.
        /// </summary>
        public void NextLevel()
        {
            // Load the next level
            var levelMap = GlobalGameManager.Instance.NextLevel();
            LevelManager.Instance.LoadLevel(levelMap);
            
            // Restart the level
            RestartLevel();
        }
        
        /// <summary>
        /// Method <c>MovePerformed</c> is called when the player performs a move.
        /// </summary>
        public void MovePerformed()
        {
            // Update the moves
            _moves++;
            UpdateMoveCounter();
            
            // Check if the player has won
            if (!LevelManager.Instance.CheckWinCondition())
                return;
            
            // Disable the player input
            _playerInput.enabled = false;
            
            // Show the level complete message
            UpdateMessage("Level complete");

            // Show the next level button if it is not the last level
            if (!GlobalGameManager.Instance.IsLastLevel())
                nextLevelButton.SetActive(true);
            
            // Show the main menu button
            mainMenuButton.SetActive(true);
        }
        
        /// <summary>
        /// Method <c>UpdateMessage</c> updates the message.
        /// </summary>
        /// <param name="message">The message to update.</param>
        private void UpdateMessage(string message)
        {
            messagesText.text = message;
        }
        
        /// <summary>
        /// Method <c>UpdateLevelName</c> updates the level name.
        /// </summary>
        private void UpdateLevelName()
        {
            levelNameText.text = GlobalGameManager.Instance.GetCurrentLevelName();
        }

        /// <summary>
        /// Method <c>UpdateMoveCounter</c> updates the move counter.
        /// </summary>
        private void UpdateMoveCounter()
        {
            moveCounterText.text = _moves.ToString();
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
