using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCSender : MonoBehaviour
{
    public Transform UserHeadPosition;
    public bool LogSentOscMessages;
    private OSC osc;

    // Start is called before the first frame update
    void Start()
    {
        osc = GetComponent<OSC>();
    }

    // Update is called once per frame
    void Update()
    {
        if (UserHeadPosition.hasChanged)
        {
            send("/head_rotation", new ArrayList
            {
                UserHeadPosition.rotation.eulerAngles.x,
                UserHeadPosition.rotation.eulerAngles.y,
                UserHeadPosition.rotation.eulerAngles.z,
            });

            UserHeadPosition.hasChanged = false;
        }
    }

    private void send(string address, ArrayList arguments)
    {
        OscMessage m = new OscMessage();
        m.address = address;
        m.values = arguments;
        osc.Send(m);
        if (LogSentOscMessages)
        {
            Debug.Log($"Sent OSC Message: {m.ToString()}");
        }
    }
}
