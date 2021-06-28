using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls the robot camera location and rotation.
public class RobotCameraController : MonoBehaviour {

	private Transform robotTransform;
	private Vector3 cameraOffset;

	void Start() {
		robotTransform = GameObject.FindWithTag("Robot").transform;
		// Find the offset between camera and player at the start of the game.
		cameraOffset = transform.position - robotTransform.position;
	}

	void Update() {
		// Update camera position according to player position and camera offset.
		transform.position = robotTransform.position + cameraOffset;
	}
}
