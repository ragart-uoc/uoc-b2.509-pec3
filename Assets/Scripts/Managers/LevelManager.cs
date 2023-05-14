using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using PEC3.Entities;

namespace PEC3.Managers
{
    /// <summary>
    /// Class <c>LevelManager</c> contains the methods and properties needed for the level management.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        /// <value>Property <c>Instance</c> represents the class instance.</value>
        public static LevelManager Instance;
        
        /// <value>Property <c>Level</c> represents the level.</value>
        public Level Level { get; private set; }

        /// <value>Property <c>baseTilemap</c> represents the base tilemap.</value>
        public Tilemap baseTilemap;

        /// <value>Property <c>groundTile</c> represents the ground tile.</value>
        public Tile groundTile;
        
        /// <value>Property <c>wallTilemap</c> represents the wall tilemap.</value>
        public Tilemap wallTilemap;
        
        /// <value>Property <c>wallTile</c> represents the wall tile.</value>
        public Tile wallTile;
        
        /// <value>Property <c>goalTilemap</c> represents the goal tilemap.</value>
        public Tilemap goalTilemap;
        
        /// <value>Property <c>goalTile</c> represents the goal tile.</value>
        public Tile goalTile;
        
        /// <value>Property <c>boxTilemap</c> represents the box tilemap.</value>
        public Tilemap boxTilemap;
        
        /// <value>Property <c>boxTile</c> represents the box tile.</value>
        public Tile boxTile;
        
        /// <value>Property <c>playerPrefab</c> represents the player prefab.</value>
        public GameObject playerPrefab;
        
        /// <value>Property <c>player</c> represents the player.</value>
        private GameObject _player;

        /// <value>Property <c>_goalPositions</c> represents the goal positions.</value>
        private Vector3Int[] _goalPositions = Array.Empty<Vector3Int>();

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
            // Create a new level
            Level = new Level();
            
            // Get the player
            _player = GameObject.FindGameObjectWithTag("Player");
        }
        
        /// <summary>
        /// Method <c>LoadLevel</c> loads the level.
        /// </summary>
        /// <param name="levelMap">The JSON containing the level map.</param>
        public void LoadLevel(string levelMap)
        {
            Level.ImportLevelStructure(levelMap);
        }

        /// <summary>
        /// Method <c>DrawLevel</c> draws the level.
        /// </summary>
        public void DrawLevel()
        {
            // Fill the base tilemap
            var baseBounds = baseTilemap.cellBounds.allPositionsWithin;
            foreach (var position in baseBounds)
            {
                baseTilemap.SetTile(position, groundTile);
            }
            
            // Set the base tilemap
            foreach (var position in Level.Structure)
            {
                var positionVector = new Vector3Int(position.x, position.y);
                switch (position.type)
                {
                    case TileTypes.Type.Wall:
                        wallTilemap.SetTile(positionVector, wallTile);
                        break;
                    case TileTypes.Type.Goal:
                        goalTilemap.SetTile(positionVector, goalTile);
                        _goalPositions = _goalPositions.Append(positionVector).ToArray();
                        break;
                    case TileTypes.Type.Box:
                        boxTilemap.SetTile(positionVector, boxTile);
                        break;
                    case TileTypes.Type.Empty:
                        break;
                    case TileTypes.Type.Player:
                        var worldPosition = baseTilemap.CellToWorld(positionVector);
                        worldPosition.x += 0.5f;
                        worldPosition.y += 0.5f;
                        _player.transform.position = worldPosition;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        /// <summary>
        /// Method <c>ClearLevel</c> clears the level.
        /// </summary>
        public void ClearLevel()
        {
            // Clear the base tilemap
            var baseBounds = baseTilemap.cellBounds.allPositionsWithin;
            foreach (var position in baseBounds)
            {
                baseTilemap.SetTile(position, null);
                wallTilemap.SetTile(position, null);
                goalTilemap.SetTile(position, null);
                boxTilemap.SetTile(position, null);
            }
        }
        
        /// <summary>
        /// Method <c>CheckWinCondition</c> checks if the player has won.
        /// </summary>
        public bool CheckWinCondition()
        {
            return _goalPositions.All(position => boxTilemap.HasTile(position));
        }
    }
}
