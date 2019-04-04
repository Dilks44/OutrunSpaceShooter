using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour {

	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue;

	private GameController gameController;

	// Use this for initialization
	void Start () {
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController> ();
		}

		if (gameController == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}
	}
	
	void OnTriggerEnter(Collider other) {

		// Dont destroy if obj ran into boundary or GameBoard
		if (other.tag == "Boundary" || other.tag == "GameBoard") {
			return;
		}

		if (explosion != null) {
			Vector3 temp = new Vector3 (transform.position.x, transform.position.y, transform.position.z-0.5f);
			Instantiate (explosion, temp, transform.rotation);
		}

		if (other.tag != "Enemy") {
			gameController.AddScore (scoreValue);
		}

		gameController.DecreaseHazardCount();
		Destroy (other.gameObject);
		Destroy (gameObject);

	}
}
