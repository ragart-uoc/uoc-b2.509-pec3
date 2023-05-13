using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using PEC3.Managers;

namespace PEC3.Controllers
{
    /// <summary>
    /// Class <c>PlayerGridController</c> controls the player movement in a grid.
    /// </summary>
    public class PlayerGridController : MonoBehaviour
    {
        /// <value>Property <c>moveSpeed</c> represents the player's movement speed.</value>
        public float moveSpeed = 8f;

        /// <value>Property <c>_moveInput</c> represents the player's movement input.</value>
        private Vector2 _moveInput;
        
        /// <value>Property <c>_isMoving</c> represents the player's movement.</value>
        private bool _isMoving;
        
        /// <value>Property <c>_lastMovement</c> represents the player's last movement.</value>
        private Vector2 _lastMovement = Vector2.zero;
        
        /// <value>Property <c>_gameManager</c> represents the game manager.</value>
        private GameManager _gameManager;

        /// <value>Property <c>_animator</c> represents the player's animator.</value>
        private Animator _animator;

        /// <value>Property <c>InputX</c> represents the player's input on the x-axis.</value>
        private static readonly int InputX = Animator.StringToHash("InputX");
        
        /// <value>Property <c>InputY</c> represents the player's input on the y-axis.</value>
        private static readonly int InputY = Animator.StringToHash("InputY");
        
        /// <value>Property <c>IsMoving</c> represents the player's movement.</value>
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        
        /// <summary>
        /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            // Get the game manager
            _gameManager = GameManager.Instance;

            // Get the animator component
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Method <c>FixedUpdate</c> is called every fixed frame-rate frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void FixedUpdate()
        {
            if (_moveInput != Vector2.zero && !_isMoving)
                MovePlayer(_moveInput);
            _animator.SetBool(IsMoving, _isMoving);
        }

        /// <summary>
        /// Method <c>OnMove</c> is called when the player moves.
        /// </summary>
        public void OnMove(InputValue value)
        {
            // Read the movement input from the input system
            _moveInput = value.Get<Vector2>();
        }
        
        /// <summary>
        /// Method <c>MovePlayer</c> moves the player.
        /// </summary>
        private void MovePlayer(Vector2 movement)
        {
            // Round the movement to the nearest integer to align with the grid
            if (movement.x != 0 && movement.y != 0)
            {
                if (_lastMovement.y != 0)
                    movement.y = 0;
                else
                    movement.x = 0;
            }
            movement = new Vector2(Mathf.RoundToInt(movement.x), Mathf.RoundToInt(movement.y));
            _lastMovement = movement;
            
            // Set the animator parameters
            _animator.SetFloat(InputX, movement.x);
            _animator.SetFloat(InputY, movement.y);

            // Calculate the new position of the player
            var newPosition = transform.position + new Vector3(movement.x, movement.y, 0f);

            // Get the grid coordinates of the new position
            var newPositionGrid = _gameManager.baseTilemap.WorldToCell(newPosition);

            // Check if the new position is inside the bounds
            if (!_gameManager.baseTilemap.cellBounds.Contains(newPositionGrid))
                return;

            // Check if the new position contains a wall
            var wallTile = _gameManager.wallTilemap.GetTile(newPositionGrid);
            if (wallTile != null)
                return;
            
            // If the new position contains a box, check if the box can be moved
            var boxTile = _gameManager.boxTilemap.GetTile(newPositionGrid);
            if (boxTile != null)
            {
                // Calculate the new position of the box
                var newBoxPosition = newPositionGrid + new Vector3Int(
                    Mathf.RoundToInt(movement.x),
                    Mathf.RoundToInt(movement.y),
                    0);
                
                // Check if the new position of the box is inside the bounds
                if (!_gameManager.baseTilemap.cellBounds.Contains(newBoxPosition))
                    return;
                
                // Check if the new position of the box contains a wall or another box
                var newBoxTile = _gameManager.boxTilemap.GetTile(newBoxPosition);
                var newWallTile = _gameManager.wallTilemap.GetTile(newBoxPosition);
                if (newBoxTile != null || newWallTile != null)
                    return;

                // Move the box to the new position
                _gameManager.boxTilemap.SetTile(newBoxPosition, _gameManager.boxTilemap.GetTile(newPositionGrid));
                _gameManager.boxTilemap.SetTile(newPositionGrid, null);
            }

            // Move the player to the new position
            _isMoving = true;
            var moveCoroutine = StartCoroutine(Move(transform, newPosition, moveSpeed));
            var coolDownRoutine = StartCoroutine(MoveCooldown(1f / moveSpeed, moveCoroutine));
            if (_gameManager.CheckWinCondition())
                enabled = false;
            
        }
        
        /// <summary>
        /// Method <c>Move</c> moves an entity to a new position.
        /// </summary>
        /// <param name="entity">The entity to move.</param>
        /// <param name="newPosition">The new position of the player.</param>
        /// <param name="speed">The speed of the movement.</param>
        private static IEnumerator Move(Transform entity, Vector3 newPosition, float speed)
        {
            var elapsedTime = 0f;
            var targetTime = 1f / speed;
            var startPosition = entity.position;
            while (elapsedTime < targetTime)
            {
                entity.position = Vector3.Lerp(startPosition, newPosition, (elapsedTime / targetTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            entity.position = newPosition;
        }

        /// <summary>
        /// Method <c>MoveCooldown</c> prevents the player from moving too fast.
        /// </summary>
        /// <param name="cooldown">The cooldown between movements.</param>
        /// /// <param name="moveCoroutine">The coroutine to wait for.</param>
        private IEnumerator MoveCooldown(float cooldown, Coroutine moveCoroutine = null)
        {
            if (moveCoroutine != null)
                yield return moveCoroutine;
            yield return new WaitForSeconds(cooldown);
            _isMoving = false;
        }
    }
}
