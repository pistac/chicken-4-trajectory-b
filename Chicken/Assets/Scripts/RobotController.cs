using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls robot movement.
public class RobotController : MonoBehaviour {

#pragma warning disable
	[SerializeField]
	private SharedVariableManager sharedVariableManager;
	[SerializeField]
	private Transform playerTransform;
	[SerializeField]
	private bool debugDoNotSwerve = false;
#pragma warning restore

	private bool paused = true;
	private bool robotHasSwerved = false;
	private bool robotWouldSwerve;
	private float margin;
	private float swerveDistance;
	private float speed;
	private float swerveForwardSpeed;
	private float swerveSideSpeed;
	private float rotationSpeed;
	private float radius;
	private int swerveDirection = 0;

	private Vector3 positiveSwerveDirection;
	private Vector3 robotDirection;

	public void OverrideSwerve() {
		swerveDistance = Mathf.Infinity;
	}

	public void UpdateWidth() {
		radius = GetComponent<CapsuleCollider>().radius;
		margin = sharedVariableManager.swerveMargin;
		sharedVariableManager.robotRadius = radius;

		swerveDistance = 3 * sharedVariableManager.swerveWidthOfLargestAgent;
		if (Application.isEditor) {
			if (debugDoNotSwerve) {
				swerveDistance = -10.0f;
			}
		}
	}

	void Awake() {
		robotDirection = transform.parent.InverseTransformDirection(transform.forward);
		positiveSwerveDirection = transform.parent.InverseTransformDirection(transform.right);
		speed = sharedVariableManager.agentSpeed;
		swerveForwardSpeed = speed * (1 - sharedVariableManager.swerveSideSpeedRatio);
		swerveSideSpeed = speed * sharedVariableManager.swerveSideSpeedRatio;
		rotationSpeed = sharedVariableManager.rotationSpeed;

		// Have a 50/50 chance that robot would swerve if the player is close enough.
		robotWouldSwerve = (Random.Range(0, 2) > 0) ? true : false; // TODO: should this depend on motivation?

		if (Application.isEditor) {
			Debug.Log("robotWouldSwerve = " + robotWouldSwerve);
		}
	}

	void Start() {
		UpdateWidth();
	}

	public void Pause() {
		paused = true;
	}

	public void UnPause() {
		paused = false;
		GetComponent<TrajectoryTracker>().StartSampling();
	}

	void Update() {
		// Only execute if not paused.
		if (paused) return;
		// Check if robot has swerved fully.
		if (Mathf.Abs(transform.localPosition.x) >= sharedVariableManager.swerveWidthOfLargestAgent) {
			robotHasSwerved = true;
		}
	}

	void FixedUpdate() {
		// Only execute if not paused.
		if (paused) return;

		// Handle the movement of the robot.
		MoveRobot();
	}

	// Moves the robot forward, swerving according to player position.
	// Rotation is also controlled by this method.
	private void MoveRobot() {
		float playerRobotDistance = Vector3.Distance(playerTransform.position, transform.position);

		// If the player is beyond the minimum swerve distance.
		// Also if the robot has swerved fully or will not swerve. Move normally.
		if (!robotWouldSwerve ||
			playerRobotDistance > swerveDistance ||
			robotHasSwerved) {
			// Move robot in robotDirection at speed.
			transform.localPosition += robotDirection * speed * Time.fixedDeltaTime;

			// Rotate robot.
			transform.localRotation = Quaternion.LookRotation(
				Vector3.RotateTowards(transform.parent.InverseTransformDirection(transform.forward),
				robotDirection, rotationSpeed, 0.0f));
		} else { // If the player is within the minimum swerve distance.
		
			// Pick the appropriate swerve direction if none has been picked, and report distances.
			if (swerveDirection == 0) {
				PickSwerveDirection();

				sharedVariableManager.robotSwerved = true;

				// Report swerve distances.
				sharedVariableManager.robotPlayerSwerveDistance = Mathf.Abs((playerTransform.position - transform.position).z);
				sharedVariableManager.robotStartSwerveDistance = Mathf.Abs((GameObject.Find("RobotStartPoint").transform.position - transform.position).z);
			}

			// Move robot along swerve direction.
			// Vector3 direction = Vector3.Normalize(robotDirection + (positiveSwerveDirection * swerveDirection));
			// transform.localPosition += direction * swerveSpeed * Time.fixedDeltaTime;
			Vector3 swerveVelocity = swerveForwardSpeed * robotDirection + swerveSideSpeed * positiveSwerveDirection * swerveDirection;
			transform.localPosition += swerveVelocity * Time.fixedDeltaTime;

			// Rotate robot.
			transform.localRotation = Quaternion.LookRotation(
				Vector3.RotateTowards(transform.parent.InverseTransformDirection(transform.forward),
				swerveVelocity.normalized, rotationSpeed, 0.0f));
		}
	}

	// Picks and sets the swerve direction to be away from the player's swerved direction.
	// If player has not yet swerved, pick uniformly random direction.
	private void PickSwerveDirection() {
		float playerSwerveAmount = playerTransform.localPosition.x;
		// If player has started swerving to their right.
		if (playerSwerveAmount > 0) {
			swerveDirection = 1; // Swerve right.
		} else if (playerSwerveAmount < 0) { // If player has started swerving to their left.
			swerveDirection = -1; // Swerve left.
		} else { // If player has not swerved at all.
				 // Pick randomly to swerve left or right.
			swerveDirection = Random.Range(0, 2) == 0 ? -1 : 1;
		}
	}
}
