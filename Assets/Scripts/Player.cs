using System;
using DefaultNamespace;
using UnityEngine;

public class Player : MonoBehaviour {
	
	public int playerNumber = 1;
	public float maxSpeed = 3f;

	public float rocketMoveSpeedFactor = 0.7f;

	public float currentSpeed;
	public float moveForce = 5f;
	public float slowDownForce = 10f;

	private bool isFacingLeft = true;
	private bool isDead;

	private bool drivingSoundIsPlaying = false;

	public PlayerCollision collisionScript { get; private set; }
	[SerializeField] private GameObject carVisual;
	private new Collider2D collider;
	[SerializeField] private ParticleSystem[] deadVFX;

	public Rocket collidingRocket { get; private set; }

	private void Awake() {
		collisionScript = GetComponent<PlayerCollision>();
		collider = GetComponent<Collider2D>();
		collisionScript.onFallHit += OnFallHit;
		collisionScript.onCollisionEnter += OnFrontColliderEnter;
		collisionScript.onCollisionExit += OnFrontColliderExit;
	}

	private void Start() {
		GameManager.Instance.InitializePlayer(this);
	}

	public void Revive() {
		isDead = false;
		carVisual.SetActive(true);
		collider.enabled = true;
		currentSpeed = 0f;
	}

	private void Update() {
		if (isDead) { return; }

		collisionScript.EarlyUpdate();
		
		float input = -Input.GetAxis("Horizontal_" + playerNumber.ToString());
		if (input != 0f) {
			SetFacingDirection(input);
		}

		// Slow car down
		float absSpeed = Mathf.Abs(currentSpeed);
		absSpeed -= slowDownForce * Time.deltaTime;
		if (absSpeed < 0) {
			absSpeed = 0f;
		}
		if (currentSpeed < 0) {
			currentSpeed = -absSpeed;
		} else {
			currentSpeed = absSpeed;
		}
		
		if (collisionScript.IsCollidingFront) {
			currentSpeed = 0f;
		} else {
			if (input != 0f) {
			
				currentSpeed += moveForce * input * Time.deltaTime;
				currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
				
			}

			float moveSpeed = currentSpeed;
			if (collidingRocket != null) {
				moveSpeed = currentSpeed * rocketMoveSpeedFactor;
				collidingRocket.transform.RotateAround(World.Position, Vector3.forward, moveSpeed * Time.deltaTime);
			}
			transform.RotateAround(World.Position, Vector3.forward, moveSpeed * Time.deltaTime);
		}

		if (Math.Abs(currentSpeed) > 0.001f) { // we rijden
			if (!drivingSoundIsPlaying) {
				print("Rijden_Wagen_" + playerNumber);
				//AkSoundEngine.Postevent("Rijden_Wagen_" + playerNumber, gameobject);
				drivingSoundIsPlaying = true;
			}
		} else if (drivingSoundIsPlaying) {
			print("Stop_Wagen_" + playerNumber);
			//AkSoundEngine.Postevent("Stop_Wagen_" + playerNumber, gameObject);
			drivingSoundIsPlaying = false;
		}
		
	}

	private void OnFrontColliderEnter(Collider2D collider) {
		if (collider.tag == "Rocket") {
			collidingRocket = collider.GetComponent<Rocket>();
		}
	}

	private void OnFrontColliderExit(Collider2D collider) {
		if(collider.tag == "Rocket") {
			collidingRocket = null;
		}
	}

	private void SetFacingDirection(float input) {
		if (input < 0f && isFacingLeft) {
			isFacingLeft = false;
			transform.localScale = new Vector3(-1, 1, 1);
		} else if (input > 0f && !isFacingLeft) {
			isFacingLeft = true;
			transform.localScale = new Vector3(1, 1, 1);
		}
	}

	private void OnFallHit(Meteor meteor) {
		if (isDead) { return; }

		isDead = true;
		carVisual.SetActive(false);
		collider.enabled = false;
		Screenshake.instance.StartShakeHorizontal(2, 0.5f, 0.05f);
		foreach (ParticleSystem deadEffect in deadVFX) {
			deadEffect.Play();
		}
		
		AkSoundEngine.PostEvent("Wagen_Destroyed", gameObject);
		GameManager.Instance.NotifyPlayerDeath();
	}

}