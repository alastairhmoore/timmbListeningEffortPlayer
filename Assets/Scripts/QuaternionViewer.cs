using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make transform visible in editor as quaternion
// based on https://stackoverflow.com/a/60938755/3041762
public class QuaternionViewer : MonoBehaviour
{
    public Vector4 rotation;
    public bool updateFromTransform = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // for testing purposes
        //transform.Rotate(0.3f * Vector3.up); 

        if (updateFromTransform)
        {
            rotation.x = transform.localRotation.x;
            rotation.y = transform.localRotation.y;
            rotation.z = transform.localRotation.z;
            rotation.w = transform.localRotation.w;
        }
    }
}
