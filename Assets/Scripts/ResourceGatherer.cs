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
        player.collisionScript.onRocketChargePlateHit += OnRocketChargePlateHit;
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
        holdResourceCollider = currentResource.GetComponent<CircleCollider2D>();
    }

    private void DropResource() {
        currentResource.Drop();
        currentResource = null;
    }

    void OnRocketChargePlateHit(Transform transform) {
        if(!hasResource) return;
        
        Destroy(currentResource.gameObject);
        currentResource = null;
    }
    
    private void OnFallHit(Meteor meteor) {
        if (hasResource) {
            DropResource();
        }
    }

	public void DeliverResource() {
		Rocket.instance.DeliverResource(currentResource);
		currentResource.Deliver();
		currentResource = null;
	}

}