using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class that when attached to an object with a text component will instert the score at {0} and the bonus at {1}.
[RequireComponent(typeof(Text))]
public class ScoreTextWriter : MonoBehaviour {

    void Start() {
        int score = 0;
        float bonus = 0.0f;
        if (ScoreManager.instance != null) {
            score = ScoreManager.instance.score;
            bonus = ScoreManager.instance.bonus;
        } else if (ExperimentDataManager.instance != null) {
            score = ExperimentDataManager.instance.totalPoints;

            if (score < 0) {
                bonus = 0.0f;
            } else {
                bonus = score * ScoreManager.bonusRate;
            }
        } else {
            score = 0;
            bonus = 0.0f;
        }

        Text textComponent = GetComponent<Text>();
        textComponent.text = string.Format(textComponent.text, score, bonus);
    }
}
