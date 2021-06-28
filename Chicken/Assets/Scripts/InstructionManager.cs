using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionManager : MonoBehaviour {
	
	public void ResumeTrials() {
		trialManager.ResumeAfterInstructions();
	}

#pragma warning disable
	[SerializeField]
	private GameObject noMotivationPage;
	[SerializeField]
	private GameObject speedMotivationPage;
	[SerializeField]
	private GameObject safetyMotivationPage;
#pragma warning restore

	private TrialManager trialManager;

	void Start() {
		trialManager = TrialManager.instance;

		MotivationType motivation = trialManager.trials[trialManager.currentTrialNum].playerMotivation;

		switch (motivation) {
			case MotivationType.NONE:
				noMotivationPage.SetActive(true);
				break;
			case MotivationType.SPEED:
				speedMotivationPage.SetActive(true);
				break;
			case MotivationType.SAFETY:
				safetyMotivationPage.SetActive(true);
				break;
		}
	}
}
