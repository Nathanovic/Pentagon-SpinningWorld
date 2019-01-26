using UnityEngine;

public class PlayerCollision : MonoBehaviour {
    public float noseCollisionOffset = 0.2f;
    public Transform noseTransform;
    
    public delegate void CollisionFunction(Transform transform);
    public event CollisionFunction onResourceHit;
	public event CollisionFunction onFallHit;

	public Vector2 boxSize = new Vector2(2, 1);
	public float boxYOffset = 1f;

    public bool collideFront;

    public void EarlyUpdate() {
        Vector2 noseDir = transform.right * -transform.localScale.x;
        Debug.DrawRay(noseTransform.transform.position, noseDir.normalized * noseCollisionOffset, Color.red);
        RaycastHit2D hitInfo = Physics2D.Raycast(noseTransform.transform.position, noseDir, noseCollisionOffset);
        collideFront = false;
        if (hitInfo.collider != null && hitInfo.collider.CompareTag("Meteor")) {
            Meteor meteor = hitInfo.collider.GetComponent<Meteor>();
            if (meteor.containsResource) {
                collideFront = true;
                onResourceHit?.Invoke(hitInfo.collider.transform);
            }
        }

		Vector2 boxCastPos = transform.position + transform.up * boxYOffset;
		RaycastHit2D boxCast = Physics2D.BoxCast(boxCastPos, boxSize, 0f, transform.forward);
    }

	private void OnDrawGizmos() {
		Gizmos.color = Color.grey;
		Vector3 cubeSize = boxSize;
		Gizmos.DrawWireCube(transform.position - cubeSize * 0.5f, cubeSize);
	}
}