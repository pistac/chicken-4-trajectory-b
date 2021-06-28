using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls main camera behavior.
public class CameraController : MonoBehaviour {

	private Transform playerTransform;
	private Vector3 cameraOffset;

	void Start() {
		playerTransform = GameObject.FindWithTag("Player").transform;
		// Find the offset between camera and player at the start of the game.
		cameraOffset = transform.position - playerTransform.position;
	}

	void Update() {
		// Update camera position according to player position and camera offset.
		transform.position = playerTransform.position + cameraOffset;
	}
}
