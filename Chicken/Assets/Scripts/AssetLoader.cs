using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;

// Class for loading assets from prefabs and applying them to environment and player and robot agents.
public class AssetLoader : MonoBehaviour {

	private static AssetLoader instance;

	// Handle asset loader instancing between scene loads.
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

	// Instantiates the player avatar with the specified traits at the player
	// agent's positions as a child of the agent. Disables the default capsule mesh renderer.
	public void LoadPlayerAvatar(Gender gender, SkinColor skinColor) {
		// Get the desired player avatar model prefab.
		AssetSelector assetSelector = GameObject.Find("AssetSelector").GetComponent<AssetSelector>();
		GameObject avatarPrefab = assetSelector.GetPlayerAvatar(gender);

		// If there is such a model.
		if (avatarPrefab != null) {
			// Find player agent game object, instantiate the model as a child, apply color and disable the capsule renderer.
			GameObject playerAgent = GameObject.Find("PlayerAgent");
			GameObject playerClone = Instantiate(avatarPrefab, playerAgent.transform);
			assetSelector.ApplyPlayerColor(playerClone, gender, skinColor);
			playerAgent.GetComponent<MeshRenderer>().enabled = false;
		}
	}

	// Instantiates the robot avatar of the specified type at the robot agent's
	// positions as a child of the agent. Disables the default capsule mesh renderer.
	public void LoadRobotAvatar(RobotType type, RobotColor color) {
		// Get the desired robot avatar model prefab.
		AssetSelector assetSelector = GameObject.Find("AssetSelector").GetComponent<AssetSelector>();
		GameObject robotPrefab = assetSelector.GetRobotAvatar(type);

		// If there is such a model.
		if (robotPrefab != null) {
			// Find robot agent game object, instantiate the model as a child, apply color and disable to capsule renderer.
			GameObject robotAgent = GameObject.Find("RobotAgent");
			GameObject robotClone = Instantiate(robotPrefab, robotAgent.transform);
			assetSelector.ApplyRobotColor(robotClone, type, color);
			robotAgent.GetComponent<MeshRenderer>().enabled = false;
			robotAgent.GetComponent<CapsuleCollider>().radius = assetSelector.colliderRadii[type];
			robotAgent.GetComponent<RobotController>().UpdateWidth();
		}
	}

	// Load the desired environment into the scene.
	public void LoadEnvironment(EnvironmentType type) {
		GameObject environmentPrefab = AssetSelector.instance.environments[type];

		if (environmentPrefab != null) {
			Instantiate(environmentPrefab, GameObject.Find("EnvironmentContainer").transform);
		}
	}
}
