using UnityEngine;

public class Player : MonoBehaviour {
	
	public int playerNumber = 1;
	public float maxSpeed = 3f;

	public float currentSpeed;
	public float moveForce = 5f;
	public float slowDownForce = 10f;

	private void Update() {
		float input = Input.GetAxis("Horizontal_" + playerNumber.ToString());

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

		// Accelerate
		if (input != 0f) {
			currentSpeed += moveForce * -input * Time.deltaTime;
			currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
		} 
		
		transform.RotateAround(World.Position, Vector3.forward, currentSpeed * Time.deltaTime);
	}

}