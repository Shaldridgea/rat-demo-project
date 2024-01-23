using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAT
{
	/// <summary>
	/// Motor settings data resource
	/// </summary>
	public class RAT_MotorSettings : ScriptableObject
	{
		[SerializeField]
		[Tooltip("Toggle whether to use menu scanning, which automatically goes through UI elements one by one to allow selection without moving")]
		private bool menuScanningEnabled;

		public bool MenuScanningEnabled
		{
			get {
				return menuScanningEnabled;
			}

			set {
				menuScanningEnabled = value;
				MotorSettingsChanged?.Invoke();
			}
		}

		[SerializeField]
		[Tooltip("How long in seconds to wait for input before moving to the next UI element")]
		private float waitInterval;

		public float WaitInterval
		{
			get {
				return waitInterval;
			}

			set {
				waitInterval = value;
				MotorSettingsChanged?.Invoke();
			}
		}

		public event RAT_Settings.SettingsValueChanged MotorSettingsChanged;

		public void ClearEvents()
		{
			MotorSettingsChanged = null;
		}

		public void TriggerChangedEvent()
		{
			MotorSettingsChanged?.Invoke();
		}
	}
}