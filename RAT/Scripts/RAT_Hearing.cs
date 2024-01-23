using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RAT
{
	/// <summary>
	/// Hearing accessibility module
	/// </summary>
	public class RAT_Hearing : MonoBehaviour
	{
		private SubtitlesDisplay subtitles;

		private CaptionsDisplay captions;

		private HorizontalLayoutGroup hearingLayoutGroup;

		private Transform playerTransform;

		private GameObject hearingDisplay;

		// Start is called before the first frame update
		public void Init()
		{
			RAT_Settings.CurrentSettingsChanged += HandleNewSettings;
			RAT_Settings.HearingSettings.HearingSettingsChanged += HandleSettingsChange;

			hearingDisplay = Instantiate(RAT_Settings.HearingSettings.HearingDisplayPrefab, transform);
			hearingLayoutGroup = hearingDisplay.GetComponentInChildren<HorizontalLayoutGroup>();

			subtitles = hearingDisplay.GetComponentInChildren<SubtitlesDisplay>();
			captions = hearingDisplay.GetComponentInChildren<CaptionsDisplay>();

			if (playerTransform == null)
				playerTransform = Camera.main.transform;

			captions.SetPlayerTransform(playerTransform);
			HandleSettingsChange();
		}

		private void HandleNewSettings()
		{
			if (RAT_Settings.HearingSettings)
				RAT_Settings.HearingSettings.HearingSettingsChanged += HandleSettingsChange;
		}

		private void HandleSettingsChange()
		{
			subtitles.gameObject.SetActive(RAT_Settings.HearingSettings.SubtitlesEnabled);
			captions.gameObject.SetActive(RAT_Settings.HearingSettings.CaptionsEnabled);
			hearingLayoutGroup.childAlignment = captions.gameObject.activeSelf ? TextAnchor.LowerRight : TextAnchor.LowerCenter;
		}

		/// <summary>
		/// Set the player origin transform for directional captioning
		/// </summary>
		public void SetPlayerTransform(Transform newTrans)
		{
			playerTransform = newTrans;
			if (captions)
				captions.SetPlayerTransform(newTrans);
		}

		private void OnDestroy()
		{
			RAT_Settings.CurrentSettingsChanged -= HandleNewSettings;
			if (RAT_Settings.HearingSettings)
				RAT_Settings.HearingSettings.HearingSettingsChanged -= HandleSettingsChange;
			if (hearingDisplay != null)
				Destroy(hearingDisplay);
		}

		public void ShowSubtitle(string text, float waitLength)
		{
			if (subtitles.gameObject.activeSelf)
				subtitles.ShowSubtitle(text, waitLength);
		}

		public void ShowCaption(string captionId, float soundLength, Transform originPoint)
		{
			if (captions.gameObject.activeSelf)
				captions.CreateCaption(captionId, soundLength, originPoint);
		}

		public void ShowCaption(string captionId, float soundLength, Vector3 originPoint)
		{
			if (captions.gameObject.activeSelf)
				captions.CreateCaption(captionId, soundLength, originPoint);
		}

		public void ShowCaptionWithPriority(string captionId, float soundLength, Transform originPoint, int newPriority)
		{
			if (captions.gameObject.activeSelf)
				captions.CreateCaption(captionId, soundLength, originPoint, newPriority);
		}

		public void ShowCaptionWithPriority(string captionId, float soundLength, Vector3 originPoint, int newPriority)
		{
			if (captions.gameObject.activeSelf)
				captions.CreateCaption(captionId, soundLength, originPoint, newPriority);
		}
	}
}