using UnityEngine;

public class Player : MonoBehaviour {
	
	public int playerNumber = 1;
	public float maxSpeed = 3f;

	public float currentSpeed;
	public float moveForce = 5f;
	public float slowDownForce = 10f;

	private bool isFacingLeft = true;
	private bool isDead;

	private PlayerCollision collisionScript;
	[SerializeField] private GameObject carVisual;
	[SerializeField] private ParticleSystem[] deadVFX;

	private void Awake() {
		collisionScript = GetComponent<PlayerCollision>();
		collisionScript.onFallHit += OnFallHit;
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
		
		if (collisionScript.isCollidingFront) {
			currentSpeed = 0f;
		} else {
			if (input != 0f) {
				currentSpeed += moveForce * input * Time.deltaTime;
				currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
			}
		}
		
		transform.RotateAround(World.Position, Vector3.forward, currentSpeed * Time.deltaTime);
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

	private void OnFallHit(Transform meteor) {
		if (isDead) { return; }

		isDead = true;
		carVisual.SetActive(false);
		Screenshake.instance.StartShakeHorizontal(2, 0.5f, 0.05f);
		foreach (ParticleSystem deadEffect in deadVFX) {
			deadEffect.Play();
		}

		GameManager.Instance.NotifyPlayerDeath();
	}

}