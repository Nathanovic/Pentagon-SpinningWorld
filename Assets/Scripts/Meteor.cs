using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Meteor : MonoBehaviour {
    private WorldBody worldBody;
    public bool containsResource;
    public ParticleSystem vfxImpact;
    public new Light pointLight;
    public GameObject visuals;

    public Light light;
    public delegate void ImpactFunction();
    public event ImpactFunction onImpact;

    private Vector3 rotation;
    public float rotationMultiplier = 1.0f;
    public float velocityMultiplier = 1.0f;

    public bool canDamage;// { private set; get; }
    public String impactSoundTrigger = "Meteor_Impact";
    
    private void Start() {
        canDamage = true;
        worldBody = GetComponent<WorldBody>();
        worldBody.minSpeed *= velocityMultiplier;
        worldBody.maxSpeed *= velocityMultiplier;

        worldBody.onTouchGround += OnWorldImpact;
        onImpact += ShowImpactParticles;
        onImpact += DestroyLight;
        containsResource = GetComponent<Resource>() != null;
        
        Quaternion randomRotation = Random.rotation;
        rotation = new Vector3(randomRotation.x, randomRotation.y, randomRotation.z);
    }

    private void OnDisable() {
        worldBody.onTouchGround -= OnWorldImpact;
    }

    private void Update() {
        if(!worldBody.isGrounded)
            visuals.transform.Rotate(rotation * rotationMultiplier);
    }

    private void OnWorldImpact() {
        canDamage = false;
        //TODO: play meteor impact sound
        //TODO: show impact particles
        onImpact?.Invoke();
        
        //AkSoundEngine.Postevent("Meteor_Impact", gameobject);
        print("Sound effect: " + impactSoundTrigger);

        if (!containsResource) {
            Destroy(this.gameObject);
            //TODO: show destroy meteor particles
        }
    }

    private void ShowImpactParticles() {
        if (vfxImpact != null) {
            vfxImpact.transform.parent = null;
            vfxImpact.Play();
            vfxImpact.GetComponent<ParticleDestroyer>().DestroyWhenDone();
        }
    }

    private void DestroyLight() {
        if(light != null) {
            Destroy(light);
        }
    }
}