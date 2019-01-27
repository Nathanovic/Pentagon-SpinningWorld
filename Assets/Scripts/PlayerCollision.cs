using UnityEngine;
using System.Collections.Generic;

public class PlayerCollision : MonoBehaviour {
	private Player player;
    public float noseCollisionOffset = 0.2f;

    public delegate void CollisionFunction(Meteor meteor);

    public event CollisionFunction onFrontResourceHit;
    public event CollisionFunction onFallHit;

    public delegate void CollisionTransformFunction(Transform transform);

    public event CollisionTransformFunction onRocketChargePlateHit;

	public delegate void ColliderDelegate(Collider2D collider);
	public event ColliderDelegate onCollisionEnter;
	public event ColliderDelegate onCollisionExit;

	private List<Collider2D> currentColliders = new List<Collider2D>();
	
	private CircleCollider2D myCollider;
	public float deathCastYOffset = 0.5f;

    private ResourceGatherer resourceGatherer;

	public LayerMask defaultLM;
	public LayerMask deathLM;

	private bool isPlayingRocketLaunchSound = false;

	private void Start() {
        resourceGatherer = GetComponent<ResourceGatherer>();
		myCollider = GetComponent<CircleCollider2D>();
		player = GetComponent<Player>();
	}

    public void EarlyUpdate() {
		Vector2 deadCheckPosition = transform.position + transform.up * deathCastYOffset;
		RaycastHit2D meteorCast = Physics2D.CircleCast(deadCheckPosition, myCollider.radius, Vector2.zero, 100f, deathLM);
        Meteor meteor = GetMeteor(meteorCast, false);
        if (meteor != null) {
            if (meteor.canDamage) {
                onFallHit?.Invoke(meteor);
            }
        }
    }

	public bool IsCollidingFront(Transform frontChecker) {
		bool isCheckingFront = frontChecker.name.Contains("front");
		bool isCollidingFront = false;
		Meteor collidingMeteor = null;

		List<Collider2D> allColliders = new List<Collider2D>();
		// Check if we can pick up resources
		if (!resourceGatherer.hasResource || !isCheckingFront) {
			Vector2 noseDir = frontChecker.up;
			Debug.DrawRay(frontChecker.position, noseDir.normalized * noseCollisionOffset, Color.red);
			RaycastHit2D hitInfo = Physics2D.Raycast(frontChecker.position, noseDir, noseCollisionOffset, defaultLM);
			collidingMeteor = GetMeteor(hitInfo, true);
			if (collidingMeteor != null && isCheckingFront) {
				onFrontResourceHit?.Invoke(collidingMeteor);
			}

			bool hasRocket = false;
			if (hitInfo.collider != null && hitInfo.collider.tag == "Rocket") {
				hasRocket = true;
				allColliders.Add(hitInfo.collider);
				TryCollisionEnter(hitInfo.collider);
				
				playRocketPushSound();
			}

			if (hasRocket) {
				Collider2D touchedCollider = Rocket.instance.CollideOther(transform.position, noseCollisionOffset);
				if (touchedCollider != null) {
					isCollidingFront = true;
				}
			}
		}
		// Check if we hit something with our carried resource
		else {
			CircleCollider2D myResourceCollider = resourceGatherer.holdResourceCollider;
			float circleCastRadius = myResourceCollider.radius * myResourceCollider.transform.localScale.x;
			RaycastHit2D[] circleCasts =
				Physics2D.RaycastAll(myResourceCollider.transform.position, transform.right, circleCastRadius, defaultLM);
			Debug.DrawRay(myResourceCollider.transform.position, transform.right * circleCastRadius, Color.black);
			foreach (RaycastHit2D circleCast in circleCasts) {
				collidingMeteor = GetMeteor(circleCast, true);
				if (collidingMeteor != null && collidingMeteor.transform != myResourceCollider.transform) {
					isCollidingFront = true;
				} else if (circleCast.collider != null && circleCast.collider.tag == "Rocket") {
					allColliders.Add(circleCast.collider);
					TryCollisionEnter(circleCast.collider);
					resourceGatherer.DeliverResource();
					
					playRocketPushSound();
				}
			}
		}

		for (int i = currentColliders.Count - 1; i > -1; i--) {
			Collider2D collider = currentColliders[i];
			if (!allColliders.Contains(collider)) {
				onCollisionExit?.Invoke(collider);
				currentColliders.Remove(collider);
			}
		}

		return isCollidingFront;
	}

	private void playRocketPushSound() {
		if (!isPlayingRocketLaunchSound) {
			AkSoundEngine.PostEvent("Push_Rocket", gameObject);
			isPlayingRocketLaunchSound = true;
		}
	}

    private void OnDrawGizmos() {
		if (!Application.isPlaying) { return; }

		Gizmos.color = Color.grey;
		Vector2 deadCheckPosition = transform.position + transform.up * deathCastYOffset;
		Gizmos.DrawWireSphere(deadCheckPosition, myCollider.radius);

		if (resourceGatherer.currentResource) {
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