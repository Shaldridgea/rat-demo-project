using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAT
{
	/// <summary>
	/// Hearing settings data resource
	/// </summary>
	public class RAT_HearingSettings : ScriptableObject
	{
		#region Properties
		[SerializeField]
		private GameObject hearingDisplayPrefab;

		public GameObject HearingDisplayPrefab => hearingDisplayPrefab;

		[Header("Subtitles")]
        [SerializeField]
        [Tooltip("Toggle whether to display subtitles")]
        private bool subtitlesEnabled;

        public bool SubtitlesEnabled
        {
            get
            {
                return subtitlesEnabled;
            }

            set
            {
                subtitlesEnabled = value;
                HearingSettingsChanged?.Invoke();
            }
        }

        [SerializeField]
		[Tooltip("Toggle subtitle boxes scrolling text and fading out boxes")]
		private bool animateSubtitles;

		public bool AnimateSubtitles
		{
			get {
				return animateSubtitles;
			}

			set {
				animateSubtitles = value;
				HearingSettingsChanged?.Invoke();
			}
		}

		[SerializeField]
		[Tooltip("The default colour displayed when no speaker is identified")]
		private Color defaultColor = Color.white;

		[Header("Subtitle speakers")]
		[SerializeField]
		[Tooltip("The character that separates speaker names from their dialogue when displayed in subtitles")]
		private string nameDelimiter;

		public string NameDelimiter => nameDelimiter;

		[SerializeField]
		[Tooltip("If a speaker name is in the subtitle text, tick this to strip the speaker and only show the dialogue, or keep the speaker name in the text")]
		private bool stripSpeakerFromText;

		public bool StripSpeakerFromText => stripSpeakerFromText;

		[System.Serializable]
		private class SpeakerColor
		{
			public string Name;
			public Color Color = Color.white;
		}

		[SerializeField]
		[Tooltip("Identifying names and colours for different speakers' text")]
		private List<SpeakerColor> speakerIdentifiers;

		public bool UseSpeakers => speakerIdentifiers.Count > 0;

		private Dictionary<string, Color> speakerColorsDict;

		[Header("Captions")]
		[SerializeField]
		[Tooltip("Toggle the use of directional captions")]
		private bool captionsEnabled;

		public bool CaptionsEnabled
		{
			get {
				return captionsEnabled;
			}

			set {
				captionsEnabled = value;
				HearingSettingsChanged?.Invoke();
			}
		}

		[System.Serializable]
		public class CaptionInfo
		{
			[Tooltip("Name id used to call caption")]
			public string CaptionId;
			public string Caption;
			[Tooltip("Priority of caption display. Higher number is higher priority")]
			public int Priority;
		}

		[SerializeField]
		[Tooltip("List of captions that can be displayed")]
		private List<CaptionInfo> captionList;

		private Dictionary<string, CaptionInfo> captionDict;
		#endregion

		public event RAT_Settings.SettingsValueChanged HearingSettingsChanged;

		void OnEnable()
		{
			if (hearingDisplayPrefab == null)
				hearingDisplayPrefab = Resources.Load<GameObject>("Hearing Display");
		}

		/// <summary>
		/// Get caption info by string id
		/// </summary>
		public CaptionInfo GetCaptionInfo(string id)
		{
			captionDict.TryGetValue(id, out CaptionInfo caption);
			return caption;
		}

		/// <summary>
		/// Get speaker colour by name
		/// </summary>
		public Color GetSpeakerColor(string nameId)
		{
			if (speakerColorsDict.TryGetValue(nameId, out Color value))
				return value;
			else
				return defaultColor;
		}

		public void ClearEvents()
		{
			HearingSettingsChanged = null;
		}

		public void TriggerChangedEvent()
		{
			HearingSettingsChanged?.Invoke();
		}

		private void OnValidate()
		{
			if (captionDict == null)
				captionDict = new Dictionary<string, CaptionInfo>();

			if (captionList.Count != captionDict.Count)
			{
				captionDict.Clear();
				for (int i = 0; i < captionList.Count; ++i)
					if (!captionDict.ContainsKey(captionList[i].CaptionId))
						captionDict.Add(captionList[i].CaptionId, captionList[i]);
			}

			if (speakerColorsDict == null)
				speakerColorsDict = new Dictionary<string, Color>();

			if (speakerIdentifiers.Count != speakerColorsDict.Count)
			{
				speakerColorsDict.Clear();
				for (int i = 0; i < speakerIdentifiers.Count; ++i)
					if (!speakerColorsDict.ContainsKey(speakerIdentifiers[i].Name))
						speakerColorsDict.Add(speakerIdentifiers[i].Name, speakerIdentifiers[i].Color);
			}
		}
	}
}