using UnityEngine;
using Random = UnityEngine.Random;

public class Meteor : MonoBehaviour {
    private WorldBody worldBody;
    public bool containsResource;
    public ParticleSystem vfxImpact;
    public Light pointLight;
    public GameObject visuals;

    public new Light light;
    public delegate void ImpactFunction();
    public event ImpactFunction onImpact;

    private Vector3 rotation;
    public float rotationMultiplier = 1.0f;
    public float velocityMultiplier = 1.0f;

    public bool canDamage { private set; get; }
    public string impactSoundTrigger = "Meteor_Impact";
    
    private void Start() {
        canDamage = true;
        worldBody = GetComponent<WorldBody>();
        worldBody.minSpeed *= velocityMultiplier;
        worldBody.maxSpeed *= velocityMultiplier;

        worldBody.onTouchGround += OnWorldImpact;
        containsResource = GetComponent<Resource>() != null;
        
        Quaternion randomRotation = Random.rotation;
        rotation = new Vector3(randomRotation.x, randomRotation.y, randomRotation.z);
    }

    private void OnDisable() {
        worldBody.onTouchGround -= OnWorldImpact;
    }

    private void Update() {
		if (worldBody.isGrounded) { return; }
		visuals.transform.Rotate(rotation * rotationMultiplier);
    }

    private void OnWorldImpact() {
        canDamage = false;
        
        AkSoundEngine.PostEvent(impactSoundTrigger, gameObject);

        if (!containsResource) {
            Destroy(gameObject);
        }

		if (light != null) {
			Destroy(light);
		}

		ShowImpactParticles();

		onImpact?.Invoke();
	}

	public void CollideRocket() {
		ShowImpactParticles();
		Destroy(gameObject);
	}

    private void ShowImpactParticles() {
        if (vfxImpact != null) {
            vfxImpact.transform.parent = null;
            vfxImpact.Play();
            vfxImpact.GetComponent<ParticleDestroyer>().DestroyWhenDone();
        }
    }

}