using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour {
    private ParticleSystem vfxImpact;

    private void Start() {
        vfxImpact = GetComponent<ParticleSystem>();
    }

    public void DestroyWhenDone() {
        StartCoroutine(DestroyParticlesGameObject());
    }
    
    private IEnumerator DestroyParticlesGameObject() {
        yield return new WaitUntil(() => vfxImpact.isStopped);
        Destroy(this.gameObject);
    }
}