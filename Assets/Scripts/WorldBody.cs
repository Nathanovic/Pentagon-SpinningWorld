using System;
using UnityEngine;

public class WorldBody : MonoBehaviour {
	
	public float GravityGrowTime { get; private set; }

	public Vector3 Position {
		get { return transform.position; }
	}

	public float acceleration = 0.01f;
	public float minSpeed = 3f;
	public float maxSpeed = 8f;
	public Vector3 velocity;
	public bool keepStanding;
	public float bodyWorldOffset;
	public bool isGrounded;// { private set; get; }

	public Action onTouchGround;
	
	public void LateUpdate() {
		bool newGrounded = World.Instance.GetGrounded(this);
		if (newGrounded != isGrounded && newGrounded) {
			onTouchGround?.Invoke();
		}
		isGrounded = newGrounded;

		if (isGrounded) {
			GravityGrowTime = 0f;
			velocity = Vector3.zero;
			World.Instance.RotateAroundMe(transform);
		} else {
			GravityGrowTime += Time.deltaTime;
			Vector3 gravity = World.Instance.GetGravity(this);

			if (velocity == Vector3.zero) {
				velocity = gravity.normalized * minSpeed;
			} else {
				velocity += gravity * acceleration * Time.deltaTime;
				velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
			}
		}

		if (keepStanding) {
			Vector3 vectorToTarget = World.Instance.transform.position - transform.position;
			float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg + 90;
			Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
			transform.rotation = targetRotation;
		}

		transform.position = transform.position + velocity * Time.deltaTime;
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, bodyWorldOffset);
	}

}