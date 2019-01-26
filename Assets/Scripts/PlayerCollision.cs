using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {
    public float noseCollisionOffset = 0.2f;
    public Transform noseTransform;
    
    public delegate void CollisionFunction(Transform transform);
    public event CollisionFunction onResourceHit;

    public bool collideFront;

    public void EarlyUpdate() {
        Vector3 noseDir = transform.right * -transform.localScale.x;
        Debug.DrawRay(noseTransform.transform.position, noseDir.normalized * noseCollisionOffset, Color.red);
        RaycastHit hitInfo;
        Physics.Raycast(noseTransform.transform.position, noseDir, out hitInfo,noseCollisionOffset);
        collideFront = false;
        if (hitInfo.collider != null && hitInfo.collider.CompareTag("Meteor")) {
            Meteor meteor = hitInfo.collider.GetComponent<Meteor>();
            if (meteor.containsResource) {
                collideFront = true;
                onResourceHit?.Invoke(hitInfo.collider.transform);
            }
        }
    }
}