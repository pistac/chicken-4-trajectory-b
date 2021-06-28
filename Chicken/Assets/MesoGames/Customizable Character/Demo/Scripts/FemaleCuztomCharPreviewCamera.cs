using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MesoGames;

public class FemaleCuztomCharPreviewCamera : MonoBehaviour
{
    protected void Start()
    {
        _gui.btnReset.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "reset" );

            _gui.grpGeneric.interactable = true;
            _gui.grpReactions.interactable = true;
            _gui.grpAttack.interactable = false;
        });

        _gui.btnIdleStand.onClick.AddListener( () => {
            _gui.grpReactions.interactable = true;
            if( _gui.grpAttack.interactable )
            {
                _gui.grpAttack.interactable = false;
            }
            _targetAnimator.SetTrigger( "idleStand" );
        });

        _gui.btnIdleLaugh.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "idleLaugh" );
        });

        _gui.btnIdleNod.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "idleNod" );
        });

        #region MOVE
        _gui.btnWalk.onClick.AddListener( () => {
            if( _gui.grpReactions.interactable )
            {
                _gui.grpReactions.interactable = false;
            }
            _targetAnimator.SetTrigger( "walk" );
        });

        _gui.btnRun.onClick.AddListener( () => {
            if( _gui.grpReactions.interactable )
            {
                _gui.grpReactions.interactable = false;
            }
            _targetAnimator.SetTrigger( "run" );
        });

        _gui.btnSneak.onClick.AddListener( () => {
            if( _gui.grpReactions.interactable )
            {
                _gui.grpReactions.interactable = false;
            }
            _targetAnimator.SetTrigger( "sneak" );
        });

        _gui.btnJumpLow.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "jumpLow" );
        });

        _gui.btnJumpHigh.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "jumpHigh" );
        });

        _gui.btnDie.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "die" );

            _gui.btnReset.interactable = true;
            _gui.grpGeneric.interactable = false;
            _gui.grpAttack.interactable = false;
            _gui.grpReactions.interactable = false;
        });
        #endregion

        #region REACTIONS
        {
            _gui.btnReactAwe.onClick.AddListener( () => {
                _targetAnimator.SetTrigger( "reactAwe" );
            });
            _gui.btnReactAngry.onClick.AddListener( () => {
                _targetAnimator.SetTrigger( "reactAngry" );
            });
            _gui.btnReactFear.onClick.AddListener( () => {
                _targetAnimator.SetTrigger( "reactFear" );
            });
            _gui.btnReactSurprise.onClick.AddListener( () => {
                _targetAnimator.SetTrigger( "reactSurprise" );
            });
            _gui.btnReactShakeFist.onClick.AddListener( () => {
                _targetAnimator.SetTrigger( "reactShakeFist" );
            });
        }
        #endregion

        #region STANCE
        _gui.btnIdleStance.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "idleStance" );

            _gui.btnIdleStanceTaunt.interactable = true;
            _gui.grpAttack.interactable = true;

            _gui.btnIdleLaugh.interactable = false;
            _gui.btnIdleNod.interactable = false;
        });

        _gui.btnIdleStanceTaunt.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "idleStanceTaunt" );
        });
        #endregion

        #region ATTACK
        //Punching
        _gui.btnPunchLeft.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "punchLeft" );
        });
        _gui.btnPunchRight.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "punchRight" );
        });
        _gui.btnPunchLeftRight.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "punchLeftRight" );
        });

        // Kicking
        _gui.btnKickLeft.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "kickLeft" );
        });
        _gui.btnKickRight.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "kickRight" );
        });

        // Blocking
        _gui.btnGetHitLeft.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "getHitLeft" );
        });
        _gui.btnGetHitRight.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "getHitRight" );
        });
        _gui.btnGetHitMiddle.onClick.AddListener( () => {
            _targetAnimator.SetTrigger( "getHitMiddle" );
        });
        #endregion
    }

    protected void Update()
    {
        if( _targetTx != null )
        {
            transform.RotateAround( _targetTx.position, Vector3.up, 20.0f * Time.deltaTime );
        }
    }

    [SerializeField] private Transform _targetTx = null;
    [SerializeField] private Animator _targetAnimator = null;

    [SerializeField] private TestGUI _gui = null;

    [System.Serializable]
    public class TestGUI
    {
        public Button btnReset = null;

        public CanvasGroup grpGeneric = null;
        public Button btnIdleStand = null;
        public Button btnIdleLaugh = null;
        public Button btnIdleNod = null;
        public Button btnIdleStance = null;
        public Button btnIdleStanceTaunt = null;

        public CanvasGroup grpReactions = null;
        public Button btnReactAwe = null;
        public Button btnReactAngry = null;
        public Button btnReactFear = null;
        public Button btnReactSurprise = null;
        public Button btnReactShakeFist = null;

        public CanvasGroup grpAttack = null;
        public Button btnWalk = null;
        public Button btnRun = null;
        public Button btnSneak = null;
        public Button btnJumpLow = null;
        public Button btnJumpHigh = null;
        public Button btnDie = null;

        public Button btnPunchLeft = null;
        public Button btnPunchRight = null;
        public Button btnPunchLeftRight = null;

        public Button btnKickLeft = null;
        public Button btnKickRight = null;

        public Button btnGetHitLeft = null;
        public Button btnGetHitRight = null;
        public Button btnGetHitMiddle = null;

        public Button btnBlockOnce = null;
        public Button btnBlockLoop = null;
    }
}
