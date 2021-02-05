using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make transform visible in editor as Euler Angles in Tascar coordinates
// based on https://stackoverflow.com/a/60938755/3041762
public class TascarEulerViewer : MonoBehaviour
{
    public float RotX;
    public float RotY;
    public float RotZ;
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
            transform.localRotation.ToTascarEulerZYX(ref RotZ, ref RotY, ref RotX);
        }
    }
}
