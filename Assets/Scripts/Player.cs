using UnityEngine;

public class Player : MonoBehaviour {
	
	public int playerNumber = 1;
	public float maxSpeed = 3f;

	public float reachMaxSpeedDuration = 0.8f;
	public float moveInputDuration;
	public AnimationCurve acceleration;
	public AnimationCurve deceleration;

	private void Update() {
		float input = Input.GetAxis("Horizontal_" + playerNumber.ToString());
		float moveSpeed = 0f;
		if (input != 0f) {
			moveInputDuration += Time.deltaTime;
			moveInputDuration = Mathf.Min(moveInputDuration, reachMaxSpeedDuration);
			float curveFactor = 1f / reachMaxSpeedDuration * moveInputDuration;
			float curveValue = acceleration.Evaluate(curveFactor);
			moveSpeed = maxSpeed * curveValue;
		} else if(input == 0f) {
			moveInputDuration = 0f;
		}

		moveSpeed *= input * Time.deltaTime;
		transform.RotateAround(World.Position, Vector3.forward, moveSpeed);
	}


}