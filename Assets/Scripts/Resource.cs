using UnityEngine;

public class Resource : MonoBehaviour {
    public int amount = 1;
    public bool isHeld { get; private set; }

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
}