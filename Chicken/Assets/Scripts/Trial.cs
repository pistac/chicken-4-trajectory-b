// Enumeration holding the different types of motivations.
public enum MotivationType {
	NONE, // No motivation.
	SPEED, // Motivation indicating it pays off more to reach the goal faster.
	SAFETY // Motivation indicating it pays off more to reach the goal without colliding.
}

// Enumeration holding the different types of trials.
public enum TrialType {
	TEST,
	REGULAR
}

// Data structure to hold information about a trial.
public class Trial {

	public bool collision { get; set; } // Whether a collision happened in the trial.
	public bool robotSwerve { get; set; } // Whether the robot swerved in the trial.
	public EnvironmentType environmentType { get; private set; }
	public float robotPlayerDistance { get; set; } // The distance between robot and player when robot swerved.
	public float robotStartDistance { get; set; } // The distance between robot and start position when robot swerved.
	public int pointsEarned { get; set;  } // The number of points earned this trial.
	public float[][] playerTrajectory { get; set; } // The trajectory of the player in the trial.
	public float[][] robotTrajectory { get; set; } // The trajectory of the robot in the trial.
	public int trialNum { get; private set; } // This trial's number in the order.
	public RobotColor robotColor { get; set; }
	public RobotType robotType { get; private set; } // The type of robot that appears in the trial.
	public MotivationType robotMotivation { get; private set; } // The type of motivation the robot will have in this trial.
	public MotivationType playerMotivation { get; private set; } // The type of motivation the human will have in this trial.
	public TrialType trialType { get; private set; } // The type of trial.

	private static int numTrials = 0; // Static variable to keep track of the total number of trials.

	// Basic constructor that creates a trial of type test with no robot type.
	public Trial() {
		this.environmentType = EnvironmentType.OPEN;
		this.trialType = TrialType.TEST;
		this.robotType = RobotType.TEST;
		this.robotMotivation = MotivationType.NONE;
		this.playerMotivation = MotivationType.NONE;

		// Set trial number to current number of trials, then increment the number of trials.
		// E.g. the first trial will have the trial number 0.
		trialNum = numTrials++;
	}

	// Standard constructor allowing for defining of robot type and trial type.
	public Trial(EnvironmentType environmentType, TrialType trialType, RobotType robotType, MotivationType robotMotivation, MotivationType humanMotivation) {
		this.environmentType = environmentType;
		this.trialType = trialType;
		this.robotType = robotType;
		this.robotMotivation = robotMotivation;
		this.playerMotivation = humanMotivation;

		// Set trial number to current number of trials, then increment the number of trials.
		// E.g. the first trial will have the trial number 0.
		trialNum = numTrials++;
	}

	public override string ToString() {
		return "{collision:" + collision + ", robotSwerve:" + robotSwerve + ", robotPlayerDistance:" + robotPlayerDistance + ", robotStartDistance:" + robotStartDistance + ", playerTrajectory:" + playerTrajectory + ", robotTrajectory:" + robotTrajectory + ", trialNum:" + trialNum + ", robotType:" + robotType + ", robotMotivation:" + robotMotivation + ", humanMotivation:" + playerMotivation + "}";
	}
}
