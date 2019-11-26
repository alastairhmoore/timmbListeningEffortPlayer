using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCSender : MonoBehaviour
{
    public Camera VRCamera;
    private OSC osc;

    // Start is called before the first frame update
    void Start()
    {
        osc = GetComponent<OSC>();
    }

    // Update is called once per frame
    void Update()
    {
        OscMessage m = new OscMessage();
        m.address = "/head_rotation";
        m.values = new ArrayList{
            VRCamera.transform.rotation.eulerAngles.x,
            VRCamera.transform.rotation.eulerAngles.y,
            VRCamera.transform.rotation.eulerAngles.z,
        };
        osc.Send(m);
    }
}
