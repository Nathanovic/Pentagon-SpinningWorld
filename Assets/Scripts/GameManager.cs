using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager Instance { get; private set; }

	public int selectedItem = 0; //Debug public

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
		Restart
	}
	private GameState gameState;
	public bool IsPlaying { get { return gameState == GameState.Playing; } }

	private List<Player> players = new List<Player>();
	private int deadPlayerCount;
	public float restartDuration = 1f;

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

    private void Start() {
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

    public void StartGame() {
		deadPlayerCount = 0;
		gameState = GameState.Playing;
		menuScreen.Deactivate();
		restartScreen.Deactivate();
	    
		//AkSoundEngine.Postevent("Restart", gameobject);
	    print("Sound effect: Restart");		
	    foreach (Player player in players) {
			player.Revive();
		}
	}

	public void NotifyPlayerDeath() {
		deadPlayerCount++;
		if (deadPlayerCount >= players.Count) {
			gameState = GameState.Restart;
			restartScreen.Activate();
		}
	}

	public void RestartGame() {
		StartCoroutine(RestartOverTime());
	}

	private IEnumerator RestartOverTime() {
		float t = 0f;
		while (t < 1f) {
			t += Time.deltaTime / restartDuration;
			restartCanvasGroup.alpha = 1f - t;
			yield return null;
		}

		StartGame();
	}

}
