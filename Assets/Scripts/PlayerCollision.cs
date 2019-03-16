using UnityEngine;

public class PlayerCollision : MonoBehaviour {

	private const string TAG_METEOR = "Meteor";
	private const string TAG_ROCKET = "Rocket";

	private Player player;
	public float noseCollisionRadius = 0.2f;

    public delegate void CollisionFunction(Meteor meteor);

    public event CollisionFunction onFrontResourceHit;
    public event CollisionFunction onFallHit;

	public delegate void CollisionFrontDelegate(bool collideFront);
	public event CollisionFrontDelegate onRocketCollisionEnter;
	public delegate void CollisionDelegate();
	public event CollisionDelegate onRocketCollisionExit;

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
		if (isCarryingResource) {
			CircleCollider2D myResourceCollider = resourceGatherer.holdResourceCollider;
			float resourceSize = myResourceCollider.radius * myResourceCollider.transform.localScale.x;
			Collider2D rocketCollider = Physics2D.OverlapCircle(myResourceCollider.transform.position, resourceSize, rocketLM);
			isTouchingRocket = TryRocketCollision(rocketCollider);
			if (isTouchingRocket && isCarryingResource) {
				resourceGatherer.DeliverResource();
			}
		}
		else {
			Collider2D rocketCollider = Physics2D.OverlapCircle(frontChecker.position, noseCollisionRadius, rocketLM);
			isTouchingRocket = TryRocketCollision(rocketCollider);
			if (isTouchingRocket) {
				PlayRocketPushSound();
				Collider2D touchedCollider = Rocket.instance.CollideOther(transform.position, noseCollisionRadius);
				if (touchedCollider != null) {
					isCollidingFront = true;
				}
			}
		}

		// Check if we collide with our resource
		if (isCarryingResource) {
			CircleCollider2D myResourceCollider = resourceGatherer.holdResourceCollider;
			float resourceSize = myResourceCollider.radius * myResourceCollider.transform.localScale.x;
			Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(myResourceCollider.transform.position, resourceSize, defaultLM);
			foreach (Collider2D collider in overlappingColliders) {
				if(myResourceCollider == collider) { continue; }
				Meteor collidingMeteor = GetResource(collider);
				if (collidingMeteor != null && collidingMeteor.transform != myResourceCollider.transform) {
					isCollidingFront = true;
				}
			}
		}

		if (isCheckingFront) {
			if (didTouchRocket != isTouchingRocket) {
				//Debug.Log("Did-touch-rocket changed to: " + isTouchingRocket, transform);
				if (isTouchingRocket) {
					onRocketCollisionEnter?.Invoke(isCheckingFront);
				}
				else {
					onRocketCollisionExit?.Invoke();
				}

				didTouchRocket = isTouchingRocket;
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

	private void PlayRocketPushSound() {
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
		if (collider == null) { return false; }
		if (!collider.CompareTag(TAG_ROCKET)) { return false; }
		return true;
	}

}