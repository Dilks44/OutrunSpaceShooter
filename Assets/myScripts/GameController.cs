using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameController : MonoBehaviour {
	public GameObject ResetButton;
	public GameObject BackToMainBtn;
	public GameObject[] hazards;
	public GameObject player;
	public float ObjOntopPlayerBuffer;
	public float ObjOntopEnemyBuffer;
	public int hazardCount;
	public int maxHazardsOnScreen;
	private int numHazardsOnScreen;
	public Vector2 xRange;
	public Vector2 yRange;
	public float zSpawn;
	public float startWait;
	public float spawnWait;
	public float dificultyMult;
	public Text scoreText;
	public int score;
	private int highScore;

	// Use this for initialization
	void Start () {
		score = 0;
		highScore = 0;
		numHazardsOnScreen = 0;
		UpdateScoreText();
		Load ();
		// Debug.Log ("Load called in start");
		// Debug.Log ("Highscore: " + highScore);
		StartCoroutine (SpawnWaves());
	}

	void Update () {
		if (player == null) {
			if (score > highScore) {
				Save ();
				// Debug.Log ("Save called in Updade");
				highScore = score;
				scoreText.text = "New High Score!!: " + highScore;
			}
			ResetButton.SetActive (true);
			BackToMainBtn.SetActive (true);
		}
	}

	IEnumerator SpawnWaves() {
		while (player != null) {
			// Debug.Log ("startWait: " + startWait);
			yield return new WaitForSeconds (startWait);
			
			for (int i = 0; i < hazardCount; i++) {
				// make sure I dont spawn too many hazards and cause fps issues
				if (numHazardsOnScreen < maxHazardsOnScreen) {

					GameObject hazard = hazards[UnityEngine.Random.Range(0, hazards.Length)];
					Vector3 spawnPosition = new Vector3 (
						UnityEngine.Random.Range(xRange.x, xRange.y),
						UnityEngine.Random.Range(yRange.x, yRange.y),
						zSpawn);

					GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
					while (isOntopPlayer (spawnPosition) || isOntopOfAnyEnemy (spawnPosition, enemies) ) {
						spawnPosition = new Vector3 (
							UnityEngine.Random.Range(xRange.x, xRange.y),
							UnityEngine.Random.Range(yRange.x, yRange.y),
							zSpawn);
					}

					if (isOntopOfAnyEnemy(spawnPosition, enemies)) {
						Debug.Log("wouldnt have spawned here due to enemeies");
					}
					
					Quaternion spawnRotation = Quaternion.identity;
					numHazardsOnScreen++;
					Instantiate (hazard, spawnPosition, spawnRotation);
				}
				else {
					Debug.Log ("Max items on screen reached. MAX = "+maxHazardsOnScreen
						+", CUR = " +numHazardsOnScreen);
				}

				yield return new WaitForSeconds (spawnWait);
			}

			startWait *= dificultyMult;
			spawnWait *= dificultyMult;
		}
	}

	private Boolean isOntopOfAnyEnemy(Vector3 location, GameObject[] enemies) {
		Boolean isOnTop = false;
		for (int j = 0; j < enemies.Length; j++) {
			if (isOntopofEnemy (location, enemies[j])) {
				isOnTop = true;
			}
		}
		return isOnTop;
	}

	private Boolean isOntopPlayer(Vector3 location) {
		if (player != null) {
			Vector3 playerPos = player.transform.position;

			if (Math.Abs (location.x - playerPos.x) < ObjOntopPlayerBuffer
			    && Math.Abs (location.y - playerPos.y) < ObjOntopPlayerBuffer) {
					Debug.Log ("Rejected location because its on player");
				//			Debug.Log ("Object spawning at x: " + location.x + ", y: " + location.y);
				//			Debug.Log ("Player at x: " + playerPos.x + ", y: " + playerPos.y);
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}

	private Boolean isOntopofEnemy(Vector3 location, GameObject enemy) {
		if (enemy != null) {
			Vector3 enemyPos = enemy.transform.position;
			if (Math.Abs (location.x - enemyPos.x) < ObjOntopEnemyBuffer
			    && Math.Abs (location.y - enemyPos.y) < ObjOntopEnemyBuffer) {
					Debug.Log ("Rejected location because its on enemy");
				//			Debug.Log ("Object spawning at x: " + location.x + ", y: " + location.y);
				//			Debug.Log ("Player at x: " + enemyPos.x + ", y: " + enemyPos.y);
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}

	public void AddScore(int toAdd) {
		score += toAdd;
		if (score > highScore) {
			scoreText.color = Color.green;
		}
		UpdateScoreText();
	}

	public void DecreaseHazardCount() {
		numHazardsOnScreen--;
	}

	void UpdateScoreText() {
		scoreText.text = "Score: " + score;
	}

	public void Load() {
		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			PlayerData data = (PlayerData) bf.Deserialize (file);
			file.Close();

			highScore = data.highScore;
		}
	}

	public void Save() {
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");

		PlayerData data = new PlayerData ();
		data.highScore = score;

		bf.Serialize (file, data);
		file.Close();
	}
}

[Serializable]
class PlayerData {
	public int highScore;
}