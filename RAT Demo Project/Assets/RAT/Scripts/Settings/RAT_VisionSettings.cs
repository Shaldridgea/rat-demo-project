using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

namespace RAT
{
	/// <summary>
	/// Vision settings data resource
	/// </summary>
	public class RAT_VisionSettings : ScriptableObject
	{
		public enum ColorblindCorrection
		{
			Protanopia,
			Deuteranopia,
			Tritanopia
		}

		[System.Serializable]
		private class IdentifyColorData
		{
			public string Name;
			public string Tag;
			public LayerMask Layer;
			public Color Color = Color.white;
		}

		#region Properties
		[SerializeField]
		[HideInInspector]
		private Shader unlitColorShader;

		public Shader UnlitColorShader => unlitColorShader;

		[SerializeField]
		[HideInInspector]
		private Shader forceWhiteShader;

		public Shader ForceWhiteShader => forceWhiteShader;

		[SerializeField]
		[HideInInspector]
		private Material edgeDetectMaterial;

		public Material EdgeDetectMaterial => edgeDetectMaterial;

		[SerializeField]
		[HideInInspector]
		private Material colorblindCorrectionMaterial;

		public Material ColorblindCorrectionMaterial => colorblindCorrectionMaterial;


		[Header("Low Contrast")]
		[SerializeField]
		[Tooltip("Toggle low contrast mode, which displays identified targets in blocks of colour for visual clarity")]
		private bool lowContrastEnabled;

		public bool LowContrastEnabled
		{
			get {
				return lowContrastEnabled;
			}

			set {
				lowContrastEnabled = value;
				VisionSettingsChanged?.Invoke();
			}
		}

		[SerializeField]
		[Tooltip("Toggle whether to highlight all matching identifiers without manually adding highlighting component")]
		private bool autoEnableHighlights;

		public bool AutoEnableHighlights
		{
			get {
				return autoEnableHighlights;
			}

			set {
				autoEnableHighlights = value;
				VisionSettingsChanged?.Invoke();
			}
		}

		[SerializeField]
		[Tooltip("Identifying data for objects to match and highlight")]
		private List<IdentifyColorData> lowContrastIdentifiers;

		[Header("Outlines")]
		[SerializeField]
		[Tooltip("Toggle outline mode, which creates coloured outlines around identified targets")]
		private bool outlinesEnabled;

		public bool OutlinesEnabled
		{
			get {
				return outlinesEnabled;
			}

			set {
				outlinesEnabled = value;
				VisionSettingsChanged?.Invoke();
			}
		}

		[SerializeField]
		[Tooltip("Toggle whether new outline colours can be defined at runtime (has a performance cost)")]
		private bool dynamicOutlinesEnabled;

		public bool DynamicOutlinesEnabled
		{
			get {
				return dynamicOutlinesEnabled;
			}

			set {
				dynamicOutlinesEnabled = value;
				VisionSettingsChanged?.Invoke();
			}
		}

		[SerializeField]
		[Tooltip("Toggle whether to outline matching identifiers without manually adding an outline component")]
		private bool autoEnableOutlines;

		public bool AutoEnableOutlines
		{
			get {
				return autoEnableOutlines;
			}

			set {
				autoEnableOutlines = value;
				VisionSettingsChanged?.Invoke();
			}
		}

		[SerializeField]
		[Tooltip("Identifying data for objects to match and outline")]
		private List<IdentifyColorData> outlineIdentifiers;

		[SerializeField]
		[HideInInspector]
		private int dynamicOutlineCount;


		[Header("Colorblind Correction")]
		[SerializeField]
		[Tooltip("Toggle colorblind correction mode")]
		private bool colorblindEnabled;

		public bool ColorblindEnabled
		{
			get {
				return colorblindEnabled;
			}

			set {
				colorblindEnabled = value;
				VisionSettingsChanged?.Invoke();
			}
		}

		[SerializeField]
		[Tooltip("Change which type of colourblindness the correction filter is targeting")]
		private ColorblindCorrection colorblindType;

		public ColorblindCorrection ColorblindType
		{
			get {
				return colorblindType;
			}

			set {
				colorblindType = value;
				VisionSettingsChanged?.Invoke();
			}
		}
		#endregion

		public event RAT_Settings.SettingsValueChanged VisionSettingsChanged;

		private void OnEnable()
		{
			if (unlitColorShader == null)
				unlitColorShader = Shader.Find("Unlit/Color");
			if (forceWhiteShader == null)
				forceWhiteShader = Resources.Load<Shader>("UnlitForceWhiteShader");
			if (edgeDetectMaterial == null)
				edgeDetectMaterial = Resources.Load<Material>("EdgeDetectMaterial");
			if (colorblindCorrectionMaterial == null)
				colorblindCorrectionMaterial = Resources.Load<Material>("ColorBlindCorrectionMat");

			// Remove any outlines that were added at runtime
			if (dynamicOutlineCount > 0)
				for (int i = 0; i < dynamicOutlineCount; ++i)
					outlineIdentifiers.RemoveAt(outlineIdentifiers.Count - 1);
			dynamicOutlineCount = 0;
		}

		/// <summary>
		/// Check whether a GameObject matches with a specific identifier colour
		/// </summary>
		private bool DoesMatchIdentifier(IdentifyColorData id, GameObject check)
		{
			if (id.Layer != (id.Layer | 1 << check.layer))
				return false;

			return check.CompareTag(id.Tag);
		}

		#region Low contrast & highlights
		/// <summary>
		/// Get the highlight colour this GameObject should be using (transparent if none)
		/// </summary>
		public Color GetHighlightColor(GameObject check)
		{
			Color returnColor = Color.clear;
			for (int i = 0; i < lowContrastIdentifiers.Count; ++i)
			{
				IdentifyColorData c = lowContrastIdentifiers[i];

				if (!DoesMatchIdentifier(c, check))
					continue;

				returnColor = c.Color;
				break;
			}
			return returnColor;
		}

		/// <summary>
		/// Get all existing highlights and add highlights to matching objects in scene
		/// </summary>
		public void CreateAllHighlights()
		{
			for (int i = 0; i < lowContrastIdentifiers.Count; ++i)
			{
				IdentifyColorData c = lowContrastIdentifiers[i];
				GameObject[] candidates = GameObject.FindGameObjectsWithTag(c.Tag);
				for (int j = 0; j < candidates.Length; ++j)
				{
					if (c.Layer != (c.Layer | 1 << candidates[j].layer))
						continue;

					LowContrastHighlight exists = candidates[j].GetComponent<LowContrastHighlight>();
					if (exists)
					{
						exists.enabled = true;
						exists.ApplyContrastColor();
						continue;
					}

					LowContrastHighlight h = candidates[j].AddComponent<LowContrastHighlight>();
					h.SetContrastColor(c.Color);
					h.ApplyContrastColor();
				}
			}
		}

		public void EnableLowContrastMode()
		{
			lowContrastEnabled = true;
			RAT_Controller.Instance.Vision.EnableLowContrastEffect();
		}

		public void DisableLowContrastMode()
		{
			lowContrastEnabled = false;
			RAT_Controller.Instance.Vision.DisableLowContrastEffect();
		}

		public void ToggleLowContrastMode()
		{
			if (lowContrastEnabled)
				DisableLowContrastMode();
			else
				EnableLowContrastMode();
		}
		#endregion

		#region Outlines
		/// <summary>
		/// Get all existing outlines and add outlines to matching objects in scene
		/// </summary>
		public void CreateAllOutlines()
		{
			for (int i = 0; i < outlineIdentifiers.Count; ++i)
			{
				IdentifyColorData c = outlineIdentifiers[i];
				GameObject[] candidates = GameObject.FindGameObjectsWithTag(c.Tag);
				for (int j = 0; j < candidates.Length; ++j)
				{
					if (c.Layer != (c.Layer | 1 << candidates[j].layer))
						continue;

					Outline exists = candidates[j].GetComponent<Outline>();
					if (exists)
					{
						exists.enabled = true;
						continue;
					}

					Outline h = candidates[j].AddComponent<Outline>();
				}
			}
		}

		public void EnableOutlines()
		{
			outlinesEnabled = true;
			RAT_Controller.Instance.Vision.EnableOutlinesEffect();
		}

		public void DisableOutlines()
		{
			outlinesEnabled = false;
			RAT_Controller.Instance.Vision.DisableOutlinesEffect();
		}

		public void ToggleOutlines()
		{
			if (outlinesEnabled)
				DisableOutlines();
			else
				EnableOutlines();
		}

		/// <summary>
		/// Get list of only colours from outline identifiers
		/// </summary>
		public List<Color> GetOutlineColors()
		{
			List<Color> outlineColors = new List<Color>(outlineIdentifiers.Count);
			for (int i = 0; i < outlineIdentifiers.Count; ++i)
				outlineColors.Add(outlineIdentifiers[i].Color);
			return outlineColors;
		}

		/// <summary>
		/// Get ID number of outline identifier that matches the passed GameObject
		/// </summary>
		public int GetOutlineColorId(GameObject check)
		{
			for (int i = 0; i < outlineIdentifiers.Count; ++i)
			{
				if (!DoesMatchIdentifier(outlineIdentifiers[i], check))
					continue;

				return i;
			}
			return 0;
		}

		public void AddOutlineIdentifier(string name, string newTag, LayerMask newLayer, Color newColor)
		{
			outlineIdentifiers.Add(new IdentifyColorData() { Name = name, Tag = newTag, Layer = newLayer, Color = newColor });
			RAT_Controller.Instance.Vision.AddOutlineColor(newColor);
			++dynamicOutlineCount;
		}

		public void RemoveOutlineIdentifier(string name, string newTag, LayerMask newLayer, Color newColor)
		{
			int index = outlineIdentifiers.FindIndex(i => i.Name == name);
			if (index < 0)
				return;

			RAT_Controller.Instance.Vision.RemoveOutlineColor(outlineIdentifiers[index].Color);
			outlineIdentifiers.RemoveAt(index);
			--dynamicOutlineCount;
		}
		#endregion

		#region Colorblind
		public void EnableColorblindMode()
		{
			colorblindEnabled = true;
			RAT_Controller.Instance.Vision.EnableColorblindEffect();
		}

		public void DisableColorblindMode()
		{
			colorblindEnabled = false;
			RAT_Controller.Instance.Vision.DisableColorblindEffect();
		}

		public void ToggleColorblindMode()
		{
			if (colorblindEnabled)
				DisableColorblindMode();
			else
				EnableColorblindMode();
		}
		#endregion

		public void ClearEvents()
		{
			VisionSettingsChanged = null;
		}

		public void TriggerChangedEvent()
		{
			VisionSettingsChanged?.Invoke();
		}
	}
}