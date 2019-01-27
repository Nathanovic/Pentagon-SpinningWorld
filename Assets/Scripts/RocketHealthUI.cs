using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RocketHealthUI : MonoBehaviour {

	public Transform healthTextOrigin;
	public Text healthText;
	private bool gameEnded;

    void Start() {
		Rocket.instance.onHealthChanged += OnHealthChanged;
		OnHealthChanged(Rocket.instance.rocketHealth);
	}

	private void Update() {
		if (gameEnded) { return; }
		healthText.transform.position = healthTextOrigin.position;
	}

	private void OnHealthChanged(int newHealth) {
		healthText.text = newHealth.ToString() + "%";
		if(newHealth <= 0 || newHealth >= 100) {
			healthText.enabled = false;
			gameEnded = true;
		} else {
			healthText.enabled = true;
			gameEnded = false;
		}
	}

	private IEnumerator DisableSelfAfterTime(float delay) {
		yield return new WaitForSeconds(delay);
		healthText.enabled = false;
		gameEnded = true;
	}

}