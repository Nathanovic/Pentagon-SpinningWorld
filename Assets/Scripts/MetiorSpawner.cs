using UnityEngine;

public class MetiorSpawner : MonoBehaviour {
    public GameObject metiorPrefab;
    public float spawnInterval = 1;
    public float offset = 7.5f;
    public float timeTillSpawn = 0.0f;

    // Update is called once per frame
    void Update() {
        timeTillSpawn -= Time.deltaTime;

        if (timeTillSpawn <= 0) {
            int spawnAngle = Random.Range(0, 360);
            Instantiate(metiorPrefab, new Vector3(Mathf.Sin(spawnAngle) * offset, Mathf.Cos(spawnAngle) * offset, 1), Quaternion.identity);
            timeTillSpawn = spawnInterval;
        }
    }
}