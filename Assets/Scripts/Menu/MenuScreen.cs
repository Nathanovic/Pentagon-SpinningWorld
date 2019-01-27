using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour {

	private bool isActive;
	private CustomButton[] buttons;
	private Text[] textItems;
	private int selectedButtonIndex;

	private void Awake() {
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

	private void Update() {
		if (!isActive) { return; }

		if (Input.GetKeyDown(KeyCode.UpArrow)) {
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

		if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return)) {
			buttons[selectedButtonIndex].InvokeAction();
			AkSoundEngine.PostEvent("Select", gameObject);
		}

		for (int i = 0; i < buttons.Length; i++) {
			GameManager.Instance.SetTextActive(textItems[i], i == selectedButtonIndex);
		}
	}

}