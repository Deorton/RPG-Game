using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
	public class FollowCamera : MonoBehaviour
	{
		/// <summary>
		/// Our target Transform. Do not use for setting the target at runtime. Instead use the SetTarget() method.
		/// </summary>
		[Tooltip("The script can automatically check for a target at runtime if Target Polling is used below, or you can set a target with the SetTarget() method.")]
		public Transform target;

		/// <summary>
		/// If we have no target, do we check every frame for a GameObject with the Tag "Player"?
		/// </summary>
		[Tooltip("Tick this to check continuously for a GameObject with the Tag \"Player\" if we have no target.")]
		public bool targetPolling = false;

		public float startingDistance = 10f; // Distance the camera starts from target object.
		public float maxDistance = 20f; // Max distance the camera can be from target object.
		public float minDistance = 3f; // Min distance the camera can be from target object.
		public float zoomSpeed = 20f; // The speed the camera zooms in.
		public float targetHeight = 1.0f; // The amount from the target object pivot the camera should look at.
		public float camRotationSpeed = 70;// The speed at which the camera rotates.
		public float camXAngle = 45.0f; // The camera x euler angle.
		public bool fadeObjects = false; // Enable objects of a certain layer to be faded.
		public List<int> layersToTransparent = new List<int>(); // The layers where we will allow transparency.
		public float alpha = 0.3f; // The alpha value of the material when player behind object.

		private float y = 0.0f; // The camera y euler angle.
		private Transform myTransform;
		private Transform prevHit;
		private float minCameraAngle = 0.0f; // The min angle on the camera's x axis.
		private float maxCameraAngle = 90.0f; // The max angle on the camera's x axis.

		void Start()
		{
			myTransform = transform;
			if (target != null) myTransform.position = target.position;
			Vector3 angles = myTransform.eulerAngles;
			// Set default y angle.
			y = angles.y;
		}



		/// <summary>
		/// Set the camera's Target transform.
		/// </summary>
		/// <param name="target"></param>
		public void SetTarget(Transform target)
		{
			this.target = target;
			if (target != null)
			{
				myTransform.position = target.position;
				y = myTransform.eulerAngles.y;
			}
		}



		void LateUpdate()
		{

			if (target == null)
			{
				if (targetPolling)
				{
					GameObject player = GameObject.FindGameObjectWithTag("Player");
					if (player != null) SetTarget(player.transform);
					if (target == null) return;
				}
				else
				{
					return;
				}
			}

			// Zoom Camera and keep the distance between [minDistance, maxDistance].
			float mw = Input.GetAxis("Mouse ScrollWheel");
			if (mw > 0)
			{
				startingDistance -= Time.deltaTime * zoomSpeed;
				if (startingDistance < minDistance)
					startingDistance = minDistance;
			}
			else if (mw < 0)
			{
				startingDistance += Time.deltaTime * zoomSpeed;
				if (startingDistance > maxDistance)
					startingDistance = maxDistance;
			}

			// Rotate Camera around character.
			if (Input.GetButton("Fire3"))
			{ // 0 is left, 1 is right, 2 is middle mouse button.
				float h = Input.GetAxis("Mouse X"); // The horizontal movement of the mouse.						
				float v = Input.GetAxis("Mouse Y"); // The vertical movement of the mouse.
				if (h > 0 && h > Math.Abs(v))
				{
					myTransform.RotateAround(target.transform.position, new Vector3(0, 1, 0), camRotationSpeed * Time.deltaTime);
					y = myTransform.eulerAngles.y;
				}
				else if (h < 0 && h < -Math.Abs(v))
				{
					myTransform.RotateAround(target.transform.position, new Vector3(0, 1, 0), -camRotationSpeed * Time.deltaTime);
					y = myTransform.eulerAngles.y;
				}
				else if (v > 0 && v > Math.Abs(h))
				{
					camXAngle += camRotationSpeed * Time.deltaTime;
					if (camXAngle > maxCameraAngle)
					{
						camXAngle = maxCameraAngle;
					}
				}
				else if (v < 0 && v < -Math.Abs(h))
				{
					camXAngle += -camRotationSpeed * Time.deltaTime;
					if (camXAngle < minCameraAngle)
					{
						camXAngle = minCameraAngle;
					}
				}
			}

			// Set camera angles.
			Quaternion rotation = Quaternion.Euler(camXAngle, y, 0);
			myTransform.rotation = rotation;

			// Position Camera.
			Vector3 trm = rotation * Vector3.forward * startingDistance + new Vector3(0, -1 * targetHeight, 0);
			Vector3 position = target.position - trm;
			myTransform.position = position;

			//Start checking if object between camera and target.
			if (fadeObjects)
			{
				// Cast ray from camera.position to target.position and check if the specified layers are between them.
				Ray ray = new Ray(myTransform.position, (target.position - myTransform.position).normalized);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, maxDistance))
				{
					Transform objectHit = hit.transform;
					if (layersToTransparent.Contains(objectHit.gameObject.layer))
					{
						if (prevHit != null)
						{
							prevHit.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
						}
						if (objectHit.GetComponent<Renderer>() != null)
						{
							prevHit = objectHit;
							// Can only apply alpha if this material shader is transparent.
							prevHit.GetComponent<Renderer>().material.color = new Color(1, 1, 1, alpha);
						}
					}
					else if (prevHit != null)
					{
						prevHit.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
						prevHit = null;
					}
				}
			}
		}
	}
}

