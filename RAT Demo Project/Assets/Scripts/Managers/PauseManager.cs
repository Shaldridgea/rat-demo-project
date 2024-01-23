using UnityEngine;
using System.Collections;
using RAT;
using UnityEngine.UI;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseManager : MonoBehaviour {
	
	public AudioMixerSnapshot paused;
	public AudioMixerSnapshot unpaused;
	
	Canvas canvas;
	
	private IEnumerator Start()
	{
		canvas = GetComponent<Canvas>();
        yield return new WaitForEndOfFrame(); 
        GameEvents.EventBus.TriggerGameStarted();
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			canvas.enabled = !canvas.enabled;
			Pause();
		}
	}
	
	public void Pause()
	{
		Time.timeScale = Time.timeScale == 0 ? 1 : 0;
		Lowpass ();

		if (Time.timeScale == 0)
		{
			Selectable found = GetComponentInChildren<Selectable>();
			RAT_Controller.Instance.Motor.SetMenuScanningSelectable(found);
		}
		else
			RAT_Controller.Instance.Motor.StopMenuScanning();
	}
	
	void Lowpass()
	{
		if (Time.timeScale == 0)
		{
			paused.TransitionTo(.01f);
		}
		
		else
			
		{
			unpaused.TransitionTo(.01f);
		}
	}
	
	public void Quit()
	{
		#if UNITY_EDITOR 
		EditorApplication.isPlaying = false;
		#else 
		Application.Quit();
		#endif
	}
}
