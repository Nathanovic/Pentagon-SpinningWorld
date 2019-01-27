using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthClouds : MonoBehaviour{

    public Transform orbitTransform;
    public float rotateSpeed;

    private void Update() {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        transform.RotateAround(orbitTransform.position, orbitTransform.forward, 1 * Time.deltaTime);
    }

}
