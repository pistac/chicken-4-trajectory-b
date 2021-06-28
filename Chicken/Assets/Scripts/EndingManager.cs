using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Runtime.InteropServices;

// Class that manages everything that requires scripting in the ending scene. Similar to IntroManager.
public class EndingManager : MonoBehaviour {

	// Javascript imported function to show dialog.
	[DllImport("__Internal")]
	private static extern void ShowDialog(string completionCode);

	// Inspector variables for all the required UI objects.
#pragma warning disable
	[SerializeField]
	private ExperimentDataManager experimentDataManager;
	[SerializeField]
	private GameObject participantDataPage1;
	[SerializeField]
	private GameObject participantDataPage2;
	[SerializeField]
	private GameObject participantDataPage3;
	[SerializeField]
	private GameObject additionalCommentsPage;
	[SerializeField]
	private GameObject completionCodePage;
	[SerializeField]
	private GameObject continueButton;
	[SerializeField]
	private ToggleGroup genderToggleGroup;
	[SerializeField]
	private GameObject genderErrorText;
	[SerializeField]
	private Dropdown ageDropdown;
	[SerializeField]
	private ToggleGroup levelOfEducationToggleGroup;
	[SerializeField]
	private GameObject levelOfEducationErrorText;
	[SerializeField]
	private InputField countryInputField;
	[SerializeField]
	private GameObject countryErrorText;
	[SerializeField]
	private ToggleGroup chickenExperienceToggleGroup;
	[SerializeField]
	private GameObject chickenExperienceErrorText;
	[SerializeField]
	private ToggleGroup plausibilityToggleGroup;
	[SerializeField]
	private GameObject plausibilityErrorText;
	[SerializeField]
	private ToggleGroup robotExperienceToggleGroup;
	[SerializeField]
	private GameObject robotExperienceErrorText;
	[SerializeField]
	private ToggleGroup seenBeforeToggleGroup;
	[SerializeField]
	private GameObject seenBeforeErrorText;
	[SerializeField]
	private InputField commentsInputField;
	[SerializeField]
	private InputField technicalIssuesInputField;
#pragma warning disable

	private GameObject[] orderedPages; // Array of references to pages in the order they should appear.
	private int currentPageIndex;

	// Temporary variables to hold values from previous pages.
	private string tempGender;
	private int tempAge;
	private string tempLevelOfEducation;
	private string tempCountry;

	// Handle initialization.
	void Start() {
		// Get the experiment data manager.
		experimentDataManager = GameObject.Find("ExperimentDataManager").GetComponent<ExperimentDataManager>();

		// Order the pages.
		orderedPages = new GameObject[] { participantDataPage1, participantDataPage2, participantDataPage3, additionalCommentsPage, completionCodePage };

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

	// Called when the continue button is clicked.
	public void OnClickContinueButton() {
		// If else construction to check which page is active and what to do in each case.
		// If else is appropriate since it is guaranteed that only one page is active at any time.
		if (participantDataPage1.activeSelf) { // Participant data entry page 1 is active.
			if (ParticipantData1IsCorrect()) {
				// Save the page's data temporarily.
				tempGender = genderToggleGroup.ActiveToggles().ToList()[0].transform.Find("Label").GetComponent<Text>().text;
				tempAge = int.Parse(ageDropdown.options[ageDropdown.value].text);
				NextPage();
			} else {
				genderErrorText.SetActive(true);
			}
		} else if (participantDataPage2.activeSelf) { // Participant data entry page 2 is active.
													  // If entered data is valid.
			if (ParticipantData2IsCorrect()) {
				// Save the page's data temporarily.
				tempLevelOfEducation = levelOfEducationToggleGroup.ActiveToggles().ToList()[0].transform.Find("Label").GetComponent<Text>().text;
				tempCountry = countryInputField.text;
				NextPage();
			} else {
				if (countryInputField.text == "") {
					countryErrorText.SetActive(true);
				} else {
					countryErrorText.SetActive(false);
				}

				if (!levelOfEducationToggleGroup.AnyTogglesOn()) {
					levelOfEducationErrorText.SetActive(true);
				} else {
					levelOfEducationErrorText.SetActive(false);
				}
			}
		} else if (participantDataPage3.activeSelf) { // Participant data entry page 3 is active.
			if (ParticipantData3IsCorrect()) {
				// Save the page's data temporarily.
				string tempChickenExperience = chickenExperienceToggleGroup.ActiveToggles().ToList()[0].transform.Find("Label").GetComponent<Text>().text;
				string tempPlausibility = plausibilityToggleGroup.ActiveToggles().ToList()[0].transform.Find("Label").GetComponent<Text>().text;
				string tempRobotExperience = robotExperienceToggleGroup.ActiveToggles().ToList()[0].transform.Find("Label").GetComponent<Text>().text;
				string tempSeenBeforeString = seenBeforeToggleGroup.ActiveToggles().ToList()[0].transform.Find("Label").GetComponent<Text>().text;
				bool tempSeenBefore = String.Equals(tempSeenBeforeString, "yes") ? true : false;

				// Set the participant in the experiment data manager.
				experimentDataManager.participant = new Participant(
				tempSeenBefore,
				tempAge,
				tempChickenExperience,
				tempCountry,
				tempGender,
				tempLevelOfEducation,
				tempPlausibility,
				tempRobotExperience
				);

				NextPage();
			} else {
				if (!chickenExperienceToggleGroup.AnyTogglesOn()) {
					chickenExperienceErrorText.SetActive(true);
				} else {
					chickenExperienceErrorText.SetActive(false);
				}

				if (!plausibilityToggleGroup.AnyTogglesOn()) {
					plausibilityErrorText.SetActive(true);
				} else {
					plausibilityErrorText.SetActive(false);
				}

				if (!robotExperienceToggleGroup.AnyTogglesOn()) {
					robotExperienceErrorText.SetActive(true);
				} else {
					robotExperienceErrorText.SetActive(false);
				}

				if (!seenBeforeToggleGroup.AnyTogglesOn()) {
					seenBeforeErrorText.SetActive(true);
				} else {
					seenBeforeErrorText.SetActive(false);
				}
			}
		} else if (additionalCommentsPage.activeSelf) { // Additional comments page is active.
														// Set the comments in the experiment data manager.
			experimentDataManager.comments = new Comments(
			  commentsInputField.text,
			  technicalIssuesInputField.text
			  );

			// Move to next page and disable continue button.
			NextPage();
			continueButton.SetActive(false);

			// Tell experiment data manager to start packing and sending data.
			experimentDataManager.PackAndSendExperimentDataServer();
		}
	}

	// Inactivates the current page and activates the next page according to predefined order.
	// Wraps around on index overflow.
	private void NextPage() {
		orderedPages[currentPageIndex].SetActive(false);
		orderedPages[++currentPageIndex % orderedPages.Length].SetActive(true);
	}

	// Check if the participant has entered data correctly in the fields of page 1.
	private bool ParticipantData1IsCorrect() {
		// The only field that does not have an acceptable default value is gender.
		return genderToggleGroup.AnyTogglesOn();
	}

	// Check if the participant has entered data correctly in the fields of page 2.
	private bool ParticipantData2IsCorrect() {
		return countryInputField.text != "" && levelOfEducationToggleGroup.AnyTogglesOn();
	}

	// Check if the participant has entered data correctly in the fields of page 3.
	private bool ParticipantData3IsCorrect() {
		return chickenExperienceToggleGroup.AnyTogglesOn() && plausibilityToggleGroup.AnyTogglesOn() && robotExperienceToggleGroup.AnyTogglesOn() && seenBeforeToggleGroup.AnyTogglesOn();
	}

	// Function to be called by the show dialog button on click.
	public void OnClickShowDialogButton() {
		if (!Application.isEditor) {
			ShowDialog(experimentDataManager.completionCode);
		}
	}
}
