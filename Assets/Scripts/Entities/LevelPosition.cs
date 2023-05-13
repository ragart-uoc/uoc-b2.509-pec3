using System;

namespace PEC3.Entities
{
    /// <summary>
    /// Class <c>LevelPosition</c> represents the level structure position.
    /// </summary>
    [Serializable]
    public class LevelPosition
    {
        /// <value>Property <c>x</c> represents the x-axis position.</value>
        public int x;
        
        /// <value>Property <c>y</c> represents the y-axis position.</value>
        public int y;
        
        /// <value>Property <c>type</c> represents the type of the position.</value>
        public TileTypes.Type type;
        
        /// <summary>
        /// Method <c>LevelPosition</c> is the constructor of the class.
        /// </summary>
        /// <param name="x">The x-axis position.</param>
        /// <param name="y">The y-axis position.</param>
        /// <param name="type">The type of the position.</param>
        public LevelPosition(int x, int y, TileTypes.Type type)
        {
            this.x = x;
            this.y = y;
            this.type = type;
        }
    }
}
