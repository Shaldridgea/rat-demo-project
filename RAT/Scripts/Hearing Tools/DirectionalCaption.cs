using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace RAT
{
	/// <summary>
	/// Directional captioning text instance
	/// </summary>
	public class DirectionalCaption : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI captionText;

		[SerializeField]
		private Image directionImage;

        [SerializeField]
        private Sprite directionArrow;

        [SerializeField]
        private Sprite directionCentre;

		public int Priority { get; private set; }

		private Transform camOrigin;

		private Transform origin;

		private float timer;

		private Transform pointTransform;

		private Vector3 pointPosition;

		// Make caption pointing to transform that can move
		public void MakeCaption(string caption, int priority, float length, Transform cameraTransform, Transform originTransform, Transform pointAtTransform)
		{
			captionText.text = caption;
			timer = length * 1.1f;
			Priority = priority;
			camOrigin = cameraTransform;
			origin = originTransform;
			pointTransform = pointAtTransform;
		}

		// Make caption pointing to static position
		public void MakeCaption(string caption, int priority, float length, Transform cameraTransform, Transform originTransform, Vector3 pointAtPosition)
		{
			captionText.text = caption;
			timer = length * 1.1f;
			Priority = priority;
			camOrigin = cameraTransform;
			origin = originTransform;
			pointPosition = pointAtPosition;
		}

		private void Update()
		{
            bool isCentre = false;

			if (pointTransform != null)
				pointPosition = pointTransform.position;
			
			// Update caption origin pointing to position
			Vector3 goalPos = pointPosition;
			goalPos.y = 0f;
			Vector3 originPos = origin.position;
			originPos.y = 0f;
			Vector3 pointVector = (goalPos - originPos).normalized;

            // Check whether the point the sound was emitted from was very close to our player
            if (originPos == goalPos)
                isCentre = true;
            else if (Vector3.Distance(originPos, goalPos) <= 0.5f)
                isCentre = true;

			// Find angle from camera to sound origin and apply to caption direction image
			float relativeAngle = -Vector3.SignedAngle(camOrigin.forward, pointVector, camOrigin.up);

            // If the sound is very near our player, indicate this with the centred image, otherwise point to it
            directionImage.sprite = isCentre ? directionCentre : directionArrow;
			directionImage.transform.rotation = Quaternion.Euler(0f, 0f, relativeAngle);
			timer -= Time.deltaTime;
			if (timer <= 0f)
				Destroy(gameObject);
		}
	}
}