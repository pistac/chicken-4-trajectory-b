using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundConstructor : MonoBehaviour {

    [SerializeField]
    private GameObject groundTile;

    private void Awake() {
        Vector3 center = Vector3.zero;

        for (int i = 0; i < 10; ++i) {
            float xOffset = -225 + i * 50;
            for (int j = 0; j < 10; ++j) {
                float zOffset = -225 + j * 50;

                Instantiate(groundTile, center + xOffset * Vector3.right + zOffset * Vector3.forward, Quaternion.identity, transform);
            }
        }
    }
}
