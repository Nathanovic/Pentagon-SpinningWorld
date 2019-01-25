using UnityEngine;

public class Player : MonoBehaviour {

	public Transform world;
	public int playerNumber = 1;
	public float moveSpeed = 3f;

	private void Update() {
		float input = Input.GetAxis("Horizontal_" + playerNumber.ToString());
		float movement = input * Time.deltaTime * moveSpeed;
		transform.RotateAround(world.position, Vector3.forward, movement);
	}

}