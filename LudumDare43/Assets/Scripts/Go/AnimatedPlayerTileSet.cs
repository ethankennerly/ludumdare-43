using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    [CreateAssetMenu(fileName = "AnimatedPlayerTileSet", menuName = "Go/Animated Player Tile Set")]
    public sealed class AnimatedPlayerTileSet : ScriptableObject
    {
        [Serializable]
        private struct Transition
        {
            public Content previous;
            public Content next;
            public string animationName;
        }

        [SerializeField]
        private Transition[] m_Transitions;

        public string GetAnimationName(Content previous, Content next)
        {
            foreach (Transition transition in m_Transitions)
            {
                if (previous != transition.previous ||
                    next != transition.next)
                    continue;

                return transition.animationName;
            }
            return null;
        }
    }
}
