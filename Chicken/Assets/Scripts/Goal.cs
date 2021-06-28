using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

	[SerializeField]
	private float gameOverDelay = 1.0f;

    private bool localGameOver = false;

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			// Check if experiment is over.
			if (!localGameOver && !SharedVariableManager.instance.gameIsOver) {
				localGameOver = true;

				// Commence game over sequence.
				StartCoroutine(GameOverSequence());
			}
		}
	}

	// Waits for a specified time period, then sets the gameIsOver flag.
	IEnumerator GameOverSequence() {
		if (Application.isEditor) {
			TrialManager.instance.VisualizeTrajectory();
		}

		// Wait for the game over delay.
		yield return new WaitForSecondsRealtime(gameOverDelay);

		// Pause the movement and animation of robot and player avatars.
		PlayerController player = FindObjectOfType<PlayerController>();
		player.Pause();
		RobotController robot = FindObjectOfType<RobotController>();
		robot.Pause();

		Animator playerAnimator = player.gameObject.GetComponentInChildren<Animator>();
		Animator robotAnimator = robot.gameObject.GetComponentInChildren<Animator>();

		if (playerAnimator != null) {
			playerAnimator.SetTrigger("idleStand");
		}

		if (robotAnimator != null && robotAnimator.gameObject.name.Contains("Sophia")) {
			robotAnimator.SetTrigger("idleStand");
		}

		// Signal that the game is over.
		SharedVariableManager.instance.gameIsOver = true;
	}
}
