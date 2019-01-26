using UnityEngine;

public class World : MonoBehaviour {

	public static World Instance;

	public float rotateSpeed;
	public float gravityForce = 9.81f;
	public float gravityGrowDuration = 1f;// How long it takes to reach max gravity
	public AnimationCurve gravityGrowCurve;

	public float radius = 15;

	private void Awake() {
		Instance = this;
	}

	private void Update() {
		transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }

	public Vector3 GetGravity(WorldBody worldBody) {
		float gravityGrowFactor = gravityGrowCurve.Evaluate(worldBody.GravityGrowTime);
		if (gravityGrowFactor > 1f) {
			gravityGrowFactor = 1f;
		}

		Vector3 gravityDirection = transform.position - worldBody.Position;
		float gravityMagnitude = gravityDirection.magnitude;

		Vector3 gravity = (gravityDirection / gravityMagnitude) * gravityForce * Time.deltaTime * gravityGrowFactor;
		return gravity;
	}

	public bool GetGrounded(WorldBody worldBody) {
		float distance = Vector3.Distance(transform.position, worldBody.Position);
		return distance < (radius + worldBody.bodyWorldOffset);
	}

	public void RotateAroundMe(Transform target) {
		target.RotateAround(transform.position, Vector3.forward, rotateSpeed * Time.deltaTime);
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, radius);
	}

}