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

	public PlayerCollision collisionScript { get; private set; }
	[SerializeField] private GameObject carVisual;
	[SerializeField] private ParticleSystem[] deadVFX;

	private Rocket collidingRocket;

	private void Awake() {
		collisionScript = GetComponent<PlayerCollision>();
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
	}

	private void OnFrontColliderEnter(Collider2D collider) {
		if (collider.tag == "Rocket") {
			Debug.Log("Rocket touched!");
			collidingRocket = collider.GetComponent<Rocket>();
			collidingRocket.StartPlayerTouch(this);
		}
	}

	private void OnFrontColliderExit(Collider2D collider) {
		if(collider.tag == "Rocket") {
			Debug.Log("Rocket lost!");
			collidingRocket.StopPlayerTouch(this);
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
		Screenshake.instance.StartShakeHorizontal(2, 0.5f, 0.05f);
		foreach (ParticleSystem deadEffect in deadVFX) {
			deadEffect.Play();
		}

		if (collidingRocket != null) {
			collidingRocket.StopPlayerTouch(this);
		}

		GameManager.Instance.NotifyPlayerDeath();
	}

}