using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace PEC3.Entities
{
    /// <summary>
    /// Method <c>Level</c> represents the level structure.
    /// </summary>
    [Serializable]
    public class Level
    {
        /// <value>Property <c>levelName</c> represents the level name.</value>
        [field : SerializeField]
        public string LevelName { get; set; }

        /// <value>Property <c>levelStructure</c> represents the level structure.</value>
        [field : SerializeField]
        public List<LevelPosition> Structure { get; set; }
        
        /// <value>Property <c>_minPosition</c> represents the minimum position.</value>
        private Vector2Int _minPosition = new Vector2Int(-5, -5);
        
        /// <value>Property <c>_maxPosition</c> represents the maximum position.</value>
        private Vector2Int _maxPosition = new Vector2Int(4, 4);
        

        /// <summary>
        /// Method <c>Level</c> is the constructor of the class.
        /// </summary>
        public Level()
        {
            Structure = new List<LevelPosition>();
        }

        /// <summary>
        /// Method <c>SetPosition</c> sets the value of a position.
        /// </summary>
        /// <param name="x">The x-axis position.</param>
        /// <param name="y">The y-axis position.</param>
        /// <param name="value">The value to set.</param>
        public void SetPosition(int x, int y, string value)
        {
            // Check if the position is out of range
            if (x < _minPosition.x || x > _maxPosition.x || y < _minPosition.y || y > _maxPosition.y)
            {
                Debug.LogWarning("The position is out of range.");
                return;
            }
            // Overwrite the value if the position already exists
            foreach (var position in Structure.Where(position => position.x == x && position.y == y))
            {
                position.type = value;
                return;
            }
            // Add the position if it doesn't exist
            Structure.Add(new LevelPosition(x, y, value));
        }
        
        /// <summary>
        /// Method <c>GetPosition</c> gets the value of a position.
        /// </summary>
        /// <param name="x">The x-axis position.</param>
        /// <param name="y">The y-axis position.</param>
        public string GetPosition(int x, int y)
        {
            // Check if the position is out of range
            if (x < _minPosition.x || x > _maxPosition.x || y < _minPosition.y || y > _maxPosition.y)
            {
                Debug.LogWarning("The position is out of range.");
                return String.Empty;
            }
            // Check if the position exists
            foreach (var position in Structure.Where(position => position.x == x && position.y == y))
            {
                return position.type;
            }
            return String.Empty;
        }

        /// <summary>
        /// Method <c>ExportLevelStructure</c> exports the level structure.
        /// </summary>
        /// <param name="savePath">The path to save the level structure.</param>
        public void ExportLevelStructure(string savePath)
        {
            var json = JsonUtility.ToJson(this, true);
            Directory.CreateDirectory(Path.GetDirectoryName(savePath) ?? string.Empty);
            File.WriteAllText(savePath, json);
        }
        
        /// <summary>
        /// Method <c>ImportLevelStructure</c> imports the level structure.
        /// </summary>
        /// <param name="loadPath">The path to load the level structure.</param>
        public void ImportLevelStructure(string loadPath)
        {
            var json = File.ReadAllText(loadPath);
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }
}
