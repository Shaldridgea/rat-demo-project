using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAT
{
	public class ColorblindCorrectionEffect : MonoBehaviour
	{
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, destination,
				RAT_Settings.VisionSettings.ColorblindCorrectionMaterial, (int)RAT_Settings.VisionSettings.ColorblindType);
		}
	}
}