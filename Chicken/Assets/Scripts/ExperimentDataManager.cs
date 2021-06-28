using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Data structure for holding all the different parts of the experiment data:
// avatar appearance, participant information and trial data.
// Handles the arranging and sending of the data to the database.
public class ExperimentDataManager : MonoBehaviour {

	[Serializable]
	public class ExperimentData {
		public bool fromMturk { get; set; }
		public int totalPoints { get; set; }
		public string completionCode { get; set; }
		public string versionGame { get; set; }
		public BrowserData browser { get; set; }
		public Participant participant { get; set; }
		public Appearance appearance { get; set; }
		public Comments comments { get; set; }
		public Trial[] trials { get; set; }

		public ExperimentData(ExperimentDataManager mgr) {
			this.fromMturk = mgr.fromMturk;
			this.totalPoints = mgr.totalPoints;
			this.completionCode = mgr.completionCode;
			this.versionGame = mgr.versionGame;
			this.browser = mgr.browser;
			this.participant = mgr.participant;
			this.appearance = mgr.appearance;
			this.comments = mgr.comments;
			this.trials = mgr.allCompletedTrials;
		}
	}

	[Serializable]
	public class BrowserData {
		public string usedBrowser { get; set; }
		public string deviceWidth { get; set; }
		public string deviceHeight { get; set; }
		public string devicePixelRatio { get; set; }
		public string colorDepth { get; set; }
		public string pixelDepth { get; set; }

		public BrowserData(string splitDataFromBrowser) {
			// Split the data along the token string "SPLIT". In the order of:
			// usedBrowser, device width, device height, device pixel ratio, color depth, pixel depth.
			string[] strBrowser = splitDataFromBrowser.Split(new string[] { "SPLIT" }, StringSplitOptions.None);

			this.usedBrowser = strBrowser[0];
			this.deviceWidth = strBrowser[1];
			this.deviceHeight = strBrowser[2];
			this.devicePixelRatio = strBrowser[3];
			this.colorDepth = strBrowser[4];
			this.pixelDepth = strBrowser[5];
		}
	}

	// Inspector variable for defining the length of the mturk completion code.
	[SerializeField]
	private int completionCodeLength = 10;

	// Alphabet of the random characters available to pick when constructing the completion code.
	private string randomCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

	public static ExperimentDataManager instance;

	public ExperimentData expData { get; set; }

	// Indicates whether the game was loaded as a result of clicking on the survey link in mturk.
	public bool fromMturk = false;

	// The different parts of the experiment data.
	// Appearance is normally set in the intro scene.
	// BrowserData contains the different browser data.
	// Comments are set after the participant data in the ending scene.
	// Participant data is normally set in the ending scene.
	// Playerscore is set at the end of the trial scenes.
	// Completion code is set inside this class and read by the ending manager.
	// Game version is set in the player settings.
	// Trial data is set at the end of the trial scene loop.
	public Appearance appearance { get; set; }
	public BrowserData browser { get; set; }
	public Comments comments { get; set; }
	public Participant participant { get; set; }
	public int totalPoints { get; set; }
	public string completionCode { get; private set; }
	public string versionGame { get; private set; }
	public Trial[] allCompletedTrials { get; set; }

	// The text that is displayed once the data has been sent succesfully without any errors at all.
	private GameObject dataSentText;

	// Handle experiment data manager instancing between scene loads.
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
	}

	// Start function strictly for debugging purposes.
	void Start() {
		// If playing in editor, the player might not start in the intro scene, and needs default experiment data.
		if (Application.isEditor) {
			if (participant == null) participant = new Participant(false, 100, "chimkin", "country", "games", "gender", "education", "robot");
			if (appearance == null) appearance = new Appearance(SkinColor.BLACK, Gender.FEMALE_PRESENTING);
			if (comments == null) comments = new Comments("", "");
			if (allCompletedTrials == null) allCompletedTrials = new Trial[0];
		}
	}

	// Receives the referrer url from the javascript script and checks if it is from mturk.
	// If it is, sets fromMturk.
	public void ProcessReferrer(string url) {
		if (url != null) {
			// If the url contains "mturk", the player was referred from mturk.
			fromMturk = url.Contains("mturk");
		}
	}

	// Receives the browser data from the javascript script and splits it into its components.
	// Sets browserdata.
	public void ProcessBrowserData(string data) {
		if (data != null) {
			// Constructor from string to split.
			browser = new BrowserData(data);
		}
	}

	public void GenerateCompletionCode() {
		completionCode = "";
		for (int i = 0; i < completionCodeLength; ++i) {
			completionCode += randomCharacters[UnityEngine.Random.Range(0, randomCharacters.Length)];
		}
	}

	public string GetExperimentDataJson() {
		expData = new ExperimentData(this);
		return JsonConvert.SerializeObject(this.expData, new StringEnumConverter());
	}

	public void PackAndSendExperimentDataServer() {
		// Find the data sent text.
		dataSentText = GameObject.Find("SentText");

		// Generate mturk survey completion code.
		GenerateCompletionCode();

		// Set the version string.
		versionGame = Application.version;

		// Hide the data sent text.
		dataSentText.SetActive(false);

		string postURL = "https://chicken-kth.herokuapp.com/api/experiments";

		string dataJsonString = this.GetExperimentDataJson();

		// Make sure no data is sent from the editor.
		if (Application.isEditor) {
			Debug.Log(dataJsonString);
			return;
		}

		// Start sending form data in a separate coroutine for asynchronous behavior.
		StartCoroutine(HttpPostJson(postURL, dataJsonString));
	}

	IEnumerator HttpPostJson(string url, string bodyJsonString) {
		// Create temporary UnityWebRequest.
		using (UnityWebRequest request = new UnityWebRequest(url, "POST")) {
			byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
			request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
			request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");

			// Send web request and wait for response.
			yield return request.SendWebRequest();

			Debug.Log("Status Code: " + request.responseCode);

			// Logs potential errors or displays the text indicating no errors.
			// When playing in the editor there should be no errors.
			if (request.isNetworkError || request.isHttpError) {
				Debug.LogError(request.error);
			}

			dataSentText.SetActive(true);
		}
	}
}