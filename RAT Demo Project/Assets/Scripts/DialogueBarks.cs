using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RAT;

public class DialogueBarks : MonoBehaviour
{
    [System.Serializable]
    private class DialogueData
    {
        public string Name;
        public AudioClip Clip;
        public string Transcript;
        public GameEvents.GameEventType Event;
    }

    [SerializeField]
    private DialogueData[] dialogueList;

    [SerializeField]
    private AudioSource dialogueSource;

    private Queue<DialogueData> barkQueue = new Queue<DialogueData>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(DialogueData d in dialogueList)
        {
            var currentDialogue = d;
            GameEvents.EventBus.RegisterListener(d.Event, () => AddToQueue(currentDialogue));
        }
    }

    private void AddToQueue(DialogueData data) => barkQueue.Enqueue(data);

    private void PlayDialogue(DialogueData data)
    {
        if (dialogueSource.isPlaying)
            dialogueSource.Stop();
        dialogueSource.clip = data.Clip;
        dialogueSource.Play();
        if (RAT_Settings.HearingSettings.SubtitlesEnabled)
            RAT_Controller.Instance.Hearing.ShowSubtitle(data.Transcript, data.Clip.length * 1.1f);
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (barkQueue.Count > 0)
        {
            if (!dialogueSource.isPlaying)
                PlayDialogue(barkQueue.Dequeue());
        }
    }
}
