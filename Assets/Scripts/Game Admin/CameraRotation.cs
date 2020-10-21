using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    float rotationRate = 30f;

    void Update()
    {
        Vector3 rotation = Vector3.zero;
        if(Input.GetKey(KeyCode.A))
        {
            rotation.y += rotationRate;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rotation.y -= rotationRate;
        }
        transform.Rotate(rotation * Time.deltaTime, Space.World);
    }
}