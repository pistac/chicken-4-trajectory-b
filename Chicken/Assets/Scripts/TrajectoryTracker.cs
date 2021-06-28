using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryTracker : MonoBehaviour {

#pragma warning disable
    [SerializeField]
    private float samplingFrequency = 0.1f;
#pragma warning restore

    private bool startedSampling = false;
    private List<float[]> samples;

    public float[][] trajectory { 
        get {
            return samples.ToArray();
        }
    }

    private void Start() {
        samples = new List<float[]>();
    }

    public void StartSampling() {
        if (!startedSampling) {
            startedSampling = true;
            InvokeRepeating("Sample", samplingFrequency, samplingFrequency);
        }
    }

    private void Sample() {
        Vector2 velocity = Vector2.zero;
        
        if (samples.Count > 0) {
            velocity.x = (transform.position.x - samples[samples.Count - 1][0]) / samplingFrequency;
            velocity.y = (transform.position.z - samples[samples.Count - 1][1]) / samplingFrequency;
        }

        samples.Add(new float[] { transform.position.x, transform.position.z, velocity.x, velocity.y });
    }
}
