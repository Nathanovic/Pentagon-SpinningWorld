using UnityEngine;
using System.Collections.Generic;

public class PlayerCollision : MonoBehaviour {
    public float noseCollisionOffset = 0.2f;
    public Transform noseTransform;

    public delegate void CollisionFunction(Meteor meteor);

    public event CollisionFunction onResourceHit;
    public event CollisionFunction onFallHit;

    public delegate void CollisionTransformFunction(Transform transform);

    public event CollisionTransformFunction onRocketChargePlateHit;

	public delegate void ColliderDelegate(Collider2D collider);
	public event ColliderDelegate onCollisionEnter;
	public event ColliderDelegate onCollisionExit;

	private List<Collider2D> currentColliders = new List<Collider2D>();

    public Transform deadCheckOrigin;
    public float deadCheckRadius = 1f;

	private bool isCollidingFront;
    public bool IsCollidingFront {
		get {
			if (Rocket.IsTouchedByTwoPlayers()) { return true; }
			return isCollidingFront;
		}
	}

    private ResourceGatherer resourceGatherer; 

    private void Start() {
        resourceGatherer = GetComponent<ResourceGatherer>();
    }

    public void EarlyUpdate() {
		isCollidingFront = false;
        Meteor collidingMeteor = null;
		
		List<Collider2D> allColliders = new List<Collider2D>();
        // Check if we can pick up resources
        if (!resourceGatherer.hasResource) {
            Vector2 noseDir = transform.right * -transform.localScale.x;
            Debug.DrawRay(noseTransform.transform.position, noseDir.normalized * noseCollisionOffset, Color.red);
            RaycastHit2D hitInfo = Physics2D.Raycast(noseTransform.transform.position, noseDir, noseCollisionOffset);
            collidingMeteor = GetMeteor(hitInfo, true);
            if (collidingMeteor != null) {
                onResourceHit?.Invoke(collidingMeteor);
			}

			if (hitInfo.collider != null && hitInfo.collider.tag == "Rocket") {
				allColliders.Add(hitInfo.collider);
				TryCollisionEnter(hitInfo.collider);
			}
		}
        // Check if we hit something with our carried resource
        else {
            CircleCollider2D myResourceCollider = resourceGatherer.holdResourceCollider;
            float circleCastRadius = myResourceCollider.radius * myResourceCollider.transform.localScale.x;
            RaycastHit2D[] circleCasts =
                Physics2D.CircleCastAll(myResourceCollider.transform.position, circleCastRadius, Vector2.zero);
            foreach (RaycastHit2D circleCast in circleCasts) {
				collidingMeteor = GetMeteor(circleCast, true);
				if (collidingMeteor != null && collidingMeteor.transform != myResourceCollider.transform) {
					isCollidingFront = true;
					continue;
				}

				if (circleCast.collider != null && circleCast.collider.tag == "Rocket") {
					allColliders.Add(circleCast.collider);
					TryCollisionEnter(circleCast.collider);
				}
            }
        }

        RaycastHit2D meteorCast = Physics2D.CircleCast(deadCheckOrigin.position, deadCheckRadius, Vector2.zero);
        Meteor meteor = GetMeteor(meteorCast, false);
        if (meteor != null) {
            if (meteor.canDamage) {
                onFallHit?.Invoke(meteor);
            }
        }

		for(int i = currentColliders.Count - 1; i > -1; i --) {
			Collider2D collider = currentColliders[i];
			if (!allColliders.Contains(collider)) {
				onCollisionExit?.Invoke(collider);
				currentColliders.Remove(collider);
			}
		}
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(deadCheckOrigin.position, deadCheckRadius);

        if (Application.IsPlaying(transform) && resourceGatherer.currentResource) {
            Gizmos.color = Color.black;
            CircleCollider2D myResourceCollider = resourceGatherer.holdResourceCollider;
            Gizmos.DrawWireSphere(myResourceCollider.transform.position,
                myResourceCollider.radius * myResourceCollider.transform.localScale.x);
        }
    }

    private Meteor GetMeteor(RaycastHit2D hitInfo, bool resourceOnly) {
        if (hitInfo.collider != null && hitInfo.collider.CompareTag("Meteor")) {
            Meteor meteor = hitInfo.collider.GetComponent<Meteor>();
            if (!resourceOnly || meteor.containsResource) {
                return meteor;
            }
        }

        return null;
    }

	private bool TryCollisionEnter(Collider2D collider) {
		if (currentColliders.Contains(collider)){
			return false;
		}

		currentColliders.Add(collider);
		onCollisionEnter?.Invoke(collider);
		return true;
	}
}