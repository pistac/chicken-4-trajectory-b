using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enumeration representing the two possible gender expressions for avatars.
public enum Gender {
	FEMALE_PRESENTING,
	MALE_PRESENTING
}

// Enumeration representing the possible skin colors for avatars.
public enum SkinColor {
	BLACK,
	BROWN,
	LIGHT_YELLOW,
	PINK
}

// Data structure to hold information about a player avatar's appearance.
public class Appearance {

	public SkinColor skinColor { get; private set; }
	public Gender gender { get; private set; }

	public Appearance(SkinColor skinColor, Gender gender) {
		this.skinColor = skinColor;
		this.gender = gender;
	}

	public override string ToString() {
		return "{skinColor:" + skinColor + ", gender:" + gender + "}";
	}
}
