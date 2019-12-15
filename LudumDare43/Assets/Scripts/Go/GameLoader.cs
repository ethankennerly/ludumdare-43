using FineGameDesign.Go.AI;
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
            Referee5x5.instance.Game = new GoGameState5x5();
            Referee5x5.instance.Init(m_SizeX, m_SizeY);
        }
    }
}
