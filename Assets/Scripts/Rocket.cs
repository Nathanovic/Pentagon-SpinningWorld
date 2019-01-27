using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

	public Transform earth;

	public float launchPower;
	public float liftOffAccelerateFactor = 3f;
	public float standardAccelerateFactor = 1f;
	public float finishLiftOffSpeed = 50f;

	private float currentSpeed;
	private bool isLaunched;

	private static List<Player> touchingPlayers = new List<Player>();

    private void Update() {
		if (Input.GetKeyUp(KeyCode.Space)) {
			Launch();
		}

		if (isLaunched) {
			float acceleration = launchPower;
			if (currentSpeed < finishLiftOffSpeed) {
				acceleration += currentSpeed * (liftOffAccelerateFactor - 1f);
			} else {
				acceleration += finishLiftOffSpeed * (standardAccelerateFactor - 1f);
			}

			currentSpeed += acceleration * Time.deltaTime;
			transform.position = transform.position + transform.up * currentSpeed * Time.deltaTime;
		}
    }

	public void StartPlayerTouch(Player player) {
		if (touchingPlayers.Contains(player)) {
			Debug.LogWarning("Player " + player.name + " is already added, aborting...");
			return;
		}

		touchingPlayers.Add(player);
	}

	public static bool IsTouchedByTwoPlayers() {
		return touchingPlayers.Count == 2;
	}

	public void StopPlayerTouch(Player player) {
		touchingPlayers.Remove(player);
	}

	private void Launch() {
		//AkSoundEngine.Postevent("Rocket_Launch", gameobject);
		print("Sound effect: Rocket_Launch");
		
		isLaunched = true;
		transform.SetParent(null);
	}

}
