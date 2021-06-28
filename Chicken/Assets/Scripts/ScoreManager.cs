using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Class that is responsible for persistently holding the score variable.
public class ScoreManager : MonoBehaviour {
    
    public static readonly float bonusRate = 0.1f;

    private int _score = 0;
    public int score {
        get {
            return _score;
        }

        set {
            prevScore = score;
            _score = value;
        }
    }

    public int prevScore { get; private set; }

    public float bonus {
        get {
            return score * bonusRate;
        }
    }

    // The order of the elements is: collision, only robot swerve, only human swerve, both swerve.
    public Dictionary<MotivationType, int[]> scoreMatrices { get; private set; }

    public static ScoreManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        scoreMatrices = new Dictionary<MotivationType, int[]>();
        scoreMatrices.Add(MotivationType.NONE, new int[] { -4, 3, 0, 2 });
        scoreMatrices.Add(MotivationType.SPEED, new int[] { -4, 4, 0, 2 });
        scoreMatrices.Add(MotivationType.SAFETY, new int[] { -4, 3, 1, 2 });
    }

    private void Start() {
        score = 0;
    }
}
