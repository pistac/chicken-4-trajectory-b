using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

// Class that manages everything that requires scripting in the intro scene. Similar to EndingManager.
public class IntroManager : MonoBehaviour {

	// Inspector variables for all the required UI objects.
#pragma warning disable
	[SerializeField]
	private ExperimentDataManager experimentDataManager;
	[SerializeField]
	private GameObject initialInstructionsPage;
	[SerializeField]
	private GameObject longInstructionsPage;
	[SerializeField]
	private GameObject appearancePage;
	[SerializeField]
	private GameObject continueButton;
	[SerializeField]
	private ToggleGroup genderExpressionToggleGroup;
	[SerializeField]
	private GameObject genderExpressionErrorText;
	[SerializeField]
	private GameObject skinColorErrorText;
#pragma warning disable
	// The amount of time in seconds that the continue button is hidden in certain scenes.
	[SerializeField]
	private float continueDelay = 1.0f;

	private bool listenForKey = false;
	private bool setSkinColor = false; // Indicates whether skin color has been set.
	private bool[] hideButtonPage; // Bool array defining which pages should initially hide continue button.
	private Button continueButtonComponent;
	private GameObject[] orderedPages; // Array of references to pages in the order they should appear.
	private int currentPageIndex;
	private SkinColor selectedSkinColor;

	// Handle initialization.
	void Start() {
		// Get the experiment data manager.
		experimentDataManager = GameObject.Find("ExperimentDataManager").GetComponent<ExperimentDataManager>();
		continueButtonComponent = continueButton.GetComponent<Button>();

		// Order the pages and define which ones should hide the continue button initially.
		orderedPages = new GameObject[] { initialInstructionsPage, longInstructionsPage, appearancePage };
		hideButtonPage = new bool[] { true, true, false };

		// Find the first active page and set the currentPageIndex. This is only necessary if the scene is set up incorrectly.
		// Guarantees that at most one page is open at the start.
		bool foundActive = false; // Indicates whether a first active page in the hierarchy has been found.
		for (int i = 0; i < orderedPages.Length; ++i) {
			GameObject page = orderedPages[i];
			// If no active page has been found yet and this page is active == the first active page is found.
			if (!foundActive && page.activeSelf) {
				currentPageIndex = i;
				foundActive = true;
			} else if (page.activeSelf) { // This page is active but not the first active one found.
				page.SetActive(false);
			}
		}
	}

	void Update() {
		// Listen for presses on the verification key if the flag says to.
		if (listenForKey) {
			if (Input.GetAxisRaw("Vertical") > 0) {
				// If continue button has been made non-interactable.
				if (!continueButtonComponent.interactable) {
					// Make the continue button interactable.
					continueButtonComponent.interactable = true;
				}
				// Stop listening for the key and register that the key has been pressed.
				listenForKey = false;
			}
		}
	}

	// Called when the continue button is clicked.
	public void OnClickContinueButton() {
		// If else construction to check which page is active and what to do in each case.
		// If else is appropriate since it is guaranteed that only one page is active at any time.
		if (initialInstructionsPage.activeSelf) { // Initial instructions page is active.
												  // Simply go to the next page after instructions.
			NextPage();
			continueButtonComponent.interactable = false;
		} else if (longInstructionsPage.activeSelf) { // Long instructions page is active.
													  // If the key has been pressed when given the instructions to do so, the button will be active.
													  // Just go to the next page.
			NextPage();
		} else if (appearancePage.activeSelf) { // Player avatar appearance picker page is active.
												// If entered data is valid.
			if (AppearanceIsCorrect()) {
				// Get the gender expression from the toggle group.
				string genderExpressionString = genderExpressionToggleGroup.ActiveToggles().ToList()[0].transform.Find("Label").GetComponent<Text>().text;
				Gender genderExpression = String.Equals(genderExpressionString, "female") ? Gender.FEMALE_PRESENTING : Gender.MALE_PRESENTING;

				// Set the appearance in the experiment data manager.
				experimentDataManager.appearance = new Appearance(selectedSkinColor, genderExpression);
				SceneManager.LoadScene("TrialScene");
			} else {
				// Display or hide error text.
				if (!setSkinColor) {
					skinColorErrorText.SetActive(true);
				} else {
					skinColorErrorText.SetActive(false);
				}

				if (!genderExpressionToggleGroup.AnyTogglesOn()) {
					genderExpressionErrorText.SetActive(true);
				} else {
					genderExpressionErrorText.SetActive(false);
				}
			}
		}
	}

	// Called when black skin color is selected.
	public void OnClickBlackSkinColorButton() {
		selectedSkinColor = SkinColor.BLACK;
		setSkinColor = true;
	}

	// Called when brown skin color is selected.
	public void OnClickBrownSkinColorButton() {
		selectedSkinColor = SkinColor.BROWN;
		setSkinColor = true;
	}

	// Called when light yellow skin color is selected.
	public void OnClickLightYellowSkinColorButton() {
		selectedSkinColor = SkinColor.LIGHT_YELLOW;
		setSkinColor = true;
	}

	// Called when pink skin color is selected.
	public void OnClickPinkSkinColorButton() {
		selectedSkinColor = SkinColor.PINK;
		setSkinColor = true;
	}

	// Inactivates the current page and activates the next page according to predefined order.
	// Wraps around on index overflow.
	private void NextPage() {
		orderedPages[currentPageIndex].SetActive(false);
		orderedPages[++currentPageIndex % orderedPages.Length].SetActive(true);

		// If the next page requires participants to wait, hide the button.
		if (hideButtonPage[currentPageIndex]) {
			// Handle the special case where the button is not hidden, but requires the
			// key to be pressed first.
			if (orderedPages[currentPageIndex] == longInstructionsPage) {
				listenForKey = true;
			} else {
				StartCoroutine(HideThenShowButton());
			}
		}
	}

	// Hides continue button, waits for a time period of predefined length then shows button.
	IEnumerator HideThenShowButton() {
		continueButton.SetActive(false);
		yield return new WaitForSecondsRealtime(continueDelay);
		continueButton.SetActive(true);
	}

	// Check if the participant has entered data in the fields.
	private bool AppearanceIsCorrect() {
		return setSkinColor && genderExpressionToggleGroup.AnyTogglesOn();
	}
}
