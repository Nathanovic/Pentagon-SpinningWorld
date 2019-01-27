using UnityEngine;

public class Resource : MonoBehaviour {
	public int repairPower = 10;
	public bool isHeld { get; private set; }
	private bool isDelivered;
	public bool canPickUp {
		get {
			if (isDelivered) { return false; }
			if (isHeld) { return false; }
			return true;
		}
	}

    private WorldBody worldBody;

    private void Awake() {
        worldBody = GetComponent<WorldBody>();
    }

    public void PickUp(Transform newParent) {
        worldBody.enabled = false;
        transform.SetParent(newParent);
        isHeld = true;
    }

    public void Drop() {
        worldBody.enabled = true;
        transform.SetParent(null);
        isHeld = false;
    }

	public void Deliver() {
		Drop();
		isDelivered = true;
		Destroy(gameObject);
	}
}