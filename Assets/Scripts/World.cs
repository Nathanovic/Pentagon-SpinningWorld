using UnityEngine;

public class World : MonoBehaviour {

	public static World Instance;

	public float rotateSpeed;
	public float gravityForce = 9.81f;
	public float gravityGrowDuration = 1f;// How long it takes to reach max gravity
	public AnimationCurve gravityGrowCurve;

	private void Awake() {
		Instance = this;
	}

	private void Update() {
		transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }

	public Vector3 GetGravity(Vector3 position, float growDuration) {
		float gravityGrowFactor = gravityGrowCurve.Evaluate(growDuration);
		if (gravityGrowFactor > 1f) {
			gravityGrowFactor = 1f;
		}

		Vector3 gravity = (position - transform.position).normalized * gravityForce * Time.deltaTime * gravityGrowFactor;
		return gravity;
	}

}