using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour {
    private WorldBody worldBody;
    [SerializeField]
    private bool isGrounded = false;
    public bool containsResource = false;
    public ParticleSystem vfxImpact;
    public GameObject visuals;
    
    public delegate void ImpactFunction();
    public event ImpactFunction onImpact;

    private Vector3 rotation;

    private void Start() {
        worldBody = GetComponent<WorldBody>();
        isGrounded = World.Instance.GetGrounded(worldBody);
        onImpact += ShowImpactParticles;
        
        Quaternion randomRotation = Random.rotation;
        rotation = new Vector3(randomRotation.x, randomRotation.y, randomRotation.z);
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
        
        if(!isGrounded)
            visuals.transform.Rotate(rotation);
    }


    private void ShowImpactParticles() {
        if (vfxImpact != null) {
            vfxImpact.transform.parent = null;
            vfxImpact.Play();
            vfxImpact.GetComponent<ParticleDestroyer>().DestroyWhenDone();
        }
    }

  
}