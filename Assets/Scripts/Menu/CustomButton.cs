using UnityEngine;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour {

	public UnityEvent customAction;

	public void InvokeAction() {
		Debug.Log("Invoke action for " + transform.name);
		customAction?.Invoke();
	}

}