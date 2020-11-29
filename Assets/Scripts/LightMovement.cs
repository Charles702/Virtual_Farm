using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMovement : MonoBehaviour
{
    public GameObject tree;
    Vector3 target_position;

    // Start is called before the first frame update
    void Start()
    {
        target_position = tree.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(target_position, Vector3.right, 20 * Time.deltaTime);
    }
}
