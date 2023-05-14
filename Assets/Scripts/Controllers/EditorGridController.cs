using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using PEC3.Managers;

namespace PEC3.Controllers
{
    /// <summary>
    /// Class <c>EditorGridController</c> controls the editor grid.
    /// </summary>
    public class EditorGridController : MonoBehaviour
    {
        /// <value>Property <c>_previousMousePos</c> represents the previous mouse position.</value>
        private Vector3Int _previousMousePos;

        /// <summary>
        /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            // Fill the base map with the first tile
            var tile = EditorManager.Instance.availableTiles[0];
            var bounds = EditorManager.Instance.editMap.cellBounds;
            for (var x = bounds.xMin; x < bounds.xMax; x++) {
                for (var y = bounds.yMin; y < bounds.yMax; y++) {
                    EditorManager.Instance.editMap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }

        /// <summary>
        /// Method <c>Update</c> is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            // Get the mouse position
            var mousePos = GetMousePosition();
            
            // Check if the mouse is inside the base tilemap bounds
            if (!EditorManager.Instance.editMap.HasTile(mousePos))
                return;
            
            // Set the hover tile
            if (!mousePos.Equals(_previousMousePos)) {
                EditorManager.Instance.hoverMap.SetTile(_previousMousePos, null); // Remove old hoverTile
                EditorManager.Instance.hoverMap.SetTile(mousePos, EditorManager.Instance.hoverTile);
                _previousMousePos = mousePos;
            }

            // Change the tile on click
            if (Mouse.current.leftButton.wasPressedThisFrame) {
                // Get the current tile in position
                var currentTile = EditorManager.Instance.editMap.GetTile(mousePos);
                // Get position in list
                var index = EditorManager.Instance.availableTiles.IndexOf((Tile) currentTile);
                // Get next tile
                var nextTile = EditorManager.Instance.availableTiles[(index + 1) % EditorManager.Instance.availableTiles.Count];
                // Set next tile
                EditorManager.Instance.editMap.SetTile(mousePos, nextTile);
            }
        }

        /// <summary>
        /// Method <c>GetMousePosition</c> returns the mouse position.
        /// </summary>
        /// <returns></returns>
        private Vector3Int GetMousePosition ()
        {
            var clickScreenPosition = Mouse.current.position.ReadValue();
            var clickWorldPosition  = Camera.main.ScreenToWorldPoint(clickScreenPosition);
            var clickCellPosition = EditorManager.Instance.editMap.WorldToCell(clickWorldPosition);
            return clickCellPosition;
        }
    }
}
