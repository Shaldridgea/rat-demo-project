using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace RAT
{
	/// <summary>
	/// Camera image effect rendering scene with low contrast and edge detection, along with highlighting
	/// </summary>
	public class LowContrastEffect : MonoBehaviour
	{
		private Camera sourceCamera;

		private Camera lowContrastCamera;

		private CommandBuffer commandBuffer;

		private Material unlitMaterial;

		private RenderTexture rendTexture;

		private List<LowContrastHighlight> lowContrastHighlights;

		void Start()
		{
			// Unlit material for drawing highlighted meshes
			unlitMaterial = new Material(RAT_Settings.VisionSettings.UnlitColorShader);
			unlitMaterial.SetColor("_Color", Color.black);

			sourceCamera = RAT_Controller.Instance.Vision.EffectsCamera;

			// Creating and setting up camera for low contrast effect
			if (lowContrastCamera == null)
			{
				GameObject cameraGameObject = new GameObject("Low Contrast Camera");
				cameraGameObject.transform.parent = sourceCamera.transform;
				lowContrastCamera = cameraGameObject.AddComponent<Camera>();
				lowContrastCamera.enabled = false;
				lowContrastCamera.CopyFrom(sourceCamera);
				lowContrastCamera.clearFlags = CameraClearFlags.SolidColor;
				lowContrastCamera.backgroundColor = new Color(1f, 1f, 1f, 1f);
			}
			sourceCamera.depthTextureMode = DepthTextureMode.DepthNormals;
			rendTexture = new RenderTexture(sourceCamera.pixelWidth, sourceCamera.pixelHeight, 16, RenderTextureFormat.Default);
			commandBuffer = new CommandBuffer();
			lowContrastCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, commandBuffer);
		}

		private void OnPreCull()
		{
			// Keeping texture sizes consistent if they change
			if (rendTexture.width != sourceCamera.pixelWidth || rendTexture.height != sourceCamera.pixelHeight)
			{
				rendTexture.Release();
				rendTexture.width = sourceCamera.pixelWidth;
				rendTexture.height = sourceCamera.pixelHeight;
			}

			lowContrastCamera.targetTexture = rendTexture;
			commandBuffer.SetRenderTarget(rendTexture);

			commandBuffer.Clear();
			LayerMask l = sourceCamera.cullingMask;

			// Drawing highlighted meshes in black
			for (int i = 0; i < lowContrastHighlights.Count; ++i)
			{
				if (l != (l | 1 << lowContrastHighlights[i].gameObject.layer))
					continue;

				List<Renderer> currentRends = lowContrastHighlights[i].Renderers;
				for (int j = 0; j < currentRends.Count; ++j)
				{
					currentRends[j].SetPropertyBlock(null);
					for (int m = 0; m < lowContrastHighlights[i].HighlightedSubMeshes[j]; ++m)
						commandBuffer.DrawRenderer(currentRends[j], unlitMaterial, m, 0);
				}
			}
			lowContrastCamera.RenderWithShader(RAT_Settings.VisionSettings.ForceWhiteShader, null);

			// Setting highlighted meshes to show their colour
			for (int i = 0; i < lowContrastHighlights.Count; ++i)
				lowContrastHighlights[i].ApplyContrastColor();
		}

		[ImageEffectOpaque]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Material edgeDetect = RAT_Settings.VisionSettings.EdgeDetectMaterial;
			edgeDetect.SetTexture("_StencilTexture", rendTexture);

			Graphics.Blit(source, destination, edgeDetect);
		}

		private void OnDestroy()
		{
			DestroyImmediate(unlitMaterial);
			rendTexture.Release();
			if (lowContrastCamera)
				Destroy(lowContrastCamera.gameObject);
		}

		public void SetHighlightsList(List<LowContrastHighlight> hilights) => lowContrastHighlights = hilights;
	}
}