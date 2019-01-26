using UnityEngine;

public class Resource : MonoBehaviour {
    public int amount = 1;
    public bool isHeld { get; private set; }

    public void PickUp() {
        isHeld = true;
    }

    public void Drop() {
        isHeld = false;
    }
}