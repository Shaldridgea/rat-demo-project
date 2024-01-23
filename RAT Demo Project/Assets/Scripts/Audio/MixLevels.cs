using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using RAT;

public class MixLevels : MonoBehaviour {

	public AudioMixer masterMixer;

    public void SetSfxLvl(float sfxLvl)
	{
		masterMixer.SetFloat("sfxVol", sfxLvl);
	}

	public void SetMusicLvl (float musicLvl)
	{
		masterMixer.SetFloat ("musicVol", musicLvl);
	}

    public void SetAudioToggle(UnityEngine.UI.Toggle soundToggle)
    {
        masterMixer.SetFloat("masterVol", soundToggle.isOn ? 0f : -80f);
    }
}
