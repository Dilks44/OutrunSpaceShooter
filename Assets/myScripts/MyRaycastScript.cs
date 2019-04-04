using System;
using UnityEngine;

namespace VRStandardAssets.Utils
{
	// In order to interact with objects in the scene
	// this class casts a ray into the scene and if it finds
	// a VRInteractiveItem it exposes it for other classes to use.
	// This script should generally be placed on the camera.
	public class MyRaycastScript : MonoBehaviour
	{
		public event Action<RaycastHit> OnRaycasthit;                   // This event is called every frame that the user's gaze is over a collider.


		[SerializeField] private Transform m_Camera;
		[SerializeField] private LayerMask m_ExclusionLayers;           // Layers to exclude from the raycast.
		[SerializeField] private Reticle m_Reticle;                     // The reticle, if applicable.
		[SerializeField] private VRInput m_VrInput;                     // Used to call input based events on the current VRInteractiveItem.
		[SerializeField] private bool m_ShowDebugRay;                   // Optionally show the debug ray.
		[SerializeField] private float m_DebugRayLength = 5f;           // Debug ray length.
		[SerializeField] private float m_DebugRayDuration = 1f;         // How long the Debug ray will remain visible.
		[SerializeField] private float m_RayLength = 500f;              // How far into the scene the ray is cast.

		[SerializeField] private Transform playerTransform;
		[SerializeField] private float turnCircleRad = 0.3f;
		[SerializeField] private float playerMovementSpeed = 1.5f;


		private VRInteractiveItem m_CurrentInteractible;                //The current interactive item
		private VRInteractiveItem m_LastInteractible;                   //The last interactive item


		// Utility for other classes to get the current interactive item
		public VRInteractiveItem CurrentInteractible
		{
			get { return m_CurrentInteractible; }
		}


		private void OnEnable()
		{
			m_VrInput.OnClick += HandleClick;
			m_VrInput.OnDoubleClick += HandleDoubleClick;
			m_VrInput.OnUp += HandleUp;
			m_VrInput.OnDown += HandleDown;
		}


		private void OnDisable ()
		{
			m_VrInput.OnClick -= HandleClick;
			m_VrInput.OnDoubleClick -= HandleDoubleClick;
			m_VrInput.OnUp -= HandleUp;
			m_VrInput.OnDown -= HandleDown;
		}


		private void Update()
		{
			EyeRaycast();
		}


		private void EyeRaycast()
		{
			// Show the debug ray if required
			if (m_ShowDebugRay)
			{
				Debug.DrawRay(m_Camera.position, m_Camera.forward * m_DebugRayLength, Color.blue, m_DebugRayDuration);
			}

			// Create a ray that points forwards from the camera.
			Ray ray = new Ray(m_Camera.position, m_Camera.forward);
			RaycastHit hit;

			// Do the raycast forweards to see if we hit an interactive item
			if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers))
			{
				VRInteractiveItem interactible = hit.collider.GetComponent<VRInteractiveItem>(); //attempt to get the VRInteractiveItem on the hit object
				m_CurrentInteractible = interactible;

				// If we hit an interactive item and it's not the same as the last interactive item, then call Over
				if (interactible && interactible != m_LastInteractible)
					interactible.Over(); 

				// Deactive the last interactive item 
				if (interactible != m_LastInteractible)
					DeactiveLastInteractible();

				m_LastInteractible = interactible;

				// Something was hit, set at the hit position.
				if (m_Reticle) {
					m_Reticle.SetPosition(hit);


					// This code block rotates the ship to face the croshair and Moves the ship if far enough away
					if (playerTransform != null && m_CurrentInteractible != null && m_CurrentInteractible.gameObject.tag == "GameBoard") {
						float tempX = hit.point.x - playerTransform.position.x;
						float tempY = hit.point.y - playerTransform.position.y;
						//Debug.Log ("tempX: " + tempX);
						//Debug.Log ("tempY: " + tempY);
						float angle = Mathf.Atan2 (tempX, tempY) * Mathf.Rad2Deg;
						playerTransform.rotation = Quaternion.Euler (new Vector3 (0, 0, -angle));
						
						
						// If the croshair is outside the specified length move the ship towards it
						if (isOutsideCircle(hit.point, playerTransform.position)) {
							
							// place to move to
							//Debug.Log("Changing player pos");
							Vector3 newPlayerPos = new Vector3 (hit.point.x, hit.point.y, playerTransform.position.z);
							playerTransform.position = Vector3.Lerp (playerTransform.position, newPlayerPos, playerMovementSpeed * Time.deltaTime);
						}
					}
					// done with ship rotation code

				}

				if (OnRaycasthit != null)
					OnRaycasthit(hit);
			}
			else
			{
				// Nothing was hit, deactive the last interactive item.
				DeactiveLastInteractible();
				m_CurrentInteractible = null;

				// Position the reticle at default distance.
				if (m_Reticle)
					m_Reticle.SetPosition();
			}
		}

		// This method checks to see if the raycast position translates to player movement
		private bool isOutsideCircle(Vector3 ray, Vector3 playerPos) {
			// use x and y to determine if distance is large enought to move the ship

			Vector2 ray2d = new Vector2 (ray.x, ray.y);
			Vector2 player2d = new Vector2 (playerPos.x, playerPos.y);

			float length = Vector2.Distance (ray2d, player2d);

			if (length > turnCircleRad) {
				//Debug.Log ("Length is greater than parameter. Length: " + length);
				return true;	
			} else {
				return false;
			}
		}


		private void DeactiveLastInteractible()
		{
			if (m_LastInteractible == null)
				return;

			m_LastInteractible.Out();
			m_LastInteractible = null;
		}


		private void HandleUp()
		{
			if (m_CurrentInteractible != null)
				m_CurrentInteractible.Up();
		}


		private void HandleDown()
		{
			if (m_CurrentInteractible != null)
				m_CurrentInteractible.Down();
		}


		private void HandleClick()
		{
			if (m_CurrentInteractible != null)
				m_CurrentInteractible.Click();
		}


		private void HandleDoubleClick()
		{
			if (m_CurrentInteractible != null)
				m_CurrentInteractible.DoubleClick();

		}
	}
}