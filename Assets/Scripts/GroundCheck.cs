using UnityEngine;

public class GroundCheck : MonoBehaviour {

	public bool isGrounded;
	
	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log("Enter " + other.name);
		isGrounded = true;
	}

	void OnTriggerExit2D(Collider2D other) {
		isGrounded = false;
	}

}