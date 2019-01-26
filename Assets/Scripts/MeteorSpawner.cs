using UnityEngine;

public class MeteorSpawner : MonoBehaviour {
    public GameObject meteorPrefab;
    public float spawnInterval = 1;
    public float offset = 15.0f;
    public float timeTillSpawn = 0.0f;
    
    public float maxMeteorFallingSpeed = 50;
    public float minMeteorFallingSpeed = 2;
    public float minMeteorScale = 0.5f;
    public float maxMeteorScale = 2;

    // Update is called once per frame
    void Update() {
        timeTillSpawn -= Time.deltaTime;

        if (timeTillSpawn <= 0) {
            SpawnMeteor();
            timeTillSpawn = spawnInterval;
        }
    }

    private void SpawnMeteor() {
        int spawnAngle = Random.Range(0, 360);
        GameObject meteor = Instantiate(meteorPrefab, new Vector3(Mathf.Sin(spawnAngle) * offset, Mathf.Cos(spawnAngle) * offset, 1), Quaternion.identity);
        WorldBody worldBody = meteor.GetComponent<WorldBody>();
        float speed1 = Random.Range(minMeteorFallingSpeed, maxMeteorFallingSpeed);
        float speed2 = Random.Range(minMeteorFallingSpeed, maxMeteorFallingSpeed);
        worldBody.minSpeed = speed1 < speed2 ? speed1 : speed2;
        worldBody.maxSpeed = speed1 < speed2 ? speed2 : speed1;

        float size = Random.Range(minMeteorScale, maxMeteorScale);
        meteor.transform.localScale *= size;
    }
}