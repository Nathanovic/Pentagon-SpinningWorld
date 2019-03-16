using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorTrail : MonoBehaviour{

    public TrailRenderer[] trailRenderer;

    [SerializeField]
    public float lengthDifference;
    [SerializeField]
    public float lengthDifferenceTime;

    private void Start() {
        StartCoroutine(IntervalWait());
    }

    private IEnumerator IntervalWait() {
        float lengthOffset = 0f;
        int randomInt = 1;

        while (true) {
            randomInt *= -1;
            
            if (randomInt == 1) {
                lengthOffset = lengthDifference;
            }
            else {
                lengthOffset = -lengthDifference;
            }
            for(int i = 0; i < trailRenderer.Length; i++) {
                trailRenderer[i].time += lengthOffset;
            }

            yield return new WaitForSeconds(lengthDifferenceTime);
        }
    }



    public void OnWorldImpact() {
        for (int i = 0; i < trailRenderer.Length; i++) {
            trailRenderer[i].enabled = false;
        }
    }

}
