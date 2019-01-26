using UnityEngine;

public class Player : MonoBehaviour {
	
	public int playerNumber = 1;
	public float maxSpeed = 3f;

	public float currentSpeed;
	public float moveForce = 5f;
	public float slowDownForce = 10f;

	private bool isFacingLeft = true;

	public float noseCollisionOffset = 0.2f;
	public Transform noseTransform;

	private void Update() {
		float input = -Input.GetAxis("Horizontal_" + playerNumber.ToString());
		if (input != 0f) {
			SetFacingDirection(input);
		}

		// Slow car down
		float absSpeed = Mathf.Abs(currentSpeed);
		absSpeed -= slowDownForce * Time.deltaTime;
		if (absSpeed < 0) {
			absSpeed = 0f;
		}
		if (currentSpeed < 0) {
			currentSpeed = -absSpeed;
		} else {
			currentSpeed = absSpeed;
		}

		Vector3 noseDir = transform.right * -transform.localScale.x;
		Debug.DrawRay(noseTransform.transform.position, noseDir.normalized * noseCollisionOffset, Color.red);
		bool collideFront = Physics.Raycast(noseTransform.transform.position, noseDir, noseCollisionOffset);
		if (collideFront) {
			currentSpeed = 0f;
		} else {
			if (input != 0f) {
				currentSpeed += moveForce * input * Time.deltaTime;
				currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
			}
		}
		
		transform.RotateAround(World.Position, Vector3.forward, currentSpeed * Time.deltaTime);
	}


	private void SetFacingDirection(float input) {
		if (input < 0f && isFacingLeft) {
			isFacingLeft = false;
			transform.localScale = new Vector3(-1, 1, 1);
		} else if (input > 0f && !isFacingLeft) {
			isFacingLeft = true;
			transform.localScale = new Vector3(1, 1, 1);
		}
	}

}