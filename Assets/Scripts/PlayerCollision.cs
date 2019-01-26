using UnityEngine;

public class PlayerCollision : MonoBehaviour {
    public float noseCollisionOffset = 0.2f;
    public Transform noseTransform;
    
    public delegate void CollisionFunction(Transform transform);
    public event CollisionFunction onResourceHit;
	public event CollisionFunction onFallHit;

	public Transform deadCheckOrigin; 
	public float deadCheckRadius = 1f;

    public bool collideFront;

    public void EarlyUpdate() {
        Vector2 noseDir = transform.right * -transform.localScale.x;
        Debug.DrawRay(noseTransform.transform.position, noseDir.normalized * noseCollisionOffset, Color.red);
        RaycastHit2D hitInfo = Physics2D.Raycast(noseTransform.transform.position, noseDir, noseCollisionOffset);
        collideFront = false;
		Transform resource = HitInfoMeteor(hitInfo, true);
		if(resource != null) { 
            collideFront = true;
            onResourceHit?.Invoke(hitInfo.collider.transform);
        }
		
		RaycastHit2D boxCast = Physics2D.CircleCast(deadCheckOrigin.position, deadCheckRadius, Vector2.zero);
		Transform meteor = HitInfoMeteor(boxCast, false);
		if (meteor != null) {
			onFallHit?.Invoke(meteor);
		}
    }

	private void OnDrawGizmos() {
		Gizmos.color = Color.grey;
		Gizmos.DrawWireSphere(deadCheckOrigin.position, deadCheckRadius);
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