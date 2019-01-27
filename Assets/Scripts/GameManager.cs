using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager Instance { get; private set; }

	public int selectedItem = 0; //Debug public

	public Image ugleGGJImage;
	public float uglyGGJImageShowDuration = 1.5f;
	public float uglyGGJImageFadeDuration = 0.5f;

	public CanvasGroup blackCanvasGroup;
	public MenuScreen menuScreen;
	public CanvasGroup restartCanvasGroup;
	public MenuScreen restartScreen;
	public Color textActive;
	public Color textInActive;
	public int fontSizeActive;
	public int fontSizeInActive;

	public enum GameState {
		Menu,
		Playing,
		Restart,
		FadeInGame
	}
	private GameState gameState;
	public bool IsPlaying { get { return gameState == GameState.Playing; } }
	private bool isFading;

	private List<Player> players = new List<Player>();
	private int deadPlayerCount;
	public float fadeDuration = 1f;

	private void Awake() {
		Instance = this;
	}

	public void SetTextActive(Text text, bool isActive) {
		text.color = isActive ? textActive : textInActive;
		text.fontSize = isActive ? fontSizeActive : fontSizeInActive;
	}

	public void InitializePlayer(Player player) {
		players.Add(player);
	}

    private IEnumerator Start() {
		yield return new WaitForSeconds(uglyGGJImageShowDuration);
		float t = 0f;
		while (t < 1f) {
			t += Time.deltaTime / uglyGGJImageFadeDuration;
			ugleGGJImage.color = new Color(1,1,1, 1f - t);
			yield return null;
		}
		EnterMenu();
    }

	private void EnterMenu() {
		StartCoroutine(FadeBlackGroup());
		menuScreen.Activate();
	}

    private IEnumerator FadeBlackGroup() {
        while (true) {
			if (IsPlaying) { yield return null; }

            float blackA = Random.Range(0.3f, 0.4f);
            float timeA = Random.Range(1.3f, 2.1f);
            float t = 0f;
            while (t < 1f) {
                t += Time.deltaTime / timeA;
                blackCanvasGroup.alpha = t * blackA;
                yield return null;
            }
            while (t > 0f) {
                t -= Time.deltaTime / timeA;
                blackCanvasGroup.alpha = t * blackA;
                yield return null;
            }
        }        
    }

    public void StartCameraMotion() {
		if (gameState != GameState.Menu) { return; }
		gameState = GameState.FadeInGame;
		StartCoroutine(CameraMotionDelay());
    }

	private IEnumerator CameraMotionDelay() {
		Animator cameraAnimator = GameObject.Find("CameraParent").GetComponent<Animator>();
		cameraAnimator.SetTrigger("startCameraMotion");
		menuScreen.Deactivate();
		yield return new WaitForSeconds(4f);
		menuScreen.Fade(1f, 0f, () => {
			StartGame();
		});
	}

    public void StartGame() {
		deadPlayerCount = 0;
		gameState = GameState.Playing;
		menuScreen.Deactivate();
		restartScreen.Deactivate();
	    
		//AkSoundEngine.PostEvent("Restart", gameObject);
		AkSoundEngine.SetState("Muziek", "Start");

	    foreach (Player player in players) {
			player.Revive();
		}

		Rocket.instance.Initialize();
	}

	public void QuitGame() {
		Application.Quit();
	}

	public void NotifyPlayerDeath() {
		if(gameState != GameState.Playing) { return; }
		deadPlayerCount++;
		if (deadPlayerCount >= players.Count) {
			AkSoundEngine.SetState("Muziek", "Dood");
			EnterRestartState();
		}
	}

	public void NotifyRocketDestroyed() {
		EnterRestartState();
	}

	private void EnterRestartState() {
		if (gameState == GameState.Restart) { return; }
		gameState = GameState.Restart;
		restartScreen.Fade(0f, 1f, () => {
			restartScreen.Activate();
			AkSoundEngine.PostEvent("Stop_Geluid", gameObject);
		});
	}

	public void RestartGame() {
		if(gameState == GameState.FadeInGame) { return; }
		gameState = GameState.FadeInGame;

		Meteor[] allMeteors = FindObjectsOfType<Meteor>();
		foreach (Meteor meteor in allMeteors) {
			Destroy(meteor.gameObject);
		}

		restartScreen.Fade(1f, 0f, () => {
			StartGame();
		});
	}

}
