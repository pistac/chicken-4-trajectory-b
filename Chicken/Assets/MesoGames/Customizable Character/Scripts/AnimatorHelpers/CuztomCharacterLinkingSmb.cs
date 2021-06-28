using UnityEngine;
using System.Collections;

namespace MesoGames
{
    public class CuztomCharacterLinkingSmb : StateMachineBehaviour
    {
        public override void OnStateMachineEnter( Animator animator, int stateMachinePathHash )
        {
            base.OnStateMachineEnter( animator, stateMachinePathHash );
            if( m_cuztomAnimatorUtil == null )
            {
                m_cuztomAnimatorUtil = animator.gameObject.GetComponent< CuztomCharacterAnimatorUtility >() as CuztomCharacterAnimatorUtility;
            }

            if( m_cuztomAnimatorUtil != null )
            {
                m_cuztomAnimatorUtil.SetCurrentState( m_thisTrigger, m_doPreviousWhenDone );
            }
        }

        public override void OnStateMachineExit( Animator animator, int stateMachinePathHash )
        {
            base.OnStateMachineExit( animator, stateMachinePathHash );
            if( ( m_cuztomAnimatorUtil != null ) & m_doPreviousWhenDone )
            {
                m_cuztomAnimatorUtil.GotoPreviousState( ref animator );
            }
        }

        private CuztomCharacterAnimatorUtility m_cuztomAnimatorUtil = null;

        [SerializeField] private string m_thisTrigger = "";
        [SerializeField] private bool m_doPreviousWhenDone = false;
    }
}
