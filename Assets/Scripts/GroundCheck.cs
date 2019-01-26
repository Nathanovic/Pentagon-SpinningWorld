using UnityEngine;

public class GroundCheck : MonoBehaviour {

	public bool isGrounded;
	
	void OnTriggerEnter2D(Collider2D other) {
		isGrounded = true;
	}

	void OnTriggerExit2D(Collider2D other) {
		isGrounded = false;
	}

}