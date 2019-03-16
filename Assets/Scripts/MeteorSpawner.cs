using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeteorSpawner : MonoBehaviour {

	[Serializable]
	public class MeteorSettings {
		public Meteor prefab;
		public float priority;

		[Header("Calculated on start: ")]
		public float chanceValue;
	}

	public MeteorSettings[] meteorSpawners;
	private float remainingSpawnWaitTime;

    public float spawnInterval = 1;
    public float spawnOffsetFromMars = 15.0f;
	public float spawnOffsetZ = -3f;
    
    public float maxMeteorFallingSpeed = 50;
    public float minMeteorFallingSpeed = 2;
    public float minMeteorRotationMultiplier = 1.0f;
    public float maxMeteorRotationMultiplier = 4.0f;
    public float minMeteorScale = 0.5f;
    public float maxMeteorScale = 2;

	private void Start() {
		float totalSpawnPriority = 0;
		foreach(MeteorSettings spawner in meteorSpawners) {
			totalSpawnPriority += spawner.priority;
		}

		float chanceValue = 0f;
		foreach (MeteorSettings spawner in meteorSpawners) {
			chanceValue += spawner.priority / totalSpawnPriority;
			spawner.chanceValue = chanceValue;
		}

		remainingSpawnWaitTime = spawnInterval;
	}
	
    void Update() {
		if (!GameManager.Instance.IsPlaying) { return; }

		remainingSpawnWaitTime -= Time.deltaTime;

        if (remainingSpawnWaitTime <= 0) {
			float randomValue = Random.value;
            foreach(MeteorSettings spawner in meteorSpawners) {
				if(randomValue <= spawner.chanceValue) {
					SpawnMeteor(spawner.prefab);
					break;
				}
			}
        }
    }

    private void SpawnMeteor(Meteor meteorToSpawn) {
        int spawnAngle = Random.Range(0, 360);
		Meteor meteor = Instantiate(meteorToSpawn, new Vector3(Mathf.Sin(spawnAngle) * spawnOffsetFromMars, Mathf.Cos(spawnAngle) * spawnOffsetFromMars, spawnOffsetZ), Quaternion.identity);
        WorldBody worldBody = meteor.GetComponent<WorldBody>();
        float speed1 = Random.Range(minMeteorFallingSpeed, maxMeteorFallingSpeed);
        float speed2 = Random.Range(minMeteorFallingSpeed, maxMeteorFallingSpeed);
        worldBody.minSpeed = speed1 < speed2 ? speed1 : speed2;
        worldBody.maxSpeed = speed1 < speed2 ? speed2 : speed1;

        float size = Random.Range(minMeteorScale, maxMeteorScale);
        meteor.transform.localScale *= size;

		meteor.onImpact += () => {
            Screenshake.instance.StartShakeVertical(2, 0.02f * size * (speed2 + 5), 0.003f * size * (speed2 + 5));
        };

		float rotateMultiplier = Random.Range(minMeteorRotationMultiplier, maxMeteorRotationMultiplier);
		meteor.SetRotateMultiplier(rotateMultiplier);
		remainingSpawnWaitTime = spawnInterval;
	}

}