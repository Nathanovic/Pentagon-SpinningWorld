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
        player.collisionScript.onResourceHit += ResourceHit;
        player.collisionScript.onRocketChargePlateHit += OnRocketChargePlateHit;
        player.collisionScript.onFallHit += OnFallHit;
    }

    private void Update() {
        if (hasResource) {
            float dropResourceInput = Input.GetAxis("DropResource_" + player.playerNumber);
            if (dropResourceInput > 0f || dropResourceInput < 0f) {
                DropResource();
            }
        }
    }

    void ResourceHit(Meteor meteor) {
        if (hasResource) { return; }

        Resource resource = meteor.GetComponent<Resource>();
        if (resource.isHeld) { return; }
        
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
}