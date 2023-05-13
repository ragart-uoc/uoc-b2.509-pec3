using System;
using System.Linq;
using PEC3.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PEC3.Managers
{
    /// <summary>
    /// Class <c>GameManager</c> controls the game.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <value>Property <c>Instance</c> represents the game manager instance.</value>
        public static GameManager Instance;

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
            // Get the level
            var level = new Level();
            level.ImportLevelStructure(Application.persistentDataPath + "/levels/test.json");
            
            // Fill the base tilemap
            var baseBounds = baseTilemap.cellBounds.allPositionsWithin;
            foreach (var position in baseBounds)
            {
                baseTilemap.SetTile(position, groundTile);
            }
            
            // Set the base tilemap
            foreach (var position in level.Structure)
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
                        Instantiate(playerPrefab, worldPosition, Quaternion.identity);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
