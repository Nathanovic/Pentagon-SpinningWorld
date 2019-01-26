using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour {
    private WorldBody worldBody;
    [SerializeField]
    private bool isGrounded = false;
    public bool containsResource = false;
    public ParticleSystem vfxImpact;
    
    public delegate void ImpactFunction();
    public event ImpactFunction onImpact;

    private void Start() {
        worldBody = GetComponent<WorldBody>();
        isGrounded = World.Instance.GetGrounded(worldBody);
        onImpact += ShowImpactParticles;
    }

    private void Update() {
        if (!isGrounded && World.Instance.GetGrounded(worldBody)) {
            isGrounded = true;
            //TODO: play meteor impact sound
            //TODO: show impact particles
            onImpact?.Invoke();

            if (!containsResource) {
                Destroy(this.gameObject);
                //TODO: show destroy meteor particles
            }
        }
    }


    private void ShowImpactParticles() {
        if (vfxImpact != null) {
            vfxImpact.transform.parent = null;
            vfxImpact.Play();
            vfxImpact.GetComponent<ParticleDestroyer>().DestroyWhenDone();
        }
    }

  
}