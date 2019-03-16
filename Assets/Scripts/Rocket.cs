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

	public LayerMask meteorLM;
	public Transform deadVFXParent;
	private Vector3 deadVFXLocalPosition;
	private Quaternion deadVFXLocalRotation;
	private ParticleSystem[] deadVFX;

	public Action onInitialize;
	private Vector3 startPos;
	private Quaternion startRot;

	private void Awake() {
		instance = this;
		startPos = transform.localPosition;
		startRot = transform.localRotation;
		deadVFX = deadVFXParent.GetComponentsInChildren<ParticleSystem>();
		deadVFXLocalPosition = deadVFXParent.localPosition;
		deadVFXLocalRotation = deadVFXParent.localRotation;
	}

	public void Initialize() {
		isLaunched = false;
		currentSpeed = 0f;
		gameObject.SetActive(true);
		rocketHealth = rocketStartHealth;
		ChangeHealth(0);

		transform.SetParent(World.Instance.transform);
		transform.localPosition = startPos;
		transform.localRotation = startRot;
	}

	private void Start() {
		boxCollider = GetComponent<BoxCollider2D>();
	}

	private void Update() {
		// Check for meteor damage
		Vector2 meteorCheckPosition = transform.position + transform.up * boxCollider.offset.y;
		Collider2D[] overlappingMeteors = Physics2D.OverlapBoxAll(meteorCheckPosition, boxCollider.size, transform.rotation.eulerAngles.z, meteorLM);
		foreach (Collider2D collider in overlappingMeteors) {
			if (!collider.CompareTag("Meteor")) { continue; }
			Meteor meteor = collider.GetComponent<Meteor>();
			if (meteor == null) { continue; }
			if (!meteor.canDamage) { continue; }
			
			meteor.CollideRocket();
			ChangeHealth(-meteorDamage);
		}
		
		// Check for rocket launch
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
		GameManager.Instance.FinishGame();
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
		if(resource == null) { return; }
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
		deadVFXParent.SetParent(transform);
		deadVFXParent.localPosition = deadVFXLocalPosition;
		deadVFXParent.localRotation = deadVFXLocalRotation;
		deadVFXParent.SetParent(null);

		GameManager.Instance.NotifyRocketDestroyed();
		foreach (ParticleSystem deadEffect in deadVFX) {
			deadEffect.Play();
		}
		gameObject.SetActive(false);
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.grey;
		Gizmos.DrawWireCube(transform.position + transform.up * collisionYOffset, new Vector3(0.2f, 0.1f));
		
		if(boxCollider == null) { return; }
		Gizmos.color = Color.red;
		Vector2 meteorCheckPosition = transform.position + transform.up * boxCollider.offset.y;
		DebugUtils.DrawBoxCast2D(meteorCheckPosition, boxCollider.size, transform.rotation.eulerAngles.z, Vector2.zero, 0f, Color.red);
	}

}
