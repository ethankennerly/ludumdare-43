using Go;
using System;
using TMPro;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class ScoreText : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text m_ScoreText;

        [SerializeField]
        private Content m_PlayerFilter;

        private Action<Content, float> m_OnScoreSet;

        private void OnEnable()
        {
            if (m_OnScoreSet == null)
                m_OnScoreSet = SetScore;

            Referee.OnScoreSet -= m_OnScoreSet;
            Referee.OnScoreSet += m_OnScoreSet;
        }

        private void OnDisable()
        {
            Referee.OnScoreSet -= m_OnScoreSet;
        }

        private void SetScore(Content player, float score)
        {
            if (player != m_PlayerFilter)
                return;

            m_ScoreText.text = score.ToString();
        }
    }
}
