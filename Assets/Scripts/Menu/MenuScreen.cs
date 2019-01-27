using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class MenuScreen : MonoBehaviour {

	private bool isActive;
	private CustomButton[] buttons;
	private Text[] textItems;
	private int selectedButtonIndex;

	private CanvasGroup canvasGroup;
	public float fadeDuration = 1f;

	private void Awake() {
		canvasGroup = GetComponent<CanvasGroup>();
		buttons = GetComponentsInChildren<CustomButton>();
		textItems = new Text[buttons.Length];
		for (int i = 0; i < buttons.Length; i ++) {
			textItems[i] = buttons[i].GetComponent<Text>();
		}
	}

	public void Activate() {
		isActive = true;
		GetComponent<CanvasGroup>().alpha = 1f;
	}

	public void Deactivate() {
		isActive = false;
		GetComponent<CanvasGroup>().alpha = 0f;
	}

	public void Fade(float startA, float endA, Action onDone) {
		StartCoroutine(FadeCanvasGroup(startA, endA, onDone));
	}

	private IEnumerator FadeCanvasGroup(float startA, float endA, Action onDone) {
		canvasGroup.alpha = startA;
		float t = 0f;
		while (t < 1f) {
			t += Time.deltaTime / fadeDuration;
			canvasGroup.alpha = Mathf.Lerp(startA, endA, t);
			yield return null;
		}
		onDone?.Invoke();
	}

	private void Update() {
		if (!isActive) { return; }

		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyUp("joystick button 5")) {
			if (selectedButtonIndex > 0) {
				selectedButtonIndex--;
			} else if (selectedButtonIndex == 0) {
				selectedButtonIndex = buttons.Length - 1;
			}
			AkSoundEngine.PostEvent("Scroll", gameObject);
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			if (selectedButtonIndex < (buttons.Length - 1)) {
				selectedButtonIndex++;
			} else if (selectedButtonIndex == (buttons.Length - 1)) {
				selectedButtonIndex = 0;
			}
			AkSoundEngine.PostEvent("Scroll", gameObject);
		}

		if (Input.GetButtonUp("Submit")) {
			buttons[selectedButtonIndex].InvokeAction();
			AkSoundEngine.PostEvent("Select", gameObject);
		}

		for (int i = 0; i < buttons.Length; i++) {
			GameManager.Instance.SetTextActive(textItems[i], i == selectedButtonIndex);
		}
	}

}