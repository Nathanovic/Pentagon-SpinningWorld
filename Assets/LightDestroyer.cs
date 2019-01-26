using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDestroyer : MonoBehaviour
{
    public void DestroyWhenDone() {
        StartCoroutine(DestroyObject());
    }

    private IEnumerator DestroyObject() {
        yield return new WaitForSeconds(0.05f);
        Destroy(this.gameObject);
    }
}
