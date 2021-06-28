using UnityEngine;
using System.Collections;

namespace MesoGames
{
    public class CuztomCharacterFaceSmb : StateMachineBehaviour
    {
        override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            if( m_cuztomAnimatorUtil == null )
            {
                m_cuztomAnimatorUtil = animator.gameObject.GetComponent< CuztomCharacterAnimatorUtility >() as CuztomCharacterAnimatorUtility;
            }

            if( m_cuztomAnimatorUtil != null )
            {
                m_cuztomAnimatorUtil.SetFaceAnimationState( m_faceTriggers, false );
            }
        }

        public override void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
        {
            if( m_cuztomAnimatorUtil != null )
            {
                if( !string.IsNullOrEmpty( m_faceTriggers.EndEmote ) )
                {
                    m_cuztomAnimatorUtil.SetFaceAnimationState( m_faceTriggers, true );
                }
            }
        }

        private CuztomCharacterAnimatorUtility m_cuztomAnimatorUtil = null;
        [SerializeField] private FaceAnimatorTriggers m_faceTriggers = null;
    }

    [System.Serializable]
    public class FaceAnimatorTriggers
    {
        public string Mouth = "";
        public string Eyes = "";
        public string Brows = "";
        public string EndEmote = "";
    }
}