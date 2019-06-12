using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathContainer : MonoBehaviour {

	Queue<Trajectory> paths = new Queue<Trajectory>();

	public bool destroyOnReach; //do we destroy the object once we reach the target?
	public bool destroyOnOrphan = true; //if the object we are following is destroyed for any of the trajectories, what happens to us?

	private Trajectory currTrajectory;

	public delegate void ReachAction();
	public ReachAction reachAction;

	private bool reachActionFired;

	public Trajectory CurrentTrajectory {
		get { return currTrajectory; }
	}

	void Start() {
		currTrajectory = null;
		reachActionFired = false;
	}

	public void SetReachAction(ReachAction reachActionFunc) {

		reachAction = reachActionFunc;
		reachActionFired = false;
	}

	void FixedUpdate() {

		if (currTrajectory == null) {

			if (paths.Count == 0) {
				//last trajectory reached and no other trajectories to follow
				//get component path action. execute the path action for 
				if (reachAction != null && !reachActionFired) {

					reachActionFired = true;
					reachAction.Invoke ();
				}


				if (destroyOnReach) {
					Destroy (gameObject);
				}
				return;

			} else {

				currTrajectory = paths.Dequeue ();
			}
		}

		//if the trajectory is following an object, handle the case when the target is gone
		if (currTrajectory.followingTarget && currTrajectory.targetObject == null) {

			if (destroyOnOrphan) {
				Destroy (gameObject);
			} else {
				//get off this trajectory since the object no longer exists
				if (currTrajectory.delayToNextPath == 0) {

					MoveToNextTrajectory();
				} else {

					if (!currTrajectory.hitPathAndWaitingForDelay) {
						//set bit that says we already hit the path and just waiting for the delay to pass
						currTrajectory.hitPathAndWaitingForDelay = true;
						Invoke ("MoveToNextTrajectory", currTrajectory.delayToNextPath);
					} 
				}
			}

			return;
		}
			
		//now update the trajectory
		if (currTrajectory.UpdateTrajectory (gameObject)) {

			Debug.Log ("Trajectory reached. Moving to next trajectory");

			//current trajectory is done. if we have another in the queue, move to that, otherwise say current trajectory is null
			//still a trajectory in the queue. set the current trajectory to it for updating
			if (currTrajectory.delayToNextPath == 0) {

				MoveToNextTrajectory();
			} else {

				if (!currTrajectory.hitPathAndWaitingForDelay) {
					//set bit that says we already hit the path and just waiting for the delay to pass
					currTrajectory.hitPathAndWaitingForDelay = true;
					Invoke ("MoveToNextTrajectory", currTrajectory.delayToNextPath);
				} 
			}
		}
	}

	private void MoveToNextTrajectory() {

		currTrajectory = (paths.Count == 0) ? null : paths.Dequeue ();
	}
		
	public void AddTrajectory(Trajectory t) {
		paths.Enqueue (t);
	}

	public void Clear() {
		paths.Clear ();
	}

}
