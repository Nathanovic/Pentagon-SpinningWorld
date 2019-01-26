using UnityEngine;

public class ResourceGatherer : MonoBehaviour {
    public bool hasResource = false;    
    public Transform holdResource { private set; get; }
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

    void ResourceHit(Transform resourceTransform) {
        if (hasResource) { return; }

        Resource resource = resourceTransform.GetComponent<Resource>();
        if (resource.isHeld) { return; }
        
        resource.PickUp();
        resourceTransform.parent = this.transform;
        resourceTransform.GetComponent<WorldBody>().enabled = false;
        holdResource = resourceTransform;
        holdResourceCollider = holdResource.GetComponent<CircleCollider2D>();
        hasResource = true;
    }

    private void DropResource() {
        holdResource.SetParent(null);
        holdResource.GetComponent<WorldBody>().enabled = true;
        holdResource.GetComponent<Resource>().Drop();
        holdResource = null;
        hasResource = false;
    }

    void OnRocketChargePlateHit(Transform transform) {
        if(!hasResource) return;
        
        Destroy(holdResource.gameObject);
        holdResource = null;
    }
    
    private void OnFallHit(Transform meteor) {
        if (hasResource) {
            DropResource();
        }
    }
}