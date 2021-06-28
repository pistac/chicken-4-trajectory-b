using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// Class containing static helper functions that are accessible from everywhere
// but fit nowhere else in the code.
public static class HelperFunctions {
	private static System.Random rng = new System.Random();

	private static float spareGaussian;
	private static bool hasSpareGaussian = false;

	// Calculates and skews the swerve distance depending on the type of motivation.
	public static float CalculateSwerveDistance(float meanDistance, float standardDeviation, MotivationType motivation) {
		// Generate Gaussian regularly.
		float gaussian = GenerateGaussian(meanDistance, standardDeviation);
		
		// Switch depending on motivation type.
		switch (motivation) {
			case MotivationType.SPEED:
				// If speed is the motivation, only take values that are lesser than the mean.
				return meanDistance - Mathf.Abs(gaussian - meanDistance);
			case MotivationType.SAFETY:
				// If safety is the motivation, only take values that are greater than the mean.
				return meanDistance + Mathf.Abs(gaussian - meanDistance);
			default:
				// In all other cases, take the unbiased Gaussian.
				return gaussian;
		}
	}

	// Uses the Marsaglia polar method to generate a Gaussian distributed number with given mean and standard deviation.
	// Optimized to use both generated numbers from Marsaglia.
	public static float GenerateGaussian(float mean, float standardDeviation) {
		if (hasSpareGaussian) {
			hasSpareGaussian = false;
			return spareGaussian * standardDeviation * (UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1) + mean;
		} else {
			float u, v, s;

			do {
				u = UnityEngine.Random.Range(0.0f, 1.0f);
				v = UnityEngine.Random.Range(0.0f, 1.0f);
				s = u * u + v * v;
			} while (s >= 1 || s == 0);

			s = Mathf.Sqrt((float) -2.0 * Mathf.Log(s) / s);
			spareGaussian = v * s;
			hasSpareGaussian = true;
			return mean + standardDeviation * u * s * (UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1);
		}
	}

	// Random list shuffle method proposed by stackoverflow user grenade:
	// https://stackoverflow.com/questions/273313/randomize-a-listt
	public static void Shuffle<T>(this IList<T> list) {
		int n = list.Count;
		while (n > 1) {
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}
