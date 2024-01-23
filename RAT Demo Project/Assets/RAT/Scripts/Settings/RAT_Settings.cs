using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RAT
{
	[CreateAssetMenu(fileName = "New RAT Settings", menuName = "RAT Settings")]
	public class RAT_Settings : ScriptableObject
	{
		[SerializeField]
		[Tooltip("Identifying name for this settings resource")]
		private string settingsIdentifier = "New Settings";

		private string currentName;

		[SerializeField]
		[Tooltip("Flag designating the default settings")]
		private bool defaultSettings;

		[SerializeField]
		[Tooltip("Toggle usage of vision accessibility features")]
		private bool enableVisionAccessibility;

		[SerializeField]
		[Tooltip("Toggle usage of hearing accessibility features")]
		private bool enableHearingAccessibility;

		[SerializeField]
		[Tooltip("Toggle usage of motor accessibility features")]
		private bool enableMotorAccessibility;

		private static RAT_Settings _settings;

		public delegate void SettingsValueChanged();

		public static event SettingsValueChanged CurrentSettingsChanged;

		public static RAT_Settings Settings
		{
			get {
				if (_settings == null)
					CollectSettings();
				return _settings;
			}
		}

		[SerializeField]
		[HideInInspector]
		private RAT_VisionSettings _visionSettings;

		public static RAT_VisionSettings VisionSettings
		{
			get {
				if (_settings == null)
					CollectSettings();
				return _settings._visionSettings;
			}
		}

		[SerializeField]
		[HideInInspector]
		private RAT_HearingSettings _hearingSettings;

		public static RAT_HearingSettings HearingSettings
		{
			get {
				if (_settings == null)
					CollectSettings();
				return _settings._hearingSettings;
			}
		}

		[SerializeField]
		[HideInInspector]
		private RAT_MotorSettings _motorSettings;

		public static RAT_MotorSettings MotorSettings
		{
			get {
				if (_settings == null)
					CollectSettings();
				return _settings._motorSettings;
			}
		}

		private static Dictionary<string, RAT_Settings> settingsInstances = new Dictionary<string, RAT_Settings>();

		private static bool settingsCollected;

		private const string RAT_REFERENCES_LOAD_PATH = "RAT References";

		private static ReferenceRAT ratReferences;

		/// <summary>
		/// Set the currently used RAT settings by the passed string id
		/// </summary>
		/// <param name="id">Settings identifier name</param>
		public static void SetSettingsInstance(string id)
		{
			if (settingsInstances.TryGetValue(id, out RAT_Settings setting))
			{
				RAT_Settings oldSettings = _settings;
				_settings = setting;
				CurrentSettingsChanged?.Invoke();
				oldSettings.ClearEvents();
				if (_settings._visionSettings)
					_settings._visionSettings.TriggerChangedEvent();
				if (_settings._hearingSettings)
					_settings._hearingSettings.TriggerChangedEvent();
				if (_settings._motorSettings)
					_settings._motorSettings.TriggerChangedEvent();
			}
			else
				Debug.Log("No instance of RAT settings with identifier of " + id + " could be found.");
		}

		/// <summary>
		/// Find and store references to RAT Settings
		/// </summary>
		private static void CollectSettings()
		{
			settingsInstances.Clear();
			RAT_Settings[] allSettings = Resources.FindObjectsOfTypeAll<RAT_Settings>();
			if (allSettings.Length == 0)
			{
				// If our settings aren't referenced by the engine yet then we load it now and let OnEnable take over
				Instantiate(Resources.Load<GameObject>(RAT_REFERENCES_LOAD_PATH));
				return;
			}
			for (int i = 0; i < allSettings.Length; ++i)
			{
				settingsInstances.Add(allSettings[i].settingsIdentifier, allSettings[i]);
				if (allSettings[i].defaultSettings)
					_settings = allSettings[i];
			}
		}

		/// <summary>
		/// Load references to RAT Settings resources
		/// </summary>
		private void LoadReferences()
		{
			if (EditorApplication.isCompiling || EditorApplication.isUpdating)
				return;

			if (ratReferences == null)
				ratReferences = Resources.Load<GameObject>(RAT_REFERENCES_LOAD_PATH).GetComponent<ReferenceRAT>();

			if (ratReferences != null && !ratReferences.references.Contains(this))
			{
				ratReferences.references.Add(this);
				ratReferences.RemoveNullReferences();
			}
		}

		private void OnEnable()
		{
			LoadReferences();

			if (!settingsCollected)
			{
				CollectSettings();
				settingsCollected = true;
			}
			else
			{
				// Keep settings instance cache values consistent
				if (settingsInstances.ContainsKey(settingsIdentifier))
				{
					if (settingsInstances[settingsIdentifier] != this)
					{
						if (settingsInstances[settingsIdentifier] == null)
							settingsInstances[settingsIdentifier] = this;
						else
						{
							// Rename settings instance if identifier is already taken
							while (settingsInstances.ContainsKey(settingsIdentifier))
							{
								settingsIdentifier = "New " + settingsIdentifier;
							}
							settingsInstances.Add(settingsIdentifier, this);
						}
					}
				}
			}
			currentName = settingsIdentifier;
		}

		/// <summary>
		/// Validation checks for RAT Settings resources when a value is changed
		/// </summary>
		private void OnValidate()
		{
			if (!settingsCollected)
				return;

			// Ensure settings instance cache is up to date if identifier changes
			if (currentName != settingsIdentifier)
			{
				settingsInstances.Remove(currentName);
				settingsInstances.Add(settingsIdentifier, this);
				currentName = settingsIdentifier;
			}

			// Make it so only one settings instance can be flagged default
			if (defaultSettings)
			{
				if (_settings == null)
					_settings = this;
				else if (_settings != this)
				{
					_settings.defaultSettings = false;
					_settings = this;
				}
			}

			// Toggle existence of vision, hearing, and motor settings sub-resources

			if (enableVisionAccessibility && _visionSettings == null)
			{
				RAT_VisionSettings visionSettings = CreateInstance<RAT_VisionSettings>();
				AddChildSettings(visionSettings, "Vision Settings");
				_visionSettings = visionSettings;
			}
			else if (!enableVisionAccessibility && _visionSettings != null)
			{
				DeleteChildSettings(_visionSettings);
				_visionSettings = null;
			}

			if (enableHearingAccessibility && _hearingSettings == null)
			{
				RAT_HearingSettings hearingSettings = CreateInstance<RAT_HearingSettings>();
				AddChildSettings(hearingSettings, "Hearing Settings");
				_hearingSettings = hearingSettings;
			}
			else if (!enableHearingAccessibility && _hearingSettings != null)
			{
				DeleteChildSettings(_hearingSettings);
				_hearingSettings = null;
			}

			if (enableMotorAccessibility && _motorSettings == null)
			{
				RAT_MotorSettings motorSettings = CreateInstance<RAT_MotorSettings>();
				AddChildSettings(motorSettings, "Motor Settings");
				_motorSettings = motorSettings;
			}
			else if (!enableMotorAccessibility && _motorSettings != null)
			{
				DeleteChildSettings(_motorSettings);
				_motorSettings = null;
			}
		}

		/// <summary>
		/// Add scriptable object as a nested resource to this settings resource
		/// </summary>
		private void AddChildSettings(ScriptableObject newSettingsInstance, string settingsName)
		{
			newSettingsInstance.name = settingsName;
			AssetDatabase.AddObjectToAsset(newSettingsInstance, this);
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newSettingsInstance));
			Selection.activeObject = newSettingsInstance;
		}

		/// <summary>
		/// Remove scriptable object resource from this settings resource
		/// </summary>
		private void DeleteChildSettings(Object deleteSettings)
		{
			Object folder = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(this).Replace("/" + name + ".asset", string.Empty), typeof(Object));
			if (folder != null)
				Selection.activeObject = folder;
			DestroyImmediate(deleteSettings, true);
			Selection.activeObject = this;
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}

		/// <summary>
		/// Remove this settings instance from cache
		/// </summary>
		private void RemoveSettingsInstance()
		{
			if (!settingsInstances.Remove(settingsIdentifier))
			{
				if (settingsInstances.ContainsValue(this))
				{
					string foundKey = null;
					foreach (KeyValuePair<string, RAT_Settings> pair in settingsInstances)
					{
						if (pair.Value == this)
						{
							foundKey = pair.Key;
							break;
						}
					}

					if (foundKey != null)
						settingsInstances.Remove(foundKey);
				}
			}
		}

		/// <summary>
		/// Clear event listeners for sub-resource settings
		/// </summary>
		private void ClearEvents()
		{
			if (_visionSettings)
				_visionSettings.ClearEvents();

			if (_hearingSettings)
				_hearingSettings.ClearEvents();

			if (_motorSettings)
				_motorSettings.ClearEvents();
		}

		private void OnDisable()
		{
			RemoveSettingsInstance();
		}

		private void OnDestroy()
		{
			RemoveSettingsInstance();
		}
	}
}