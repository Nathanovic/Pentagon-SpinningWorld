using UnityEngine;

public class Rocket : MonoBehaviour {

	public Transform earth;

	public float launchPower;
	public float liftOffAccelerateFactor = 3f;
	public float standardAccelerateFactor = 1f;
	public float finishLiftOffSpeed = 50f;
	public float maxSpeed = 60f;

	private float currentSpeed;
	private bool isLaunched;

	public float rotateToEarthSpeed = 20f;

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
			currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
			transform.position = transform.position + transform.up * currentSpeed * Time.deltaTime;

			float rotateFactor = maxSpeed / currentSpeed;
			transform.Rotate(transform.right, rotateToEarthSpeed * Time.deltaTime);
		}
    }

	private void Launch() {
		isLaunched = true;
		transform.SetParent(null);
	}

}
