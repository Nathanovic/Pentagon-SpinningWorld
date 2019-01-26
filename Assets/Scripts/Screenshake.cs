using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshake : MonoBehaviour {

    public static Screenshake instance = null;

    private float xShake;
    private float yShake;

    private void Awake () {
        instance = this;
    }

    private void LateUpdate() {
        CameraShake();
    }

    public void StartShakeHorizontal(int framesPerShake, float shakeDur, float shakeOffset) {
        StartCoroutine(ShakeHorizontal(framesPerShake, shakeDur, shakeOffset));
    }

    public void StartShakeVertical(int framesPerShake, float shakeDur, float shakeOffset) {
        StartCoroutine(ShakeVertical(framesPerShake, shakeDur, shakeOffset));
    }

    private IEnumerator ShakeHorizontal(int framesPerShake, float shakeDur, float shakeOffset) {
        float t = 0f;
        int a = 0;
        Vector3 shakePos = Vector3.zero;
        while(t < 1) {
            t += Time.deltaTime * framesPerShake / shakeDur;
            a++;
            if (a%2 == 1) {
                xShake = shakeOffset;
            }
            else if(a%2 == 0) {
                xShake = -shakeOffset;
            }

            for (int i = 0; i < framesPerShake; i++) {
                yield return null;
            }
        }
        xShake = 0f;
    }

    private IEnumerator ShakeVertical(int framesPerShake, float shakeDur, float shakeOffset) {
        float t = 0f;
        int a = 0;
        Vector3 shakePos = Vector3.zero;
        while (t < 1) {
            t += Time.deltaTime * framesPerShake / shakeDur;
            a++;
            if (a % 2 == 1) {
                yShake = shakeOffset;
            }
            else if (a % 2 == 0) {
                yShake = -shakeOffset;
            }

            for (int i = 0; i < framesPerShake; i++) {
                yield return null;
            }
        }
        yShake = 0f;
    }

    private void CameraShake() {
        transform.localPosition = new Vector3(xShake, yShake, transform.position.z);
    }
}
