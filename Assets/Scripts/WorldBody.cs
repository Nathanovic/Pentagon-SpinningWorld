using UnityEngine;

public class WorldBody : MonoBehaviour {

	private GroundCheck groundCheck;
	private float gravityGrowTime = 0f;

	private void Awake() {
		groundCheck = GetComponentInChildren<GroundCheck>();
	}

	public void Update() {
		if (groundCheck.isGrounded) {
			gravityGrowTime = 0f;
		} else {
			gravityGrowTime += Time.deltaTime;
		}
	
		Vector3 gravity = World.Instance.GetGravity(transform.position, gravityGrowTime);
		transform.position += gravity;
	}

}