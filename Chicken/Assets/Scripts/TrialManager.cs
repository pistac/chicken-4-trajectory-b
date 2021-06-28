using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Class that keeps track of the trials, runs through them in sequence, stores the data
// and then reports the data to the ExperimentDataManager. This is the main controller of
// the trial scene loop.
public class TrialManager : MonoBehaviour {

#pragma warning disable
	[SerializeField]
	private bool debugTrials = false;
	[SerializeField]
	private string noMotivationText;
	[SerializeField]
	private string speedMotivationText;
	[SerializeField]
	private string safetyMotivationText;
#pragma warning restore

	public List<Trial> trials { get; private set; } // List of trial blocks, together containing all the trials.
	public int currentTrialNum { get; private set; } = 0; // Keeps track of which trial is currently being played.

	public void ResumeAfterInstructions() {
		// Load the trial scene again after the instruction scene is finished.
		SceneManager.LoadScene("TrialScene");
	}

	private AssetLoader assetLoader;
	private ExperimentDataManager experimentDataManager;
	private HashSet<int> instructionCutoffIndices; // To keep track of which scene indices should be preceded by and instruction page.
	private OverlayManager overlayManager;
	private SharedVariableManager sharedVariableManager;
	private Text robotViewText;

	public static TrialManager instance;

	// Handle trial manager instancing between scene loads.
	void Awake() {
		// If there is no instance, let this be the new instance, otherwise, destroy this object.
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
			return;
		}

		// If this object was set as the instance, make sure it is not destroyed on scene loads.
		DontDestroyOnLoad(gameObject);

		// Add cutoff scene indices.
		instructionCutoffIndices = new HashSet<int>();
		instructionCutoffIndices.Add(1);
		instructionCutoffIndices.Add(7);
		instructionCutoffIndices.Add(13);
	}

	void OnEnable() {
		// Subscribe trial finishing and further actions to game over event.
		SharedVariableManager.onGameIsOver += FinishTrialAndContinue;
	}

	void OnDisable() {
		// Mandatory unsubscriptions.
		SceneManager.sceneLoaded -= NewSceneActions;
		SharedVariableManager.onGameIsOver -= FinishTrialAndContinue;
	}

	void Start() {
		// Unless the start function initializations are timed correctly, there is a
		// null reference exception in builds.
		StartCoroutine(TimeActionsWithFirstSceneLoaded());
	}

	IEnumerator TimeActionsWithFirstSceneLoaded() {
		// Make sure that this script is executed after all the others.
		yield return new WaitForEndOfFrame();

		// Find all the objects required by this class.
		assetLoader = GameObject.Find("AssetLoader").GetComponent<AssetLoader>();
		experimentDataManager = GameObject.Find("ExperimentDataManager").GetComponent<ExperimentDataManager>();
		overlayManager = GameObject.Find("OverlayCanvas").GetComponent<OverlayManager>();
		sharedVariableManager = GameObject.Find("SharedVariableManager").GetComponent<SharedVariableManager>();
		robotViewText = GameObject.Find("RobotViewText").GetComponent<Text>();

		// Set up list of robot motivations.
		List<MotivationType> robotMotivations = new List<MotivationType>();
		robotMotivations.Add(MotivationType.NONE);
		robotMotivations.Add(MotivationType.NONE);
		robotMotivations.Add(MotivationType.SPEED);
		robotMotivations.Add(MotivationType.SPEED);
		robotMotivations.Add(MotivationType.SAFETY);
		robotMotivations.Add(MotivationType.SAFETY);

		// Set up list of human motivations.
		List<MotivationType> humanMotivations = new List<MotivationType>();
		humanMotivations.Add(MotivationType.NONE);
		humanMotivations.Add(MotivationType.SPEED);
		humanMotivations.Add(MotivationType.SAFETY);
		humanMotivations.Shuffle();

		// Trial list construction.
		// Initialize trials list as empty.
		trials = new List<Trial>();

		// Construct a trial block for each human motivation.
		foreach (MotivationType humanMotivation in humanMotivations) {

			// Shuffle robot motivations.
			// This ensures that all robot motivation appear as wanted in each block, but in random order.
			robotMotivations.Shuffle();

			// Add trials.
			for (int i = 0; i < robotMotivations.Count; ++i) {
				Trial trial = new Trial(EnvironmentType.OPEN, TrialType.REGULAR, RobotType.PEPPER, robotMotivations[i], humanMotivation);
				
				// Pick robot color dending on robot motivation.
				switch (robotMotivations[i]) {
					case MotivationType.NONE:
						trial.robotColor = RobotColor.WHITE;
						break;
					case MotivationType.SPEED:
						trial.robotColor = RobotColor.RED;
						break;
					case MotivationType.SAFETY:
						trial.robotColor = RobotColor.PURPLE;
						break;
				} 
				trials.Add(trial);
			}
		}

		// Add the test trial to the front of the list (should always be first!).
		trials.Insert(0, new Trial());
		// End trial list construction.

		// Load the trial environment.
		assetLoader.LoadEnvironment(trials[currentTrialNum].environmentType);

		// Display the first trial's loading screen, which is the test trial.
		overlayManager.DisplayLoadingScreen();

		// Hide the score matrix.
		GameObject.Find("Table").SetActive(false);
		GameObject.Find("TableLines").SetActive(false);

		// Subscribe the new scene actions to future scene loads.
		// Needs to be here in order to not perform NewSceneActions for the first trial.
		SceneManager.sceneLoaded += NewSceneActions;
	}

	// Actions to take to finish a trial after the game is over.
	void FinishTrialAndContinue() {
		PackTrialData();
		SubmitScore();
		StartCoroutine(AnnounceScoreThenContinue());
	}

	// Tells the overlay manager to display the score screen, wait for the score screen duration and then continue to the next trial.
	IEnumerator AnnounceScoreThenContinue() {
		overlayManager.DisplayScoreScreen(ScoreManager.instance.score - ScoreManager.instance.prevScore);
		yield return new WaitForSeconds(overlayManager.scoreDisplayTime);
		NextTrial();
	}

	// Actions to take to start a trial properly after the trial scene is loaded.
	private void NewSceneActions(Scene scene, LoadSceneMode mode) {
		if (scene.name.Equals("TrialScene")) {
			StartCoroutine(TimeActionsWithSceneLoaded());
		}
	}

	// Reloads the managers and displays the loading screen after scene reloads.
	// Ensures correct timing between scene load and finding of objects.
	IEnumerator TimeActionsWithSceneLoaded() {
		// Time the actions.
		yield return new WaitForEndOfFrame();

		// Update the managers to the ones loaded in the new scene.
		sharedVariableManager = GameObject.Find("SharedVariableManager").GetComponent<SharedVariableManager>();
		overlayManager = GameObject.Find("OverlayCanvas").GetComponent<OverlayManager>();
		robotViewText = GameObject.Find("RobotViewText").GetComponent<Text>();

		// Set the approprite robot view text.
		switch (trials[currentTrialNum].robotMotivation) {
			case MotivationType.NONE:
				robotViewText.text = noMotivationText;
				break;
			case MotivationType.SPEED:
				robotViewText.text = speedMotivationText;
				break;
			case MotivationType.SAFETY:
				robotViewText.text = safetyMotivationText;
				break;
		}

		// Display the loading screen.
		overlayManager.DisplayLoadingScreen(trials[currentTrialNum].robotMotivation);

		// Load the appropriate models for the player and robot avatars.
		assetLoader.LoadPlayerAvatar(experimentDataManager.appearance.gender, experimentDataManager.appearance.skinColor);
		assetLoader.LoadRobotAvatar(trials[currentTrialNum].robotType, trials[currentTrialNum].robotColor);

		// Load the appropriate environment.
		assetLoader.LoadEnvironment(trials[currentTrialNum].environmentType);
	}

	// Collects the final data in a Trial data structure after a trial has finished.
	private void PackTrialData() {
		// Get the current trial.
		Trial currentTrial = trials[currentTrialNum];

		// Pack information about whether collision and swerve has happened into the trial data.
		currentTrial.collision = sharedVariableManager.collisionHasHappened;
		currentTrial.robotSwerve = sharedVariableManager.robotSwerved;

		// If robot swerved, pack the distances into the trial data.
		if (currentTrial.robotSwerve) {
			currentTrial.robotPlayerDistance = sharedVariableManager.robotPlayerSwerveDistance;
			currentTrial.robotStartDistance = sharedVariableManager.robotStartSwerveDistance;
		} else {
			currentTrial.robotPlayerDistance = -1.0f;
			currentTrial.robotStartDistance = -1.0f;
		}

		// Collect the trajectory from the TrajectoryTracker(s).
		currentTrial.playerTrajectory = GameObject.Find("PlayerAgent").GetComponent<TrajectoryTracker>().trajectory;
		currentTrial.robotTrajectory = GameObject.Find("RobotAgent").GetComponent<TrajectoryTracker>().trajectory;
	}

	// Update the score from the trial that just finished.
	private void SubmitScore() {
		// Do not submit a score for the test trial.
		if (currentTrialNum < 1) {
			return;
		}

		ScoreManager scoreManager = ScoreManager.instance;
		
		// Assume that both swerved and there was no collision.
		int scoreDifference = scoreManager.scoreMatrices[trials[currentTrialNum].playerMotivation][3];
		
		// Try to disprove the assumption by progressively weaker arguments.
		if (sharedVariableManager.collisionHasHappened) {
			// If there was a collision.
			scoreDifference = scoreManager.scoreMatrices[trials[currentTrialNum].playerMotivation][0];
		} else if (!sharedVariableManager.playerSwerved) {
			// If player did not swerve and there was no collision.
			scoreDifference = scoreManager.scoreMatrices[trials[currentTrialNum].playerMotivation][1];
		} else if (!sharedVariableManager.robotSwerved) {
			// If the player swerved, the robot did not swerve and there was no collision.
			scoreDifference = scoreManager.scoreMatrices[trials[currentTrialNum].playerMotivation][2];
		}
		// Otherwise, the player and robot swerved and there was no collision.

		trials[currentTrialNum].pointsEarned = scoreDifference;

		scoreManager.score += scoreDifference;
	}

	// Defines behavior to be executed to move the experiment on after a trial has finished.
	private void NextTrial() {
		// Move on to next trial.
		currentTrialNum++;

		// If there are trials left, go to next trial.
		if (currentTrialNum < trials.Count) {
			// Check if there should be an instruction page.
			if (instructionCutoffIndices.Contains(currentTrialNum)) {
				// Load instruction page scene.
				SceneManager.LoadScene("InstructionScene");
			} else {
				// Reload the scene with the new trial.
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
		} else { // If there are no trials left:
				 // Set the trials property of the experiment data manager.
			experimentDataManager.allCompletedTrials = trials.ToArray();

			// Send the score to the experiment data manager.
			experimentDataManager.totalPoints = ScoreManager.instance.score;

			// Show collision statistics.
			if (Application.isEditor) {
				int numCollisions = 0;

				foreach (Trial trial in trials) {
					if (trial.collision) ++numCollisions;
				}

				Debug.Log("Number of trials: " + trials.Count);
				Debug.Log("Number of collisions: " + numCollisions + ", collision rate: " + ((float) numCollisions / trials.Count));
			}

			// Load ending scene, initiating packing and sending of data.
			SceneManager.LoadScene("EndingScene");

			// Clean up persistent objects.
			Destroy(assetLoader.gameObject);
			Destroy(ScoreManager.instance.gameObject);
			Destroy(gameObject);
		}
	}

	public void VisualizeTrajectory() {
		float[][] trajectory = GameObject.Find("PlayerAgent").GetComponent<TrajectoryTracker>().trajectory;

		for (int i = 1; i < trajectory.Length; ++i) {
			Vector3 prevPoint = new Vector3(trajectory[i - 1][0], 1.0f, trajectory[i - 1][1]);
			Vector3 currentPoint = new Vector3(trajectory[i][0], 1.0f, trajectory[i][1]);
			Vector3 velocity = new Vector3(trajectory[i][2], 0.0f, trajectory[i][3]);

			Debug.DrawLine(prevPoint, currentPoint, Color.white, 10.0f, true);

			DrawArrow(currentPoint, velocity, Color.red, 10.0f);
		}
	}

	public void DrawArrow(Vector3 start, Vector3 direction, Color color, float time) {
		Vector3 offset = direction.normalized;
		Vector3 headOffset = new Vector3(offset.z, offset.y, -offset.x) * 0.3f;
		Vector3 arrowEnd = start + offset;

		Debug.DrawLine(start, arrowEnd, color, time);
		Debug.DrawLine(arrowEnd, arrowEnd + (headOffset - offset) * 0.3f, color, time);
		Debug.DrawLine(arrowEnd, arrowEnd + (-headOffset - offset) * 0.3f, color, time);
	}
}
