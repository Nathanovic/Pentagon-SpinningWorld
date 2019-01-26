using UnityEngine;

public class Earth : MonoBehaviour {

	public Transform orbitTransform;
	public float orbitSpeed; 
	public float rotateSpeed;
	
    private void Update() {
		transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
		transform.RotateAround(orbitTransform.position, orbitTransform.forward, orbitSpeed * Time.deltaTime);
	}

}