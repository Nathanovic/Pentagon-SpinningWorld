using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

	public static Rocket instance;

	public Transform earth;

	public float launchPower;
	public float liftOffAccelerateFactor = 3f;
	public float standardAccelerateFactor = 1f;
	public float finishLiftOffSpeed = 50f;

	private float currentSpeed;
	private bool isLaunched;

	public float collisionYOffset = 1f;
	public float collisionXOffset = 0.3f;
	public LayerMask collisionLM;

	private void Awake() {
		instance = this;
	}

	private void Update() {
		if (Input.GetKeyUp(KeyCode.Space)) {
			Launch();
		}

		if (isLaunched) {
			float acceleration = launchPower;
			if (currentSpeed < finishLiftOffSpeed) {
				acceleration += currentSpeed * (liftOffAccelerateFactor - 1f);
			} else {
				acceleration += finishLiftOffSpeed * (standardAccelerateFactor - 1f);
			}

			currentSpeed += acceleration * Time.deltaTime;
			transform.position = transform.position + transform.up * currentSpeed * Time.deltaTime;
		}
    }

	private void Launch() {
		//AkSoundEngine.Postevent("Rocket_Launch", gameobject);
		print("Sound effect: Rocket_Launch");
		
		isLaunched = true;
		transform.SetParent(null);
	}

	public Collider2D CollideOther(Vector3 checkingPlayerPos, float checkDistance) {
		Vector3 inverseTransformPos = transform.InverseTransformPoint(checkingPlayerPos);
		float xMultiplier = (inverseTransformPos.x < 0f) ? 1f : -1f;
		Vector2 startPos = transform.position + transform.up * collisionYOffset;
		Vector2 collisionCheck = (startPos + (Vector2)transform.right * xMultiplier) - startPos;
		startPos += collisionCheck.normalized * collisionXOffset;
		Debug.DrawRay(startPos, collisionCheck.normalized * checkDistance, Color.yellow);
		RaycastHit2D hitInfo = Physics2D.Raycast(startPos, collisionCheck, checkDistance, collisionLM);
		return hitInfo.collider;
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.grey;
		Gizmos.DrawWireCube(transform.position + transform.up * collisionYOffset, new Vector3(0.2f, 0.1f));
	}

}
