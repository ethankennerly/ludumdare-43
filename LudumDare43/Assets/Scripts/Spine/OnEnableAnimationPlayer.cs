using Spine.Unity;
using UnityEngine;

namespace FineGameDesign.Spine
{
    public sealed class OnEnableAnimationPlayer : MonoBehaviour
    {
        [SerializeField]
        private SkeletonAnimation m_Skeleton = null;

        [SerializeField]
        private string m_AnimationName = null;

        /// <summary>
        /// Clears animation before setting.
        /// Otherwise, did not restart the animation.
        /// </summary>
        private void OnEnable()
        {
            m_Skeleton.AnimationName = null;
            m_Skeleton.AnimationName = m_AnimationName;
        }
    }
}
