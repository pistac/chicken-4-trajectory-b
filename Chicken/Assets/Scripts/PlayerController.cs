using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls player input and movement. Also checks for game over conditions.
public class PlayerController : MonoBehaviour {

#pragma warning disable
	[SerializeField]
	private SharedVariableManager sharedVariableManager;
	[SerializeField]
	private Transform robotTransform;
	[SerializeField]
	private float minSpeed = 0.1f;
	[SerializeField]
	private float maxSpeedCoefficient = 2.0f; // The coefficient to the agent speed from SharedVariableManager.
	[SerializeField]
	private float walkSpeedMultiplier = 1.0f;
	[SerializeField]
	private float acceleration = 0.5f;
	[SerializeField]
	private float gameOverDelay = 3.0f;
	[SerializeField]
	private float collisionDelay = 0.5f;
#pragma warning restore

	private Animator playerAnimator;
	private bool paused = true;
	private bool startedMoving = false;
	private float margin;
	private float maxSpeed;
	private float speed;
	private float swerveForwardSpeed;
	private float swerveSideSpeed;
	private float rotationSpeed;
	private float radius;
	private float walkSpeedCoefficient;
	private Vector3 playerDirection;
	private Vector3 positiveSwerveDirection;

	void OnEnable() {
		// Subscribe unpausing to when the loading screen is finished.
		SharedVariableManager.onLoadIsFinished += UnPause;
	}

	void OnDisable() {
		// Mandatory unsibscription.
		SharedVariableManager.onLoadIsFinished -= UnPause;
	}

	void Awake() {
		// Calculate movement basis vectors relative to parent.
		playerDirection = transform.parent.InverseTransformDirection(transform.forward);
		positiveSwerveDirection = transform.parent.InverseTransformDirection(transform.right);

		float agentSpeed = sharedVariableManager.agentSpeed;
		maxSpeed = agentSpeed * maxSpeedCoefficient;
		walkSpeedCoefficient = 1 / (maxSpeed - minSpeed) * walkSpeedMultiplier;
		swerveForwardSpeed = agentSpeed * (1 - sharedVariableManager.swerveSideSpeedRatio);
		swerveSideSpeed = agentSpeed * sharedVariableManager.swerveSideSpeedRatio;
		rotationSpeed = sharedVariableManager.rotationSpeed;
		radius = GetComponent<CapsuleCollider>().radius;
		margin = sharedVariableManager.swerveMargin; // The swerving margin is a part of the width.
		sharedVariableManager.playerRadius = radius;
	}

	public void Pause() {
		paused = true;
	}

	public void UnPause() {
		paused = false;
	}

	void FixedUpdate() {
		// Only execute if not paused.
		if (paused) return;

		// Only execute if the initial starting move has been done.
		if (!startedMoving) {
			// Check for the initial input.
			bool initialInput = Input.GetAxisRaw("Vertical") > 0;

			if (initialInput) {
				startedMoving = true;
				FindObjectOfType<RobotController>().UnPause();
				GetComponent<TrajectoryTracker>().StartSampling();
				playerAnimator = gameObject.GetComponentInChildren<Animator>();
				playerAnimator?.SetTrigger("walk");
			} else {
				return;
			}
		}

		// Handle the movement of the player.
		MovePlayer();
	}

	// Take player input and moves the player forward, swerving according to input.
	// Rotation is also controlled by this method.
	private void MovePlayer() {
		// Check for swerving input.
		float swerveInput = Input.GetAxisRaw("Horizontal");
		float accelerationInput = Input.GetAxis("Vertical");

		speed = Mathf.Clamp(speed + accelerationInput * acceleration, minSpeed, maxSpeed);
		playerAnimator?.SetFloat("walkSpeedMultiplier", speed * walkSpeedCoefficient);

		// If there is no swerve input.
		if (swerveInput == 0) {
			// Move player in playerDirection at speed.
			transform.position += playerDirection * speed * Time.fixedDeltaTime;

			// Rotate player.
			transform.rotation = Quaternion.LookRotation(
				Vector3.RotateTowards(transform.forward, playerDirection, rotationSpeed, 0.0f));
		} else { // If there is swerve input, move player along swerve direction.
			sharedVariableManager.playerSwerved = true;

			Vector3 direction = Vector3.Normalize(playerDirection + (positiveSwerveDirection * swerveInput));
			transform.position += direction * speed * Time.fixedDeltaTime;

			// Rotate player.
			transform.rotation = Quaternion.LookRotation(
				Vector3.RotateTowards(transform.forward, direction, rotationSpeed, 0.0f));
		}
	}

	// Checks for collisions between the player and the robot.
	private void OnCollisionEnter(Collision other) {
		if (other.gameObject.tag == "Robot" && !sharedVariableManager.collisionHasHappened) {
			sharedVariableManager.collisionHasHappened = true;
			if (Application.isEditor) Debug.Log("collided");
			StartCoroutine(PauseAfterCollision());
		}
	}

	IEnumerator PauseAfterCollision() {
		Pause();
		RobotController robot = FindObjectOfType<RobotController>();
		robot.Pause();

		Animator robotAnimator = robot.gameObject.GetComponentInChildren<Animator>();

		playerAnimator?.SetTrigger("getHitMiddle");

		yield return new WaitForSecondsRealtime(collisionDelay);

		playerAnimator?.SetTrigger("walk");

		UnPause();
		robot.UnPause();
		robot.OverrideSwerve();
	}
}
