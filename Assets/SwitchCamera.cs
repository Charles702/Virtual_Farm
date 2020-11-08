using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    public Camera CameraOne;

    // Start is called before the first frame update
    void Start()
    {
        CameraOne.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            CameraOne.enabled = !CameraOne.enabled;
    }
}
