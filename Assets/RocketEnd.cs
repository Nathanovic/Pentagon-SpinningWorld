using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketEnd : MonoBehaviour{

    private Animator anim;

    private void Start() {
        anim = GetComponent<Animator>();
    }

    public void ActivateAnimator() {
        anim.enabled = true;
    }
}
