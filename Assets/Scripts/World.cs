using UnityEngine;

public class World : MonoBehaviour {

	public float rotateSpeed;

    void Start() {
        
    }

    void Update() {
		transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }

}