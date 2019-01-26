using UnityEngine;

public class ResourceGatherer : MonoBehaviour {
    public bool hasResource = false;    
    public Transform noseTransform;
    
    // Start is called before the first frame update
    void Start() {
        PlayerCollision collision = GetComponent<PlayerCollision>();
        collision.onResourceHit += ResourceHit;
    }

    // Update is called once per frame
    void Update() {
        
    }

    void ResourceHit(Transform resourceTransform) {
        noseTransform.transform.position = resourceTransform.position + resourceTransform.transform.localScale * 0.5f;
        resourceTransform.parent = this.transform;
        Destroy(resourceTransform.GetComponent<WorldBody>());
        hasResource = true;
    }
}