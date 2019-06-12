using UnityEngine;
using System.Collections;
using Halus.Events;

//TODO we might want to change follow to an abstract class and make abstract methods to calculate the follow paths at a later time
public class ArcFollow2D : MonoBehaviour {

	public GameObject targetObject;
	public float targetRadius;	//trigger OnReachedTarget when we get within targetRadius of the target object

	public bool destroyOnReach; //do we destroy the object once we reach the target
	public bool destroyOnOrphan = true;

	public float speed = 3.0f;
	float arcFactor = 4.0f; //arc factor calculates how much of a curving effect we get for the path. set this to 0 for linear path

	private Vector3 startPos;
	// Use this for initialization
	void Start () {
		targetObject = GameObject.FindObjectOfType<StoneBehavior>().gameObject;
		startPos = gameObject.transform.position;

	}
	
	// Update is called once per frame
	void Update () {

		//target object gone, so we are orphaned
		if (targetObject == null && destroyOnOrphan) {

			Destroy (gameObject);
		}

		Vector3 endPos = targetObject.transform.position;
		float xDistOrig = startPos.x - endPos.x;

		float nextXPos = Mathf.MoveTowards (transform.position.x, endPos.x, speed * Time.deltaTime);

		//linear Y pos is a linear interpolation on the y range, of the ratio of how far we've gone out of the entire distance 
		float lerpYPos = Mathf.Lerp(startPos.y, endPos.y, Mathf.Abs(nextXPos - startPos.x) / xDistOrig);

		//addY will add an additional Y value to the otherwise linear path
		float addY = arcFactor * ((nextXPos - startPos.x) * (nextXPos - endPos.x) / (-0.25f * xDistOrig * xDistOrig));

		float nextYPos = lerpYPos + addY;
		Vector3 nextPos = new Vector3 (nextXPos, nextYPos , gameObject.transform.position.z);


		gameObject.transform.rotation = LookAt2D (nextPos - gameObject.transform.position);
		gameObject.transform.position = nextPos;

		//DebugConsole.Log ("lerp " + ((nextXPos - startPos.x) / xDistOrig), "normal");


		if (Vector2.Distance (new Vector2 (nextXPos, nextYPos), new Vector2 (endPos.x, endPos.y)) <= targetRadius) {
			ReachedTarget ();
		} 


		/*
		Vector3 diff = endPos - startPos;
		Vector3 newBasePos = gameObject.transform.position + (diff * speed * Time.deltaTime);

		float relDist = Vector3.Distance (endPos - newBasePos) / Vector3.Distance (endPos - startPos);

		float height = (4 * arcFactor * relDist) * ((-1 * relDist) + 1);




		gameObject.transform.position = newBasePos; */

	}

	private void ReachedTarget() {
		//DebugConsole.Log ("Reached Target!", "normal");

		//EventManager.TriggerEvent(HotStoneEventType.

		if (destroyOnReach) {

			Destroy (gameObject);
		}
	}

	static Quaternion LookAt2D(Vector2 forward) {

		return Quaternion.Euler (0, 0, Mathf.Atan2 (forward.y, forward.x) * Mathf.Rad2Deg);
	}
}
