using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour {
    private WorldBody worldBody;
    private bool isGrounded = false;

    private void Start() {
        worldBody = GetComponent<WorldBody>();
        isGrounded = World.Instance.GetGrounded(worldBody);
    }

    private void Update() {
        if (!isGrounded && World.Instance.GetGrounded(worldBody)) {
            //TODO: play meteor impact sound
            isGrounded = true;
        }
    }
}