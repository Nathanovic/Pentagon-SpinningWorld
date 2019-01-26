using UnityEngine;

public class WorldBody : MonoBehaviour {

	private GroundCheck groundCheck;
	private float gravityGrowTime = 0f;

	public float acceleration = 0.01f;
	public float minSpeed = 3f;
	public float maxSpeed = 8f;
	public Vector3 velocity;
	public bool keepStanding;

	private void Awake() {
		groundCheck = GetComponentInChildren<GroundCheck>();
	}

	public void Update() {
		if (groundCheck.isGrounded) {
			gravityGrowTime = 0f;
			velocity = Vector3.zero;
			World.Instance.RotateAroundMe(transform);
		} else {
			gravityGrowTime += Time.deltaTime;
			Vector3 gravity = World.Instance.GetGravity(transform.position, gravityGrowTime);
			Debug.DrawRay(transform.position, gravity, Color.red);
			if (velocity == Vector3.zero) {
				velocity = gravity.normalized * minSpeed;
				Debug.DrawRay(transform.position, velocity * 3, Color.yellow, 3f);
			} else {
				velocity += gravity * acceleration * Time.deltaTime;
				velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
			}
			Debug.Log(velocity.magnitude);
			Debug.DrawRay(transform.position, velocity * 2, Color.blue);
		}

		transform.position = transform.position + velocity * Time.deltaTime;

		if (keepStanding) {
			Vector3 vectorToTarget = World.Instance.transform.position - transform.position;
			float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg + 90;
			Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
			transform.rotation = targetRotation;
		}
	}

}