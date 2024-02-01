using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform mainCamTransform;


    private void Start()
    {
    }
    private void LateUpdate()
    {
        this.transform.LookAt(transform.position + mainCamTransform.rotation * Vector3.forward, mainCamTransform.rotation * Vector3.up);
    }
}
