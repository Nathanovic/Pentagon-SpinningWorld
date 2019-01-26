using UnityEngine;

public class PlayerCollision : MonoBehaviour {
    public float noseCollisionOffset = 0.2f;
    public Transform noseTransform;

    public delegate void CollisionFunction(Meteor meteor);

    public event CollisionFunction onResourceHit;
    public event CollisionFunction onFallHit;

    public delegate void CollisionTransformFunction(Transform transform);

    public event CollisionTransformFunction onRocketChargePlateHit;

    public Transform deadCheckOrigin;
    public float deadCheckRadius = 1f;

    public bool isCollidingFront;

    private ResourceGatherer resourceGatherer;

    private void Start() {
        resourceGatherer = GetComponent<ResourceGatherer>();
    }

    public void EarlyUpdate() {
        isCollidingFront = false;
        Meteor collidingMeteor = null;

        // Check if we can pick up resources
        if (!resourceGatherer.hasResource) {
            Vector2 noseDir = transform.right * -transform.localScale.x;
            Debug.DrawRay(noseTransform.transform.position, noseDir.normalized * noseCollisionOffset, Color.red);
            RaycastHit2D hitInfo = Physics2D.Raycast(noseTransform.transform.position, noseDir, noseCollisionOffset);
            collidingMeteor = GetMeteor(hitInfo, true);
            if (collidingMeteor != null) {
                onResourceHit?.Invoke(collidingMeteor);
            }
        }
        // Check if we hit something with our carried resource
        else {
            CircleCollider2D myResourceCollider = resourceGatherer.holdResourceCollider;
            float circleCastRadius = myResourceCollider.radius * myResourceCollider.transform.localScale.x;
            RaycastHit2D[] circleCasts =
                Physics2D.CircleCastAll(myResourceCollider.transform.position, circleCastRadius, Vector2.zero);
            foreach (RaycastHit2D circleCast in circleCasts) {
                if (circleCast.transform.CompareTag("Meteor")) {
                    collidingMeteor = GetMeteor(circleCast, true);
                    if (collidingMeteor != null && collidingMeteor.transform != myResourceCollider.transform) {
                        isCollidingFront = true;
                    }
                }
                else  if (circleCast.collider.CompareTag("ChargeRocketPlate")) {
                    Debug.Log("Resources inleveren.");
                    onRocketChargePlateHit?.Invoke(circleCast.collider.transform);
                }
                else {
                    Debug.Log("Collision with " + circleCast.collider.tag);
                }

                /*if (collidingMeteor != null && collidingMeteor.transform != myResourceCollider.transform) {
					Debug.Log("meteor collision with: " + collidingMeteor.name);
                    isCollidingFront = true;
                }*/
            }
        }

        RaycastHit2D meteorCast = Physics2D.CircleCast(deadCheckOrigin.position, deadCheckRadius, Vector2.zero);
        Meteor meteor = GetMeteor(meteorCast, false);
        if (meteor != null) {
            if (meteor.canDamage) {
                onFallHit?.Invoke(meteor);
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
}