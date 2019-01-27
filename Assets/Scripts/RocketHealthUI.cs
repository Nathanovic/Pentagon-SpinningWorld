using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RocketHealthUI : MonoBehaviour {

	public Transform healthTextOrigin;
	public Text healthText;

    void Start() {
		Rocket.instance.onHealthChanged += OnHealthChanged;
		OnHealthChanged(Rocket.instance.rocketHealth);
	}

	private void Update() {
		healthText.transform.position = healthTextOrigin.position;
	}

	private void OnHealthChanged(int newHealth) {
		healthText.text = newHealth.ToString() + "%";
	}

	private IEnumerator DisableSelfAfterTime(float delay) {
		yield return new WaitForSeconds(delay);
		healthText.enabled = false;
	}

}