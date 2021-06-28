using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for containing all the participant's comments of different types.
public class Comments {

	public string additionalComments { get; private set; } // Regular, general comments.
	public string technicalIssues { get; private set; } // Comments about technical issues.

	public Comments(string additionalComments, string technicalIssues) {
		this.additionalComments = additionalComments;
		this.technicalIssues = technicalIssues;
	}

	public override string ToString() {
		return "{additionalComments:" + additionalComments + ", technicalIssues:" + technicalIssues + "}";
	}
}
