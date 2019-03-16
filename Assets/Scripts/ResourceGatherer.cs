using UnityEngine;

public class ResourceGatherer : MonoBehaviour {
    public Resource currentResource { private set; get; }
    public bool hasResource {
        get { return currentResource != null; }
    }
    public CircleCollider2D holdResourceCollider { private set; get; }
    private Player player;
    
    // Start is called before the first frame update
    void Start() {
        player = GetComponent<Player>();
        player.collisionScript.onFrontResourceHit += ResourceHit;
        player.collisionScript.onRocketCollisionEnter += OnRocketHit;
        player.collisionScript.onFallHit += OnFallHit;
    }

    private void Update() {
        if (hasResource) {
            bool dropResource = Input.GetButton("DropResource_" + player.playerNumber);
            if (dropResource) {
                DropResource();
            }
        }
    }

    void ResourceHit(Meteor meteor) {
        if (hasResource) { return; }
		
        Resource resource = meteor.GetComponent<Resource>();
        if (resource == null || !resource.canPickUp) { return; }
        
        currentResource = resource;
        currentResource.PickUp(transform);
		currentResource.onDeliver += OnResourceDelivered;
		holdResourceCollider = currentResource.GetComponent<CircleCollider2D>();
    }

    private void DropResource() {
		currentResource.onDeliver -= OnResourceDelivered;
		currentResource.Drop();
        currentResource = null;
    }

    private void OnRocketHit(bool isFrontCollision) {
		if (!hasResource) { return; }
		if (!isFrontCollision) { return; }

		Destroy(currentResource.gameObject);
        currentResource = null;
    }
    
    private void OnFallHit(Meteor meteor) {
        if (hasResource) {
            DropResource();
        }
    }

	private void OnResourceDelivered() {
		currentResource.onDeliver -= OnResourceDelivered;
		currentResource = null;
	}

}