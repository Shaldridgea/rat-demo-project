using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RAT
{
	/// <summary>
	/// Top level RAT module controller
	/// </summary>
	public class RAT_Controller : MonoBehaviour
	{
		private static RAT_Controller instance;

		public static RAT_Controller Instance
		{
			get {
				if (instance == null)
					StartController();

				return instance;
			}
		}

		private RAT_Vision visionController;

		public RAT_Vision Vision
		{
			get {
				if (visionController == null)
				{
					visionController = gameObject.AddComponent<RAT_Vision>();
					visionController.Init();
				}
				return visionController;
			}
		}

		private RAT_Hearing hearingController;

		public RAT_Hearing Hearing
		{
			get {
				if (hearingController == null)
				{
					hearingController = gameObject.AddComponent<RAT_Hearing>();
					hearingController.Init();
				}
				return hearingController;
			}
		}

		private RAT_Motor motorController;

		public RAT_Motor Motor
		{
			get {
				if (motorController == null)
				{
					motorController = gameObject.AddComponent<RAT_Motor>();
					motorController.Init();
				}
				return motorController;
			}
		}

		public Camera RAT_Camera { get; private set; }

		/// <summary>
		/// Creates RAT Controller singleton in scene
		/// </summary>
		public static void StartController()
		{
			if (instance != null)
				return;

			instance = new GameObject("RAT Controller").AddComponent<RAT_Controller>();
			RAT_Settings.CurrentSettingsChanged += instance.HandleNewSettings;
			instance.RAT_Camera = Camera.main;

			RAT_VisionSettings visionSettings = RAT_Settings.VisionSettings;
			if (visionSettings)
			{
				visionSettings.VisionSettingsChanged += instance.HandleVisionSettingsChange;
				instance.TryCreateVisionModule();
			}

			RAT_HearingSettings hearingSettings = RAT_Settings.HearingSettings;
			if (hearingSettings)
			{
				hearingSettings.HearingSettingsChanged += instance.HandleHearingSettingsChange;
				instance.TryCreateHearingModule();
			}

			RAT_MotorSettings motorSettings = RAT_Settings.MotorSettings;
			if (motorSettings)
			{
				motorSettings.MotorSettingsChanged += instance.HandleMotorSettingsChange;
				instance.TryCreateMotorModule();
			}
		}

		private void HandleNewSettings()
		{
			if (RAT_Settings.VisionSettings)
				RAT_Settings.VisionSettings.VisionSettingsChanged += HandleVisionSettingsChange;
			else if (visionController)
				Destroy(visionController);

			if (RAT_Settings.HearingSettings)
				RAT_Settings.HearingSettings.HearingSettingsChanged += HandleHearingSettingsChange;
			else if (hearingController)
				Destroy(hearingController);

			if (RAT_Settings.MotorSettings)
				RAT_Settings.MotorSettings.MotorSettingsChanged += HandleMotorSettingsChange;
			else if (motorController)
				Destroy(motorController);
		}

		private void HandleVisionSettingsChange()
		{
			if (visionController == null)
			{
				TryCreateVisionModule();
			}
		}

		private void HandleHearingSettingsChange()
		{
			if (hearingController == null)
			{
				TryCreateHearingModule();
			}
		}

		private void HandleMotorSettingsChange()
		{
			if (motorController == null)
			{
				TryCreateMotorModule();
			}
		}

		private void TryCreateVisionModule()
		{
			RAT_VisionSettings visionSettings = RAT_Settings.VisionSettings;
			if (visionSettings.LowContrastEnabled || visionSettings.OutlinesEnabled || visionSettings.ColorblindEnabled)
			{
				visionController = gameObject.AddComponent<RAT_Vision>();
				visionController.Init();
			}
		}

		private void TryCreateHearingModule()
		{
			RAT_HearingSettings hearingSettings = RAT_Settings.HearingSettings;
			if (hearingSettings.SubtitlesEnabled || hearingSettings.CaptionsEnabled)
			{
				hearingController = gameObject.AddComponent<RAT_Hearing>();
				hearingController.Init();
			}
		}
		private void TryCreateMotorModule()
		{
			RAT_MotorSettings motorSettings = RAT_Settings.MotorSettings;
			if (motorSettings.MenuScanningEnabled)
			{
				motorController = gameObject.AddComponent<RAT_Motor>();
				motorController.Init();
			}
		}

		private IEnumerator Start()
		{
			yield return new WaitForEndOfFrame();
			if (RAT_Settings.VisionSettings.LowContrastEnabled)
				Vision.EnableLowContrastEffect();

			if (RAT_Settings.VisionSettings.OutlinesEnabled)
				Vision.EnableOutlinesEffect();

			if (RAT_Settings.VisionSettings.ColorblindEnabled)
				Vision.EnableColorblindEffect();
		}

		private void OnDestroy()
		{
			RAT_Settings.CurrentSettingsChanged -= HandleNewSettings;

			if (RAT_Settings.VisionSettings)
				RAT_Settings.VisionSettings.VisionSettingsChanged -= HandleVisionSettingsChange;

			if (RAT_Settings.HearingSettings)
				RAT_Settings.HearingSettings.HearingSettingsChanged -= HandleHearingSettingsChange;

			if (RAT_Settings.MotorSettings)
				RAT_Settings.MotorSettings.MotorSettingsChanged -= HandleMotorSettingsChange;
		}

		public void SetCamera(Camera newCam)
		{
			RAT_Camera = newCam;

			if (visionController)
				visionController.SetEffectsCamera(newCam);
		}
	}
}