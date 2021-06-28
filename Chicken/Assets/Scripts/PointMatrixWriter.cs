using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointMatrixWriter : MonoBehaviour {

    public Text points0;
    public Text points1;
    public Text points2;
    public Text points3;

    private void Start() {
        Invoke("TimeActions", 0.1f);
    }

    void TimeActions() {
        MotivationType motivation = TrialManager.instance.trials[TrialManager.instance.currentTrialNum].playerMotivation;
        int[] scoreMatrix = ScoreManager.instance.scoreMatrices[motivation];

        points0.text = string.Format(points0.text, scoreMatrix[0]);
        points1.text = string.Format(points1.text, scoreMatrix[1]);
        points2.text = string.Format(points2.text, scoreMatrix[2]);
        points3.text = string.Format(points3.text, scoreMatrix[3]);
    }
}
