using System.Linq;
using UnityEngine;

namespace PEC3.Managers
{
    /// <summary>
    /// Class <c>GameManager</c> controls the game.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <value>Property <c>Instance</c> represents the class instance.</value>
        public static GameManager Instance;

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
    }
}
