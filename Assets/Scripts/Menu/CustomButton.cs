using UnityEngine;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour {

	public UnityEvent customAction;

	public void InvokeAction() {
		customAction?.Invoke();
	}

}