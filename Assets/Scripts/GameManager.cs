using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public int selectedItem = 0; //Debug public
    private int maxItems;

    public CanvasGroup blackCanvasGroup;
    public CanvasGroup menuCanvasGroup;
    public Text[] textItem;
    public Color textActive;
    public Color textInActive;
    public int fontSizeActive;
    public int fontSizeInActive;

    private void Start() {
        maxItems = textItem.Length;
        StartCoroutine(FadeBlackGroup());
    }

    private IEnumerator FadeBlackGroup() {
        while (true) {
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

    void Update(){
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (selectedItem > 0) {
                selectedItem--;
            }
            else if (selectedItem == 0) {
                selectedItem = 2;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            if (selectedItem < (maxItems - 1)) {
                selectedItem++;
            }
            else if (selectedItem == (maxItems - 1)) {
                selectedItem = 0;
            }
        }

        if(Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return)) {
            if(selectedItem == 0) {
                StartGame();
            }
        }

        for(int i = 0; i < maxItems; i++) {
            if(i == selectedItem) {
                textItem[i].color = textActive;
                textItem[i].fontSize = fontSizeActive;
            }
            else {
                textItem[i].color = textInActive;
                textItem[i].fontSize = fontSizeInActive;
            }
        }
    }

    private void StartGame() {

    }

}
