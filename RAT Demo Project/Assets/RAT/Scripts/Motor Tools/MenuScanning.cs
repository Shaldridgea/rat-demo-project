using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RAT
{
	/// <summary>
	/// Menu scanning behaviour component
	/// </summary>
	public class MenuScanning : MonoBehaviour
	{
		private Selectable startSelectable;

		private Selectable currentSelectable;

		private float timer;

		public void SetStartSelectable(Selectable newSelectable)
		{
			startSelectable = newSelectable;
			currentSelectable = startSelectable;
			if (newSelectable)
				newSelectable.Select();
			timer = 0f;
		}

		public void ResetTimer() => timer = 0f;

		private void Update()
		{
			if (startSelectable == null)
				return;

            // Don't move on to the next UI element if the player is making inputs
            if (Input.anyKey)
                ResetTimer();

			timer += Time.unscaledDeltaTime;

			// Move to next selectable after wait time has elapsed
			if (timer >= RAT_Settings.MotorSettings.WaitInterval)
			{
				timer = 0f;
				Selectable nextSelectable = null;
				int iteration = 0;
				// Search for a nearby selectable to transition to
				while (nextSelectable == null)
				{
					switch (iteration)
					{
						case 0:
						nextSelectable = currentSelectable.FindSelectableOnRight();
						break;

						case 1:
						nextSelectable = currentSelectable.FindSelectableOnDown();
						break;

						case 2:
						nextSelectable = currentSelectable.FindSelectableOnLeft();
						break;

						default:
						nextSelectable = startSelectable;
						break;
					}
					if (iteration > 2)
						break;
					++iteration;
				}
				currentSelectable = nextSelectable;
				currentSelectable.Select();

				// If new selectable is inside a scroll rect, move scroll to view it
				ScrollRect scroll = currentSelectable.GetComponentInParent<ScrollRect>();
				if (scroll)
					scroll.verticalNormalizedPosition =
						1f - Mathf.Abs(((RectTransform)currentSelectable.transform).anchoredPosition.y) / scroll.content.rect.height;
			}
		}
	}
}