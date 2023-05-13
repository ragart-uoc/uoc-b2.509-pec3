namespace PEC3.Entities
{
    /// <summary>
    /// Class <c>TileTypes</c> represents the tile types.
    /// </summary>
    public abstract class TileTypes
    {
        /// <value>Property <c>TileType</c> represents the tile type.</value>
        public enum Type
        {
            Box,
            Empty,
            Goal,
            Player,
            Wall
        }
    }
}
