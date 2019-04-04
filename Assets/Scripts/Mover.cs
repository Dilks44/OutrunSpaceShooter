using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	public float speed;

	void Start() {
		Rigidbody rb = GetComponent<Rigidbody> ();
		rb.velocity = transform.up * speed;
	}





}
