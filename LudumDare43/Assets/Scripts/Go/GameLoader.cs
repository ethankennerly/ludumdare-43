using Go;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class GameLoader : MonoBehaviour
    {
        [Header("Unity does not recognize .sgf extension as text.")]
        [SerializeField]
        private TextAsset m_SgfFile = null;

        private void OnEnable()
        {
            Load();
        }

        public void Load()
        {
            string sgf = m_SgfFile.text;
            Referee.instance.Game = Game.SerializeGameFromSGFString(sgf);
        }
    }
}
