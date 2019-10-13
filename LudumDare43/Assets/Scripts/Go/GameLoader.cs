using Go;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class GameLoader : MonoBehaviour
    {
        [Header("Size 1 to 3")]
        [SerializeField]
        private int m_SizeX = 3;

        [Header("Size 1 to 3")]
        [SerializeField]
        private int m_SizeY = 3;

        private void OnEnable()
        {
            Load();
        }

        public void Load()
        {
            Referee.instance.Game = new Game();
            Referee.instance.Game.Clone(new Board(m_SizeX, m_SizeY), Content.Black);
        }
    }
}
