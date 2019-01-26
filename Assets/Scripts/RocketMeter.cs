using UnityEngine;
using UnityEngine.UI;

public class RocketMeter : MonoBehaviour {

	[SerializeField] private Image fillImage;
	private bool isActive = true;
	private float increase = 1f;
	private float currentFill = 0f;
	private float increaseSpeed = 1f;
	private float increaseDirection = 1f;

    private void Start() {
        
    }
	
    private void Update() {
		if (isActive) {
			currentFill += increaseSpeed * increaseDirection * Time.deltaTime;
			if (currentFill >= 1f) {

			}
		}
    }

	public void ActivateMeter() {

	}

}