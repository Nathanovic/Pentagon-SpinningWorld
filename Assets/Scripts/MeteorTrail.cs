using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorTrail : MonoBehaviour{

    public TrailRenderer trailRenderer;

    public void Update() {
        trailRenderer.time -= Time.deltaTime * .3f;
    }

    public void OnWorldImpact() {
        trailRenderer.enabled = false;
    }

}
