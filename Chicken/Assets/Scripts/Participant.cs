using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Data structure to hold a participant's personal information.
public class Participant {

	public bool seenRobotsBefore { get; private set; }
	public int age { get; private set; }
	public string country { get; private set; }
	public string familiarityChicken { get; private set; }
	public string gender { get; private set; }
	public string levelOfEducation { get; private set; }
	public string plausibilityRobotMotivations { get; private set; }
	public string robotExperience { get; private set; }

	public Participant(bool seenRobotsBefore, int age, string familiarityChicken, string country, string gender, string levelOfEducation, string plausibilityRobotMotivations, string robotExperience) {
		this.seenRobotsBefore = seenRobotsBefore;
		this.age = age;
		this.country = country;
		this.familiarityChicken = familiarityChicken;
		this.gender = gender;
		this.levelOfEducation = levelOfEducation;
		this.plausibilityRobotMotivations = plausibilityRobotMotivations;
		this.robotExperience = robotExperience;
	}

	public override string ToString() {
		return "{seenRobotsBefore:" + seenRobotsBefore + ", age:" + age + ", country:" + country + ", familiarityChicken:" + familiarityChicken + ", gender:" + gender + ", levelOfEducation:" + levelOfEducation + ", plausibility:" + plausibilityRobotMotivations + ", robotExperience:" + robotExperience + "}";
	}
}
