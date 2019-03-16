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

    private Vector3 rotateDirection;
    private float rotationMultiplier = 1.0f;
    public float velocityMultiplier = 1.0f;

	public bool canDamage = true;
    public string impactSoundTrigger = "Meteor_Impact";
    
    private void Start() {
        worldBody = GetComponent<WorldBody>();
        worldBody.minSpeed *= velocityMultiplier;
        worldBody.maxSpeed *= velocityMultiplier;

        worldBody.onTouchGround += OnWorldImpact;
        containsResource = GetComponent<Resource>() != null;
        
        Quaternion randomRotation = Random.rotation;
        rotateDirection = new Vector3(randomRotation.eulerAngles.x, randomRotation.eulerAngles.y, randomRotation.eulerAngles.z);
    }

    private void OnDisable() {
        worldBody.onTouchGround -= OnWorldImpact;
    }

    private void Update() {
		if (worldBody.isGrounded) { return; }
		visuals.transform.Rotate(rotateDirection * rotationMultiplier * Time.deltaTime);
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

	public void SetRotateMultiplier(float rotationMultiplier) {
		this.rotationMultiplier = rotationMultiplier;
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