using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;

// Holds variables, including flags, that multiple classes need access to and
// that do not make sense being part of any one of the accessing classes.
public class SharedVariableManager : MonoBehaviour {

	public static SharedVariableManager instance;

	private void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(this.gameObject);
		}
	}

	// Delegate to subscribe to in order to be signalled when the game is over.
	public static UnityAction onGameIsOver;

	// Delegate to subscribe to in order to be signalled when the loading is finished.
	public static UnityAction onLoadIsFinished;

	// Basic flags:

	public bool collisionHasHappened { get; set; } = false;

	// Delegate-associated flags:

	private bool _gameIsOver = false;
	public bool gameIsOver {
		get => _gameIsOver;
		set {
			bool oldGameIsOver = _gameIsOver;
			_gameIsOver = value;

			// If set to true, was false before and delegate is not null, call delegate.
			if (value == true && oldGameIsOver == false && onGameIsOver != null) {
				onGameIsOver();
			}
		}
	}

	private bool _loadIsFinished = false;
	public bool loadIsFinished {
		get => _loadIsFinished;
		set {
			_loadIsFinished = value;

			// If set to true and delegate is not null, call delegate.
			if (value == true && onLoadIsFinished != null) {
				onLoadIsFinished();
			}
		}
	}

	// Inspector variables:

	// The speed in Unity units per second that the agents move along their respective forward directions when not swerving.
	[FormerlySerializedAs("Agent Speed")]
	[SerializeField]
	[Range(0.0f, 10.0f)]
	private float _agentSpeed = 1.0f;
	public float agentSpeed {
		get => _agentSpeed;
		private set => _agentSpeed = value;
	}

	// The ratio of the forward speed that is made into side speed.
	// [FormerlySerializedAs("Agent Swerve Speed Ratio")]
	// [SerializeField]
	// [Range(0.0f, 1.0f)]
	// private float _swerveSideSpeedRatio = 0.5f;
	// public float swerveSideSpeedRatio {
	//   get => _swerveSideSpeedRatio;
	//   private set => _swerveSideSpeedRatio = value;
	// }

	// The standard deviation of the sampled swerve distance.
	[FormerlySerializedAs("Swerve Distance Standard Deviation")]
	[SerializeField]
	[Range(0.0f, 20.0f)]
	private float _swerveStandardDeviation = 1.0f;
	public float swerveStandardDeviation {
		get => _swerveStandardDeviation;
		private set => _swerveStandardDeviation = value;
	}

	// The speed in radians per second at which agents rotate along local y when swerving.
	[FormerlySerializedAs("Rotation Speed")]
	[SerializeField]
	[Range(0.0f, 0.5f)]
	private float _rotationSpeed = 0.1f;
	public float rotationSpeed {
		get => _rotationSpeed;
		private set => _rotationSpeed = value;
	}

	// The size of the swerve margin.
	[FormerlySerializedAs("Swerve Margin")]
	[SerializeField]
	[Range(0.0f, 5.0f)]
	private float _swerveMargin = 0.2f;
	public float swerveMargin {
		get => _swerveMargin;
		private set => _swerveMargin = value;
	}

	// Other variables:

	// Distances reported by the player and read by the trial manager.
	public float playerRobotSwerveDistance { get; set; }
	public float playerStartSwerveDistance { get; set; }

	// Distances reported by the robot and read by the trial manager.
	public float robotPlayerSwerveDistance { get; set; }
	public float robotStartSwerveDistance { get; set; }

	// Variables for seeing if the player and robot swerved.
	public bool playerSwerved { get; set; } = false;
	public bool robotSwerved { get; set; } = false;

	// The radius of the player agent's collider.
	private float _playerRadius;
	public float playerRadius {
		get => _playerRadius;
		set {
			_playerRadius = value;

			if (2 * playerRadius + swerveMargin > swerveWidthOfLargestAgent) {
				swerveWidthOfLargestAgent = 2 * playerRadius + swerveMargin;
			}
		}
	}

	// The radius of the robot agent's collider.
	private float _robotRadius;
	public float robotRadius {
		get => _robotRadius;
		set {
			_robotRadius = value;

			if (2 * robotRadius + swerveMargin > swerveWidthOfLargestAgent) {
				swerveWidthOfLargestAgent = 2 * robotRadius + swerveMargin;
			}
		}
	}

	// The margin plus the width (twice the radius) of the largest ratio.
	public float swerveWidthOfLargestAgent { get; private set; }

	// The ratio of the forward speed that is made into side speed. With how things are set up, it needs to be 0.5f.
	public float swerveSideSpeedRatio { get; private set; } = 0.5f;
}
