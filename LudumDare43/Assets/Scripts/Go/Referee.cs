using Go;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class Referee : MonoBehaviour
    {
        private Game m_Game;
        /// <summary>
        /// GoSharp replaces Game every turn.
        /// </summary>
        public Game Game
        {
            get { return m_Game; }
            set
            {
                if (value != null)
                    Debug.Log(value.Board);

                m_Game = value;
            }
        }
    }
}
