using UnityEngine;

public class PlayerCollision : MonoBehaviour {
    public float noseCollisionOffset = 0.2f;
    public Transform noseTransform;
    
    public delegate void CollisionFunction(Transform transform);
    public event CollisionFunction onResourceHit;
	public event CollisionFunction onFallHit;

	public Transform deadCheckOrigin; 
	public float deadCheckRadius = 1f;

    public bool isCollidingFront;

	private ResourceGatherer resourceGatherer;

	private void Start() {
		resourceGatherer = GetComponent<ResourceGatherer>();
	}

    public void EarlyUpdate() {
	    isCollidingFront = false;
		
	    // Check if we can pick up resources
	    if (!resourceGatherer.hasResource) {
		    Vector2 noseDir = transform.right * -transform.localScale.x;
		    Debug.DrawRay(noseTransform.transform.position, noseDir.normalized * noseCollisionOffset, Color.red);
		    RaycastHit2D hitInfo = Physics2D.Raycast(noseTransform.transform.position, noseDir, noseCollisionOffset);
		    Transform resource = HitInfoMeteor(hitInfo, true);
		    if (resource != null) {
			    onResourceHit?.Invoke(hitInfo.collider.transform);
		    }
	    }
	    // Check if we hit something with our carried resource
	    else {
		    CircleCollider2D myResourceCollider = resourceGatherer.holdResourceCollider;
		    float circleCastRadius = myResourceCollider.radius * myResourceCollider.transform.localScale.x;
		    RaycastHit2D[] circleCasts = Physics2D.CircleCastAll(myResourceCollider.transform.position, circleCastRadius, Vector2.zero);
		    foreach (RaycastHit2D circleCast in circleCasts) {
			    Transform collidingResource = HitInfoMeteor(circleCast, true);
			    if (collidingResource != null && collidingResource != myResourceCollider.transform) {
				    isCollidingFront = true;
			    }
		    }
	    }

	    RaycastHit2D meteorCast = Physics2D.CircleCast(deadCheckOrigin.position, deadCheckRadius, Vector2.zero);
		Transform meteor = HitInfoMeteor(meteorCast, false);
		if (meteor != null) {
			onFallHit?.Invoke(meteor);
		}
    }

	private void OnDrawGizmos() {
		Gizmos.color = Color.grey;
		Gizmos.DrawWireSphere(deadCheckOrigin.position, deadCheckRadius);

		if (resourceGatherer.holdResource) {
			Gizmos.color = Color.black;
			CircleCollider2D myResourceCollider = resourceGatherer.holdResourceCollider;
			Gizmos.DrawWireSphere(myResourceCollider.transform.position, myResourceCollider.radius * myResourceCollider.transform.localScale.x);
		}
	}

	private Transform HitInfoMeteor(RaycastHit2D hitInfo, bool resourceOnly) {
		if (hitInfo.collider != null && hitInfo.collider.CompareTag("Meteor")) {
			if (!resourceOnly) {
				return hitInfo.collider.transform;
			}

			Meteor meteor = hitInfo.collider.GetComponent<Meteor>();

			if (meteor.containsResource && resourceOnly) {
				return meteor.transform;
			}
		}

		return null;
	}
}