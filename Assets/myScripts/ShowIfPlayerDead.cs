using UnityEngine;
using System.Collections;

public class ShowIfPlayerDead : MonoBehaviour {

	public GameObject player;
	public GameObject Button;


	
	// Update is called once per frame
	void Update () {
		if (player == null) {
			//Button.SetActive (true);

			Button.SetActive (true);

			//Button.GetComponent<MeshRenderer> ().enabled = true;
			//Button.GetComponent<MeshCollider> ().enabled = true;
		}
	
	}
}
