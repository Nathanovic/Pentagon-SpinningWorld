using UnityEngine;

public class ResourceGatherer : MonoBehaviour {
    public bool hasResource = false;    
    public Transform holdResource { private set; get; }
    public CircleCollider2D holdResourceCollider { private set; get; }
    
    // Start is called before the first frame update
    void Start() {
        PlayerCollision collision = GetComponent<PlayerCollision>();
        collision.onResourceHit += ResourceHit;
    }

    void ResourceHit(Transform resourceTransform) {
        if (hasResource) { return; }

        Resource resource = resourceTransform.GetComponent<Resource>();
        if (resource.isHeld) { return; }
        
        resource.PickUp();
        resourceTransform.parent = this.transform;
        Destroy(resourceTransform.GetComponent<WorldBody>());
        holdResource = resourceTransform;
        holdResourceCollider = holdResource.GetComponent<CircleCollider2D>();
        hasResource = true;
    }
}