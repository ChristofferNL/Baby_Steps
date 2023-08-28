using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;

    private void FixedUpdate()
    {
        if (cameraTransform != null) 
        {
            transform.position = new Vector3(transform.position.x, cameraTransform.transform.position.y, transform.position.z);
        }
    }
}
