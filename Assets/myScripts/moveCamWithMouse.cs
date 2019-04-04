using UnityEngine;
using System.Collections;

public class moveCamWithMouse : MonoBehaviour {

	public float horizontalSpeed = 5;
	public float verticalSpeed = 5;
	
	// Update is called once per frame
	void Update () {
		float h = horizontalSpeed * Input.GetAxis("Mouse X");
		float v = verticalSpeed * Input.GetAxis("Mouse Y");

		if (Input.GetKey ("space")) {
			transform.Rotate (-v, h, 0);
		}
	}
}
