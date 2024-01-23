using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RAT
{
	/// <summary>
	/// Directional captioning display controller
	/// </summary>
	public class CaptionsDisplay : MonoBehaviour
	{
		[SerializeField]
		private VerticalLayoutGroup captionsLayoutGroup;

		[SerializeField]
		private DirectionalCaption captionPrefab;

		[SerializeField]
		private int captionLimit;

		private List<DirectionalCaption> captions = new List<DirectionalCaption>();

		private Transform playerTransform;

		// Create caption pointing to transform that can move
		public void CreateCaption(string captionId, float soundLength, Transform pointToOrigin, int overridePriority = 0)
		{
			RAT_HearingSettings.CaptionInfo caption = RAT_Settings.HearingSettings.GetCaptionInfo(captionId);
			if (caption == null)
				return;

			int captionPriority = overridePriority == 0 ? caption.Priority : overridePriority;
			DirectionalCaption newCaption = Instantiate(captionPrefab, transform);

			newCaption.MakeCaption(caption.Caption, captionPriority, soundLength,
				RAT_Controller.Instance.RAT_Camera.transform, playerTransform, pointToOrigin);

			SetCaptionOrder(newCaption);
		}

		// Create caption pointing to static position
		public void CreateCaption(string captionId, float soundLength, Vector3 pointToOrigin, int overridePriority = 0)
		{
			RAT_HearingSettings.CaptionInfo caption = RAT_Settings.HearingSettings.GetCaptionInfo(captionId);
			if (caption == null)
				return;

			int captionPriority = overridePriority == 0 ? caption.Priority : overridePriority;
			DirectionalCaption newCaption = Instantiate(captionPrefab, transform);

			newCaption.MakeCaption(caption.Caption, captionPriority, soundLength,
				RAT_Controller.Instance.RAT_Camera.transform, playerTransform, pointToOrigin);

			SetCaptionOrder(newCaption);
		}

		public void SetPlayerTransform(Transform newTrans) => playerTransform = newTrans;

		private void OnTransformChildrenChanged()
		{
			for (int i = captions.Count - 1; i >= 0; --i)
				if (captions[i] == null)
					captions.RemoveAt(i);
		}

		private void SetCaptionOrder(DirectionalCaption caption)
		{
			bool added = false;
			// Adjust ordering of captions based on priority
			// Higher priority inserts captions lower down in the corner
			// where the player would be looking for new captions
			for (int i = 0; i < captions.Count; ++i)
				if (captions[i].Priority > caption.Priority)
				{
					added = true;
					captions.Insert(i, caption);
					caption.transform.SetSiblingIndex(i);
					break;
				}

			if (!added)
				captions.Add(caption);

			if (captions.Count > captionLimit)
			{
				Destroy(captions[captionLimit].gameObject);
				captions.RemoveAt(captionLimit);
			}
		}
	}
}