using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    public Transform orbitTransform;
    public float orbitSpeed;
    public float rotateSpeed;

    private void Update() {
        transform.Rotate(new Vector3(1, 1, 1), rotateSpeed * Time.deltaTime);
        transform.RotateAround(orbitTransform.position, Vector3.one, orbitSpeed * Time.deltaTime);
    }
}
