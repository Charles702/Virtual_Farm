using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwithUIMode : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas UICanvas;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // press U to turn on/off panel
        if(Input.GetKeyDown(KeyCode.U)) 
        {
            UICanvas.enabled = !UICanvas.enabled;

        }

        if (UICanvas.enabled)
            Cursor.lockState = CursorLockMode.None;
        else 
            Cursor.lockState = CursorLockMode.Locked;              

    }
}
