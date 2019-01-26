using System;
using UnityEngine;

public class ResourceGatherer : MonoBehaviour {
    public bool hasResource = false;
    private BoxCollider collider;
    
    
    // Start is called before the first frame update
    void Start() {
        collider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update() {
        Vector3 noseDir = transform.right * -transform.localScale.x;
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("collision!");
        other.transform.parent = this.transform;
    }
}