using UnityEngine;

public class Player : MonoBehaviour {
	
	public int playerNumber = 1;
	public float maxSpeed = 3f;

	public float currentSpeed;
	public float moveForce = 5f;
	public float slowDownForce = 10f;

	private bool isFacingLeft = true;

	private PlayerCollision collisionScript;

	private void Awake() {
		collisionScript = GetComponent<PlayerCollision>();
	}

	private void Update() {
		collisionScript.EarlyUpdate();
		
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
		
		if (collisionScript.collideFront) {
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