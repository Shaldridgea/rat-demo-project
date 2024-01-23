using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RAT
{
	/// <summary>
	/// Subtitles display controller
	/// </summary>
	public class SubtitlesDisplay : MonoBehaviour
	{
		[SerializeField]
		private TypewriterEffect[] textEffects;

		[SerializeField]
		private CanvasGroup[] canvasGroups;

		[SerializeField]
		private Image[] textBackgrounds;

		[SerializeField]
		[Range(0f, 1f)]
		private float textBackgroundAlpha;

		[SerializeField]
		[Min(0f)]
		private float textScrollingSpeed;

		private int subIndex;

		private Coroutine stopSubtitleCoroutine;

		private void Start()
		{
			for (int i = 0; i < textBackgrounds.Length; ++i)
				textBackgrounds[i].gameObject.SetActive(false);
			for (int i = 0; i < canvasGroups.Length; ++i)
				canvasGroups[i].alpha = 0f;
		}

		/// <summary>
		/// Shows a subtitle text for the passed waitLength in seconds
		/// </summary>
		public void ShowSubtitle(string text, float waitLength)
		{
			if (stopSubtitleCoroutine != null)
				StopCoroutine(stopSubtitleCoroutine);

            // Animate subtitle boxes
            if (RAT_Settings.HearingSettings.AnimateSubtitles)
            {
                bool subExists = false;
                LeanTween.cancel(canvasGroups[subIndex].gameObject);

                // Animate the just used box upwards
                if (canvasGroups[subIndex].alpha > 0f)
                {
                    LeanTween.value(textBackgrounds[subIndex].gameObject,
                        textBackgrounds[subIndex].rectTransform.anchoredPosition,
                        textBackgrounds[subIndex].rectTransform.anchoredPosition
                        + Vector2.up * textBackgrounds[subIndex].rectTransform.rect.height, 0.5f).setOnUpdateVector2(SetAnchorPos);
                    subExists = true;
                }
                // Fade just used box out
                AnimateSubtitleEnd(subIndex);
                // Cycle targeted box to next available box
                subIndex = ++subIndex % textBackgrounds.Length;

                // Cancel animations on new box
                LeanTween.cancel(textBackgrounds[subIndex].gameObject);
                // Fade in new box if it's not fully visible
                if (canvasGroups[subIndex].alpha != 1f && !subExists)
                    LeanTween.alphaCanvas(canvasGroups[subIndex], 1f, 0.5f);
                else
                    canvasGroups[subIndex].alpha = 1f;

                // Reset position since it may have been animated upwards before
                textBackgrounds[subIndex].rectTransform.anchoredPosition = Vector2.zero;

                for (int i = 0; i < textEffects.Length; ++i)
                    textEffects[i].SetTextDisplaySpeed(textScrollingSpeed);
            }
            else
            {
                canvasGroups[subIndex].alpha = 1f;
                for (int i = 0; i < textEffects.Length; ++i)
                    textEffects[i].SetTextDisplaySpeed(0f);
            }
			textBackgrounds[subIndex].rectTransform.SetAsLastSibling();
			textBackgrounds[subIndex].gameObject.SetActive(true);

			// Split subtitle text if there's a speaker name within
			if (RAT_Settings.HearingSettings.UseSpeakers)
			{
				string searchName = "";
				if (text.Contains(RAT_Settings.HearingSettings.NameDelimiter))
				{
					string[] split =
						text.Split(new string[] { RAT_Settings.HearingSettings.NameDelimiter }, 2,
						System.StringSplitOptions.RemoveEmptyEntries);
					if (split.Length > 1)
					{
						searchName = split[0];
						if (RAT_Settings.HearingSettings.StripSpeakerFromText)
							text = split[1].Trim();
					}
				}
				textEffects[subIndex].SourceText.color = RAT_Settings.HearingSettings.GetSpeakerColor(searchName);
			}
			textEffects[subIndex].StartText(text);
			stopSubtitleCoroutine = StartCoroutine(WaitToStopSubtitle(waitLength));
		}

		/// <summary>
		/// Make the box the passed index points disappear
		/// </summary>
		private void AnimateSubtitleEnd(int index)
		{
			// Fade a box to transparent
			if (RAT_Settings.HearingSettings.AnimateSubtitles)
				LeanTween.alphaCanvas(canvasGroups[index], 0f, 0.3f);
			else
				textBackgrounds[index].gameObject.SetActive(false);
		}

		private void SetAnchorPos(Vector2 pos)
		{
			textBackgrounds[(subIndex + 1) % textBackgrounds.Length].rectTransform.anchoredPosition = pos;
		}

		private void OnValidate()
		{
			for (int i = 0; i < textEffects.Length; ++i)
				textEffects[i].SetTextDisplaySpeed(textScrollingSpeed);

			for (int i = 0; i < textBackgrounds.Length; ++i)
			{
				Color c = textBackgrounds[i].color;
				c.a = textBackgroundAlpha;
				textBackgrounds[i].color = c;
			}
		}

		private void OnDisable()
		{
			for (int i = 0; i < textBackgrounds.Length; ++i)
				textBackgrounds[i].gameObject.SetActive(false);

			for (int i = 0; i < canvasGroups.Length; ++i)
				canvasGroups[i].alpha = 0f;

			for (int i = 0; i < textEffects.Length; ++i)
				textEffects[i].StartText("");
		}

		/// <summary>
		/// Wait the passed number of seconds before fading out the subtitle box
		/// </summary>
		private IEnumerator WaitToStopSubtitle(float waitLength)
		{
			int index = subIndex;
			yield return new WaitForSeconds(waitLength);
			AnimateSubtitleEnd(index);
		}
	}
}