using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour{

    private MeteorSpawner meteorSpawner;
    private float previousTime;
    public float timer; //Debug

    public Text timerText;
    public int gameTime = 60;


    private void Start(){
        meteorSpawner = GameObject.Find("World").GetComponent<MeteorSpawner>();

        timer = gameTime;
    }
    
    private void Update(){
        if (GameManager.Instance.IsPlaying) {
            timer -= Time.deltaTime;
            

            if (timer <= 40 && previousTime > 40) {
                print("wordt 1 keer aangeroepen");
                meteorSpawner.spawnInterval = 0.8f;
            }
            if (timer <= 20 && previousTime > 20) {
                print("wordt 1 keer aangeroepen!");
                meteorSpawner.spawnInterval = 0.6f;
            }

            if (timer <= 0) {
                timer = 0f;
                // Game Logic moet worden aangepast
            }

            previousTime = timer;
        }
        else {
            timer = gameTime;
        }
        timerText.text = "" + (Mathf.FloorToInt(timer) + 1);
    }
        
}