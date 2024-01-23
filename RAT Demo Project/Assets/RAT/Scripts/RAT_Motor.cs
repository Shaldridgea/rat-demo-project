using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RAT
{
	/// <summary>
	/// Motor accessibility module
	/// </summary>
	public class RAT_Motor : MonoBehaviour
	{
		private MenuScanning menuScanning;

		public void Init()
		{
			RAT_Settings.CurrentSettingsChanged += HandleNewSettings;
			RAT_Settings.MotorSettings.MotorSettingsChanged += HandleSettingsChange;
			menuScanning = gameObject.AddComponent<MenuScanning>();
			HandleSettingsChange();
		}

		/// <summary>
		/// Enable menu scanning, starting the scan with the passed selectable UI element
		/// </summary>
		/// <param name="newSelectable"></param>
		public void SetMenuScanningSelectable(Selectable newSelectable)
		{
			if(RAT_Settings.MotorSettings.MenuScanningEnabled)
				menuScanning.SetStartSelectable(newSelectable);
		}

		/// <summary>
		/// Delay the scanning timer so it stays on the current UI element. Should be used when player makes inputs
		/// </summary>
		public void DelayScanning()
		{
			menuScanning.ResetTimer();
		}

		/// <summary>
		/// Disable menu scanning
		/// </summary>
		public void StopMenuScanning()
		{
			menuScanning.SetStartSelectable(null);
		}

		private void HandleNewSettings()
		{
			if (RAT_Settings.MotorSettings)
				RAT_Settings.MotorSettings.MotorSettingsChanged += HandleSettingsChange;
		}

		private void HandleSettingsChange()
		{
			menuScanning.enabled = RAT_Settings.MotorSettings.MenuScanningEnabled;
			if (!menuScanning.enabled)
				StopMenuScanning();
		}

		private void OnDestroy()
		{
			RAT_Settings.CurrentSettingsChanged -= HandleNewSettings;
			if (RAT_Settings.MotorSettings)
				RAT_Settings.MotorSettings.MotorSettingsChanged -= HandleSettingsChange;
			if (menuScanning)
				Destroy(menuScanning);
		}
	}
}