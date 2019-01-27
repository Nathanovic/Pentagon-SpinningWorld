using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;

public class Rocket : MonoBehaviour {

	public static Rocket instance;

	public Transform earth;

	public float launchPower;
	public float liftOffAccelerateFactor = 3f;
	public float standardAccelerateFactor = 1f;
	public float finishLiftOffSpeed = 50f;

	private float currentSpeed;
	private bool isLaunched;

	public float collisionYOffset = 1f;
	public float collisionXOffset = 0.3f;
	public LayerMask collisionLM;
	private BoxCollider2D boxCollider;

	public delegate void HealthChanged(int newHealth);
	public event HealthChanged onHealthChanged;
	public int rocketMaxHealth = 100;
	public int rocketStartHealth = 50;
	public int rocketHealth { get; private set; }
	public int meteorDamage = 50;

	public Transform deadVFXParent;
	private ParticleSystem[] deadVFX;

	public Action onInitialize;

	private void Awake() {
		instance = this;
		deadVFX = deadVFXParent.GetComponentsInChildren<ParticleSystem>();
	}

	public void Initialize() {
		gameObject.SetActive(true);
		rocketHealth = rocketStartHealth;
		ChangeHealth(0);
	}

	private void Start() {
		boxCollider = GetComponent<BoxCollider2D>();
	}

	private void Update() {
		RaycastHit2D[] raycastHits = Physics2D.BoxCastAll(transform.position, new Vector2(boxCollider.size.x, boxCollider.size.y * 2), transform.rotation.eulerAngles.z, Vector2.zero, 1);
		foreach (RaycastHit2D hit2D in raycastHits) {
			if(hit2D.collider == boxCollider) continue;
			if (hit2D.transform.CompareTag("Meteor")) {
				Resource resource = hit2D.collider.GetComponent<Resource>();
				if (resource == null || !resource.isHeld) {
					hit2D.collider.GetComponent<Meteor>().CollideRocket();
					ChangeHealth(-meteorDamage);
				}
			}
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

	private void Launch() {
		AkSoundEngine.PostEvent("Rocket_Launch", gameObject);
		StartCoroutine(launchAfterSeconds(2));
	}


	private IEnumerator launchAfterSeconds(float seconds) {
		yield return new WaitForSeconds(seconds);
		isLaunched = true;
		transform.SetParent(null);
	}

	public Collider2D CollideOther(Vector3 checkingPlayerPos, float checkDistance) {
		Vector3 inverseTransformPos = transform.InverseTransformPoint(checkingPlayerPos);
		float xMultiplier = (inverseTransformPos.x < 0f) ? 1f : -1f;
		Vector2 startPos = transform.position + transform.up * collisionYOffset;
		Vector2 collisionCheck = (startPos + (Vector2)transform.right * xMultiplier) - startPos;
		startPos += collisionCheck.normalized * collisionXOffset;
		Debug.DrawRay(startPos, collisionCheck.normalized * checkDistance, Color.yellow);
		RaycastHit2D hitInfo = Physics2D.Raycast(startPos, collisionCheck, checkDistance, collisionLM);
		return hitInfo.collider;
	}

	public void DeliverResource(Resource resource) {
		ChangeHealth(resource.repairPower);
		AkSoundEngine.PostEvent("Rocket_Repair", gameObject);
	}

	private void ChangeHealth(int change) {
		if (rocketHealth >= rocketMaxHealth) { return; }
		rocketHealth += change;

		if (rocketHealth >= rocketMaxHealth) {
			rocketHealth = rocketMaxHealth;
			Launch();
		} else if (rocketHealth <= 0) {
			rocketHealth = 0;
			LoseGame();
		}

		if (change < 0) {
			AkSoundEngine.PostEvent("Wagen_Destroyed", gameObject);
		}

		onHealthChanged?.Invoke(rocketHealth);
	}

	private void LoseGame() {
		deadVFXParent.SetParent(null);GameManager.Instance.NotifyRocketDestroyed();
		foreach (ParticleSystem deadEffect in deadVFX) {
			deadEffect.Play();
		}
		gameObject.SetActive(false);
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.grey;
		Gizmos.DrawWireCube(transform.position + transform.up * collisionYOffset, new Vector3(0.2f, 0.1f));
		
		Gizmos.color = Color.red;
		//DefaultNamespace.DebugUtils.DrawBoxCast2D(transform.position, new Vector2(boxCollider.size.x, boxCollider.size.y * 2), transform.rotation.eulerAngles.z, Vector2.zero, 1, Color.red);

	}

}
