using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorMovement : MonoBehaviour
{
    public Camera CameraOne;
    public bool CanMove = false;
    public float speed = 18f;

    public CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            CameraOne.enabled = !CameraOne.enabled;

        if (CameraOne.enabled)
            CanMove = true;
        else
            CanMove = false;

        if (CanMove)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * speed * Time.deltaTime);

            // Move up
            if (Input.GetKey(KeyCode.Space))
                controller.Move(Vector3.up * speed * Time.deltaTime);

            // Move down
            if (Input.GetKey(KeyCode.LeftControl))
                controller.Move(Vector3.down * speed * Time.deltaTime);

        }
    }
}
