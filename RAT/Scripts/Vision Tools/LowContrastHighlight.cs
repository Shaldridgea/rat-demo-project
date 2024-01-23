using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAT
{
	/// <summary>
	/// Highlighting component to facilitate the low contrast visual effect
	/// </summary>
	public class LowContrastHighlight : MonoBehaviour
	{
		private Color contrastColor = new Color(1f, 1f, 1f, 1f);

		public List<Renderer> Renderers { get; private set; } = new List<Renderer>();

		public List<int> HighlightedSubMeshes { get; private set; } = new List<int>();

		private MaterialPropertyBlock propBlock;

		private bool initCalled;

		private RAT_Vision visionController;

		public bool IsColorSet { get; private set; }

		private void Start()
		{
			HighlightInit();
			enabled = RAT_Settings.VisionSettings.LowContrastEnabled;
		}

		private void HighlightInit()
		{
			if (propBlock == null)
				propBlock = new MaterialPropertyBlock();

			// Gather all the renderer and submesh components the effect needs to know about
			Renderers.Clear();
			HighlightedSubMeshes.Clear();
			Renderers.AddRange(GetComponentsInChildren<Renderer>());
			for (int i = 0; i < Renderers.Count; ++i)
			{
				MeshFilter filter = Renderers[i].GetComponent<MeshFilter>();
				SkinnedMeshRenderer skMeshRend = Renderers[i].GetComponent<SkinnedMeshRenderer>();
				Mesh sharedMesh = null;
				if (filter)
					sharedMesh = filter.sharedMesh;
				if (skMeshRend)
					sharedMesh = skMeshRend.sharedMesh;
				HighlightedSubMeshes.Add(sharedMesh == null ? 1 : sharedMesh.subMeshCount);
			}
			initCalled = true;
		}

		private void OnEnable()
		{
			visionController = RAT_Controller.Instance.Vision;
			visionController.RegisterHighlight(this);
		}

		private void OnDisable()
		{
			for (int i = 0; i < Renderers.Count; ++i)
				Renderers[i].SetPropertyBlock(null);
		}

		private void OnTransformChildrenChanged()
		{
			HighlightInit();
		}

		private void OnDestroy()
		{
			if (visionController)
				visionController.DeregisterHighlight(this);
		}

		public void ApplyContrastColor()
		{
			if (!initCalled)
				HighlightInit();

			// Set renderers to use the highlighted colour
			for (int i = 0; i < Renderers.Count; ++i)
			{
				Renderers[i].GetPropertyBlock(propBlock);
				propBlock.SetColor("_Color", contrastColor);
				Renderers[i].SetPropertyBlock(propBlock);
			}
		}

		public void SetContrastColor(Color newColor)
		{
			contrastColor = newColor;
			IsColorSet = true;
		}
	}
}