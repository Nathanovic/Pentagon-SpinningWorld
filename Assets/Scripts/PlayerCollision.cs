using UnityEngine;
using System.Collections.Generic;

public class PlayerCollision : MonoBehaviour {

	private const string TAG_METEOR = "Meteor";
	private const string TAG_ROCKET = "Rocket";

	private Player player;
	public float noseCollisionRadius = 0.2f;

    public delegate void CollisionFunction(Meteor meteor);

    public event CollisionFunction onFrontResourceHit;
    public event CollisionFunction onFallHit;

	public delegate void RocketCollisionDelegate();
	public event RocketCollisionDelegate onRocketCollisionEnter;
	public event RocketCollisionDelegate onRocketCollisionExit;

	private bool didTouchRocket;

	private CircleCollider2D myCollider;
	public float deathCastYOffset = 0.5f;

    private ResourceGatherer resourceGatherer;
	
	public LayerMask resourceLM;
	public LayerMask rocketLM;
	public LayerMask defaultLM;
	public LayerMask deathLM;

	private bool isPlayingRocketLaunchSound = false;

	private bool isCarryingResource { get { return resourceGatherer.hasResource; } }

	private void Start() {
        resourceGatherer = GetComponent<ResourceGatherer>();
		myCollider = GetComponent<CircleCollider2D>();
		player = GetComponent<Player>();
	}

    public void EarlyUpdate() {
		// Check for fall damage
		Vector2 deadCheckPosition = transform.position + transform.up * deathCastYOffset;
		Collider2D meteorCollider = Physics2D.OverlapCircle(deadCheckPosition, myCollider.radius, deathLM);
        if (meteorCollider != null && meteorCollider.CompareTag(TAG_METEOR)) {
			Meteor meteor = meteorCollider.GetComponent<Meteor>();
			if (meteor.canDamage) {
                onFallHit?.Invoke(meteor);
            }
        }
    }

	public bool IsCollidingFront(Transform frontChecker) {
		bool isCheckingFront = frontChecker.name.Contains("front");
		bool isCollidingFront = false;
		bool isTouchingRocket = false;

		Debug.DrawRay(frontChecker.position, frontChecker.up * noseCollisionRadius, Color.red);

		// Check if we can pick up resources
		if (isCheckingFront && !isCarryingResource) {
			Collider2D resourceCollider = Physics2D.OverlapCircle(frontChecker.position, noseCollisionRadius, resourceLM);
			Meteor resource = GetResource(resourceCollider);
			if (resource != null && isCheckingFront) {
				onFrontResourceHit?.Invoke(resource);
			}
		}
		
		// Check if we collide with the rocket
		if (!isCarryingResource) {
			Collider2D rocketCollider = Physics2D.OverlapCircle(frontChecker.position, noseCollisionRadius, rocketLM);
			isTouchingRocket = TryRocketCollision(rocketCollider);
			if (isTouchingRocket) {
				playRocketPushSound();
				Collider2D touchedCollider = Rocket.instance.CollideOther(transform.position, noseCollisionRadius);
				if (touchedCollider != null) {
					isCollidingFront = true;
				}
			}
		}
		else {// Check if we hit anything with our carried resource
			CircleCollider2D myResourceCollider = resourceGatherer.holdResourceCollider;
			float circleCastRadius = myResourceCollider.radius * myResourceCollider.transform.localScale.x;
			RaycastHit2D[] circleCasts = Physics2D.RaycastAll(myResourceCollider.transform.position, transform.right, circleCastRadius, defaultLM);
			Debug.DrawRay(myResourceCollider.transform.position, transform.right * circleCastRadius, Color.black);
			foreach (RaycastHit2D circleCast in circleCasts) {
				Meteor collidingMeteor = GetResource(circleCast.collider);
				isTouchingRocket = TryRocketCollision(circleCast.collider);
				if (collidingMeteor != null && collidingMeteor.transform != myResourceCollider.transform) {
					isCollidingFront = true;
				} else if (isTouchingRocket) {
					resourceGatherer.DeliverResource();					
					playRocketPushSound();
				}
			}
		}

		if (didTouchRocket != isTouchingRocket) {
			if (!didTouchRocket) {
				onRocketCollisionEnter?.Invoke();
			}else {
				onRocketCollisionExit?.Invoke();
			}
		}

		return isCollidingFront;
	}

	private Meteor GetResource(Collider2D meteorCollider) {
		if(meteorCollider == null) { return null; }
		if (!meteorCollider.CompareTag(TAG_METEOR)) { return null; }
		Meteor meteor = meteorCollider.GetComponent<Meteor>();
		if (!meteor.containsResource) { return null; }
		return meteor;
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

		Gizmos.color = Color.green;


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
            if (!resourceOnly || meteor.containsResource) { return meteor; }
        }

        return null;
    }

	private bool TryRocketCollision(Collider2D collider) {
		if (didTouchRocket) { return false; }
		if (collider == null) { return false; }
		if (!collider.CompareTag(TAG_ROCKET)) { return false; }
		return true;
	}

}