using UnityEngine;
using System.Collections;

namespace MesoGames
{
    public class CuztomCharacterAnimatorUtility : MonoBehaviour
    {
        public void SetCurrentState( string p_trigger, bool p_doPrevious = false )
        {
            if( !string.IsNullOrEmpty( _current ) )
            {
                if( p_doPrevious )
                {
                    if( !( _current.Equals( p_trigger ) ) )
                    {
                        _previous = _current;
                    }
                }
                else
                {
                    _previous = "";
                }
                _current = p_trigger;
            }
        }

        public void GotoPreviousState( ref Animator p_animator )
        {
            if( !string.IsNullOrEmpty( _previous ) )
            {
                p_animator.SetTrigger( _previous );
            }
        }

        public void SetFaceAnimationState( FaceAnimatorTriggers p_triggers, bool p_isEndEmote = false )
        {
            if( m_mouthAnimator != null )
            {
                m_mouthAnimator.SetTrigger( p_isEndEmote ? p_triggers.EndEmote : p_triggers.Mouth );
                if( p_isEndEmote )
                {
                    m_mouthAnimator.SetTrigger( p_triggers.EndEmote );
                }
                else
                {
                    m_mouthAnimator.ResetTrigger( p_triggers.EndEmote );
                    m_mouthAnimator.SetTrigger( p_triggers.Mouth );
                }
            }

            if( m_browsAnimator != null )
            {
                if( p_isEndEmote )
                {
                    m_browsAnimator.SetTrigger( p_triggers.EndEmote );
                }
                else
                {
                    m_browsAnimator.ResetTrigger( p_triggers.EndEmote );
                    m_browsAnimator.SetTrigger( p_triggers.Brows );
                }
            }

            if( m_eyesAnimator != null )
            {
                if( p_isEndEmote )
                {
                    m_eyesAnimator.SetTrigger( p_triggers.EndEmote );
                }
                else
                {
                    m_eyesAnimator.ResetTrigger( p_triggers.EndEmote );
                    m_eyesAnimator.SetTrigger( p_triggers.Eyes );
                }
            }
        }

        protected void Start()
        {
            for( int i = 0; i < transform.childCount; i++ )
            {
                Transform childBase = transform.GetChild( i );
                if( childBase.name.Contains( "mdl_" ) | childBase.name.Contains( "_char" ) )
                {
                    m_mouthAnimator = childBase.Find( "mouth" ).GetComponent< Animator >() as Animator;
                    m_mouthAnimator.applyRootMotion = true;
                    m_browsAnimator = childBase.Find( "brows" ).GetComponent< Animator >() as Animator;
                    m_browsAnimator.applyRootMotion = true;
                    m_eyesAnimator = childBase.Find( "eyes" ).GetComponent< Animator >() as Animator;
                    m_eyesAnimator.applyRootMotion = true;
                }
            }
        }

        private string _current;
        private string _previous;

        private Animator m_mouthAnimator = null;
        private Animator m_browsAnimator = null;
        private Animator m_eyesAnimator = null;
    }
}
