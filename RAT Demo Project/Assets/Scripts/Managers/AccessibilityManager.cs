using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RAT;

public class AccessibilityManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RAT_Controller.StartController();
        RAT_Controller.Instance.Hearing.SetPlayerTransform(GameObject.FindGameObjectWithTag("Player").transform);
    }
}