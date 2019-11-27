using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;
using IPAddress = System.Net.IPAddress;

public class OSCSender : MonoBehaviour
{
    public Transform UserHeadPosition;
    public bool LogSentOscMessages;
    //[SerializeField]
    //public string ClientIP
    //{
    //    get => oscClient.ClientIPAddress.ToString();
    //    set => SetClientAddress(value, ClientPort);
    //}
    //[SerializeField]
    //public int ClientPort
    //{
    //    get => oscClient.Port;
    //    set => SetClientAddress(ClientIP, value);
    //}
    public string ClientIP = "127.0.0.1";
    public int Port = 6789;
    // ClientIP that was used to set up the OSC Client, cached so we can detect change
    private string currentClientIP;

    private OSCClient oscClient;

    // Start is called before the first frame update
    void Start()
    {
        oscClient = new OSCClient(IPAddress.Parse(ClientIP), Port);
        currentClientIP = ClientIP;
    }

    private void UpdateClientAddress()
    {
        IPAddress ipAddress;
        if (IPAddress.TryParse(ClientIP, out ipAddress) && 0 <= Port && Port <= 65535)
        {
            if (oscClient != null)
            {
                oscClient.Close();
            }
            oscClient = new OSCClient(ipAddress, Port);
            // Formatting might have changed
            ClientIP = oscClient.ClientIPAddress.ToString();
            Debug.Log($"OSC Client address set to {ClientIP}:{Port}.");
        }
        else
        {
            Debug.LogWarning($"Unable to set OSC client address to invalid IP/port: {ClientIP}:{Port}. OSC is still being sent to {oscClient.ClientIPAddress}:{oscClient.Port}.");
        }
        currentClientIP = ClientIP;
    }

    // Update is called once per frame
    void Update()
    {
        if (ClientIP != currentClientIP || Port != oscClient.Port)
        {
            UpdateClientAddress();
        }

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
        OSCMessage m = new OSCMessage(address);
        foreach (object argument in arguments)
        {
            m.Append(argument);
        }
        oscClient.Send(m);
        if (LogSentOscMessages)
        {
            Debug.Log($"Sent OSC Message: {m.ToString()}");
        }
    }
}
