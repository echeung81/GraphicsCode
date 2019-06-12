using UnityEngine;
using System.Collections;

public abstract class Trajectory {

	protected Vector3 startPos;
	protected Vector3 endPos;

	public bool followingTarget;
	public GameObject targetObject;

	public float targetRadius; 
	public float speed;
	public float speedAccelFactor = 0;

	public virtual Vector3 StartPos { 
	
		get { return startPos; }
		set { startPos = value; }
	}

	public virtual Vector3 EndPos {

		get { return endPos; }
		set { endPos = value; }
	}

	public float delayToNextPath = 0; //after we've reached this trajectory, amount of time to delay before we move on to the next one

	public bool hitPathAndWaitingForDelay = false;

	public Trajectory() {
	}

	public Trajectory(Vector3 startPos, Vector3 endPos) {
		this.startPos = startPos;
		this.endPos = endPos;
		followingTarget = false;
	}

	public Trajectory(Vector3 startPos, GameObject endTarget) {

		this.startPos = startPos;
		this.targetObject = endTarget;
		followingTarget = true;
	}

	//@bool: true if we are within target radius of our target, false otherwise
	public abstract bool UpdateTrajectory (GameObject movingObject);

	public static Quaternion LookAt2D(Vector2 forward) {
		
		return Quaternion.Euler (0, 0, Mathf.Atan2 (forward.y, forward.x) * Mathf.Rad2Deg);
	}

	public virtual void ResetTrajectory() {

		delayToNextPath = 0;
		hitPathAndWaitingForDelay = false;
	}


}
