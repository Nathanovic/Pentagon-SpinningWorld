using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeteorSpawner : MonoBehaviour {

    public GameObject meteorPrefab1;
    public GameObject meteorPrefab2;
    public float spawnChangeMeteor1 = 0.9f;
    public float spawnChangeMeteor2 = 0.1f;
    private float totalSpawnChange = 1;

    public float spawnInterval = 1;
    public float offset = 15.0f;
    public float timeTillSpawn = 0.0f;
    
    public float maxMeteorFallingSpeed = 50;
    public float minMeteorFallingSpeed = 2;
    public float minMeteorRotationMultiplier = 1.0f;
    public float maxMeteorRotationMultiplier = 4.0f;
    public float minMeteorScale = 0.5f;
    public float maxMeteorScale = 2;

	private void Start() {
        totalSpawnChange = spawnChangeMeteor1 + spawnChangeMeteor2;
    }
	
    void Update() {
		if (!GameManager.Instance.IsPlaying) { return; }

        timeTillSpawn -= Time.deltaTime;

        if (timeTillSpawn <= 0) {
            float random = Random.Range(0, totalSpawnChange);
            if (random <= spawnChangeMeteor1)
                SpawnMeteor(meteorPrefab1);
            else 
                SpawnMeteor(meteorPrefab2);
            timeTillSpawn = spawnInterval;
        }
    }

    private void SpawnMeteor(GameObject meteorToSpawn) {
        int spawnAngle = Random.Range(0, 360);
        GameObject meteor = Instantiate(meteorToSpawn, new Vector3(Mathf.Sin(spawnAngle) * offset, Mathf.Cos(spawnAngle) * offset, 1), Quaternion.identity);
        WorldBody worldBody = meteor.GetComponent<WorldBody>();
        float speed1 = Random.Range(minMeteorFallingSpeed, maxMeteorFallingSpeed);
        float speed2 = Random.Range(minMeteorFallingSpeed, maxMeteorFallingSpeed);
        worldBody.minSpeed = speed1 < speed2 ? speed1 : speed2;
        worldBody.maxSpeed = speed1 < speed2 ? speed2 : speed1;

        float size = Random.Range(minMeteorScale, maxMeteorScale);
        meteor.transform.localScale *= size;
        
        Meteor meteorComponent = meteor.GetComponent<Meteor>();
        meteorComponent.onImpact += () => {
            Screenshake.instance.StartShakeVertical(2, 0.02f * size * (speed2 + 5), 0.003f * size * (speed2 + 5));
        };
        meteorComponent.rotationMultiplier = Random.Range(minMeteorRotationMultiplier, maxMeteorRotationMultiplier);
    }
}