using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

namespace RAT
{
	/// <summary>
	/// Vision accessibility module
	/// </summary>
	public class RAT_Vision : MonoBehaviour
	{
		private List<LowContrastHighlight> highlights = new List<LowContrastHighlight>();

		private List<Outline> outlines = new List<Outline>();

		private List<Color> outlineColors;

		private LowContrastEffect lowContrastEffect;

		private OutlineEffect outlineEffect;

		private ColorblindCorrectionEffect colorblindEffect;

		public Camera EffectsCamera { get; private set; }

		public void Init()
		{
			RAT_Settings.CurrentSettingsChanged += HandleNewSettings;
			RAT_Settings.VisionSettings.VisionSettingsChanged += HandleSettingsChange;
		}

		private void HandleNewSettings()
		{
			RAT_Settings.CurrentSettingsChanged += HandleNewSettings;
			if (RAT_Settings.VisionSettings)
				RAT_Settings.VisionSettings.VisionSettingsChanged += HandleSettingsChange;
		}

		private void HandleSettingsChange()
		{
			if (RAT_Settings.VisionSettings.LowContrastEnabled)
				EnableLowContrastEffect();
			else
				DisableLowContrastEffect();

			if (RAT_Settings.VisionSettings.OutlinesEnabled)
				EnableOutlinesEffect();
			else
				DisableOutlinesEffect();

			if (RAT_Settings.VisionSettings.ColorblindEnabled)
				EnableColorblindEffect();
			else
				DisableColorblindEffect();
		}

		public void EnableColorblindEffect()
		{
			if (EffectsCamera == null)
				EffectsCamera = RAT_Controller.Instance.RAT_Camera;

			if (colorblindEffect == null)
				CreateCameraEffects();

			colorblindEffect.enabled = true;
		}

		public void DisableColorblindEffect()
		{
			if (colorblindEffect)
				colorblindEffect.enabled = false;
		}

		public void EnableLowContrastEffect()
		{
			if (EffectsCamera == null)
				EffectsCamera = RAT_Controller.Instance.RAT_Camera;

			if (lowContrastEffect == null)
				CreateCameraEffects();

			// Enable existing highlights, and create new ones if necessary
			if (RAT_Settings.VisionSettings.AutoEnableHighlights)
				RAT_Settings.VisionSettings.CreateAllHighlights();
			else
				for (int i = 0; i < highlights.Count; ++i)
				{
					highlights[i].enabled = true;
					highlights[i].ApplyContrastColor();
				}
			lowContrastEffect.enabled = true;
		}

		public void DisableLowContrastEffect()
		{
			for (int i = 0; i < highlights.Count; ++i)
				highlights[i].enabled = false;
			if (lowContrastEffect)
				lowContrastEffect.enabled = false;
		}

		/// <summary>
		/// Register a highlight component for use with the low contrast effect
		/// </summary>
		/// <param name="newHighlight"></param>
		public void RegisterHighlight(LowContrastHighlight newHighlight)
		{
			highlights.Add(newHighlight);
			if (newHighlight.IsColorSet)
				return;

			Color c = RAT_Settings.VisionSettings.GetHighlightColor(newHighlight.gameObject);
			newHighlight.SetContrastColor(c);
		}

		public void DeregisterHighlight(LowContrastHighlight oldHighlight)
		{
			highlights.Remove(oldHighlight);
		}

		public void EnableOutlinesEffect()
		{
			if (EffectsCamera == null)
				EffectsCamera = RAT_Controller.Instance.RAT_Camera;

			if (outlineEffect == null)
				CreateCameraEffects();

			// Enable existing outlines, and create new ones if necessary
			if (RAT_Settings.VisionSettings.AutoEnableOutlines)
				RAT_Settings.VisionSettings.CreateAllOutlines();
			else
				for (int i = 0; i < outlines.Count; ++i)
					outlines[i].enabled = true;
			outlineEffect.enabled = true;
		}

		private void ApplyOutlineSettings()
		{
			outlineEffect = EffectsCamera.gameObject.AddComponent<OutlineEffect>();
			outlineEffect.fillAmount = 0f;
			outlineEffect.lineIntensity = 1f;
			outlineEffect.cornerOutlines = true;
			outlineEffect.sourceCamera = EffectsCamera;
			outlineEffect.enableDynamicOutlines = RAT_Settings.VisionSettings.DynamicOutlinesEnabled;
			outlineColors = RAT_Settings.VisionSettings.GetOutlineColors();
			outlineEffect.outlineColors = outlineColors;
			outlineEffect.SetOutlines(outlines);
		}

		public void DisableOutlinesEffect()
		{
			for (int i = 0; i < outlines.Count; ++i)
				outlines[i].enabled = false;
			if (outlineEffect)
				outlineEffect.enabled = false;
		}

		public void AddOutlineColor(Color newColor) => outlineColors.Add(newColor);

		public void RemoveOutlineColor(Color oldColor) => outlineColors.Remove(oldColor);

		public void RegisterOutline(Outline newHighlight)
		{
			outlines.Add(newHighlight);
			newHighlight.color = RAT_Settings.VisionSettings.GetOutlineColorId(newHighlight.gameObject);
		}

		public void DeregisterOutline(Outline oldHighlight)
		{
			outlines.Remove(oldHighlight);
		}

		public void SetEffectsCamera(Camera newCamera)
		{
			if (newCamera == null)
				return;

			EffectsCamera = newCamera;
		}

		private void OnDestroy()
		{
			if (lowContrastEffect)
				Destroy(lowContrastEffect);

			if (outlineEffect)
				Destroy(outlineEffect);

			if (colorblindEffect)
				Destroy(colorblindEffect);
		}

		/// <summary>
		/// Create and apply enabled visual effects on the camera
		/// </summary>
		private void CreateCameraEffects()
		{
			// Rendering of these effects depends on the component order, 
			// so we have to evaluate what is being used and enable
			// everything in the right order so it works together

			// Effects are rendered in order of:
			// low contrast first, then outlines, then colourblind correction
			if (RAT_Settings.VisionSettings.LowContrastEnabled)
			{
				bool remakeOutline = outlineEffect != null || RAT_Settings.VisionSettings.OutlinesEnabled;
				if (outlineEffect)
					Destroy(outlineEffect);

				bool remakeColorblind = colorblindEffect != null || RAT_Settings.VisionSettings.ColorblindEnabled;
				if (colorblindEffect)
					Destroy(colorblindEffect);

				lowContrastEffect = EffectsCamera.gameObject.AddComponent<LowContrastEffect>();
				lowContrastEffect.SetHighlightsList(highlights);

				if (remakeOutline)
				{
					ApplyOutlineSettings();
					outlineEffect.enabled = RAT_Settings.VisionSettings.OutlinesEnabled;
				}

				if (remakeColorblind)
				{
					colorblindEffect = EffectsCamera.gameObject.AddComponent<ColorblindCorrectionEffect>();
					colorblindEffect.enabled = RAT_Settings.VisionSettings.ColorblindEnabled;
				}
			}
			else if (RAT_Settings.VisionSettings.OutlinesEnabled)
			{
				bool remakeColorblind = colorblindEffect != null || RAT_Settings.VisionSettings.ColorblindEnabled;
				if (colorblindEffect)
					Destroy(colorblindEffect);

				ApplyOutlineSettings();

				if (remakeColorblind)
				{
					colorblindEffect = EffectsCamera.gameObject.AddComponent<ColorblindCorrectionEffect>();
					colorblindEffect.enabled = RAT_Settings.VisionSettings.ColorblindEnabled;
				}
			}
			else if (RAT_Settings.VisionSettings.ColorblindEnabled)
				colorblindEffect = EffectsCamera.gameObject.AddComponent<ColorblindCorrectionEffect>();
		}
	}
}