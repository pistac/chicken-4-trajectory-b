using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class managing the loading screen and loading time of the trial scene loop.
[RequireComponent(typeof(Image))]
public class OverlayManager : MonoBehaviour {

	private static OverlayManager instance;

	// Handle overlay manager uniqueness.
	void Awake() {
		// If there is no instance, let this be the new instance, otherwise, destroy this object.
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
			return;
		}
	}

	// Various inspector variables.
#pragma warning disable
	[SerializeField]
	private GameObject loadingScreen;
	[SerializeField]
	private GameObject scoreScreen;
	[SerializeField]
	private int fadeFrames;
	[SerializeField]
	private float meanLoadDuration;
	[SerializeField]
	private float loadDurationRange;
	[SerializeField]
	private GameObject noMotivationText;
	[SerializeField]
	private GameObject speedText;
	[SerializeField]
	private GameObject safetyText;
	[SerializeField]
	private GameObject testText;
	[SerializeField]
	private Text scoreText;

	[SerializeField]
	private float _scoreDisplayTime = 0.0f;
	public float scoreDisplayTime { get => _scoreDisplayTime; set => _scoreDisplayTime = value; }

#pragma warning restore

	private float fadeStepDuration;
	private CanvasGroup canvasGroup;

	void Start() {
		canvasGroup = GetComponent<CanvasGroup>();
	}

	// Public method called externally telling the OverlayManager to display a general loading screen.
	public void DisplayLoadingScreen(MotivationType robotMotivation) {
		// The load is starting, signal that the load is not finished.
		GameObject.Find("SharedVariableManager").GetComponent<SharedVariableManager>().loadIsFinished = false;

		// Decide which text to show.
		GameObject text = noMotivationText;
		switch (robotMotivation) {
			case MotivationType.NONE:
				text = noMotivationText;
				break;
			case MotivationType.SAFETY:
				text = safetyText;
				break;
			case MotivationType.SPEED:
				text = speedText;
				break;
		}

		// Draw a loading screen with a randomly generated load time dictated by the mean and the range.
		StartCoroutine(DrawLoadingScreen(text, meanLoadDuration + UnityEngine.Random.Range(-loadDurationRange, loadDurationRange)));
	}

	// Overload of DisplayLoadingScreen to display the test trial's loading screen.
	public void DisplayLoadingScreen() {
		StartCoroutine(DrawLoadingScreen(testText, meanLoadDuration + UnityEngine.Random.Range(-loadDurationRange, loadDurationRange)));
	}

	// Private method called internally that does the actual drawing of the specified loading screen.
	private IEnumerator DrawLoadingScreen(GameObject text, float duration) {
		text.SetActive(true);
		canvasGroup.alpha = 1.0f;

		// Wait for the duration.
		yield return new WaitForSecondsRealtime(duration);

		// Signal that the loading is complete.
		GameObject.Find("SharedVariableManager").GetComponent<SharedVariableManager>().loadIsFinished = true;

		// Fade the image and text back out.
		for (int i = 0; i < fadeFrames; ++i) {
			// Calculate new alpha.
			float progress = ((float) i + 1) / fadeFrames;
			float alpha = Mathf.Lerp(1.0f, 0.0f, progress);

			canvasGroup.alpha = alpha;

			// Wait until next frame.
			yield return new WaitForEndOfFrame();
		}
	}

	public void DisplayScoreScreen(int scoreDifference) {
		// Hide loading screen and show score screen.
		loadingScreen.SetActive(false);
		scoreScreen.SetActive(true);
		canvasGroup.alpha = 1.0f;

		// Show the score text with the absolute score difference formatted in.
		scoreText.text = string.Format(scoreText.text, scoreDifference);
	}
}
