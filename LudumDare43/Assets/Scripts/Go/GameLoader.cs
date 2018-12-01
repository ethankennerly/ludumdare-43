using Go;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class GameLoader : MonoBehaviour
    {
        [Header("Unity does not recognize .sgf extension as text.")]
        [SerializeField]
        private TextAsset m_SgfFile = null;

        [SerializeField]
        private Referee m_Referee = null;

        private void OnEnable()
        {
            Load();
        }

        public void Load()
        {
            string sgf = m_SgfFile.text;
            m_Referee.Game = Game.SerializeGameFromSGFString(sgf);
        }
    }
}
