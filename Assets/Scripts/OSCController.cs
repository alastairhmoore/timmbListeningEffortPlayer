using UnityEngine;
using UnityEngine.Video;

public class OSCController : MonoBehaviour
{
    private OSC osc;
    public bool logReceivedMessages;
    //public string videoDirectory;

    public VideoPlayer[] videoPlayers;

    // Start is called before the first frame update
    void Start()
    {
        osc = GetComponent<OSC>();

        {
            (System.Type, string)[] VideoArguments =
            {
                (typeof(string), "Path to video file relative to video directory")
            };
            string[] Addresses = {
                    "/video/background",
                    "/video/1",
                    "/video/2",
                    "/video/3",
                };
            for (int I = 0; I < Addresses.Length; I++)
            {
                // We need to make a local copy to ensure each created anonymous function
                // captures a copy of the current loop counter value rather than a reference to the loop 
                // counter
                int i = I;
                osc.SetAddressHandler(Addresses[i], (OscMessage message) =>
                {
                    if (testOSCMessage(message, VideoArguments))
                    {
                        Debug.Assert(message.values.Count >= 1);
                        if (videoPlayers.Length < i)
                        {
                            Debug.LogError($"No player set for {Addresses[i]} video. Please set videoPlayers[{i}] on this component.");
                        }
                        else
                        {
                            playVideo(videoPlayers[i], (string)message.values[0]);
                            Debug.Log($"{message.address} set video player {i} to {(string)message.values[0]}");
                        }
                    }
                });
            }
        }

        {
            (System.Type, string)[] SetClientAddressArguments =
            {
                (typeof(string), "IP of client to send OSC messages to"),
                (typeof(int), "Port of client to send OSC messages to"),
            };
            const string Address = "/set_client_address";
            osc.SetAddressHandler(Address, (OscMessage message) =>
            {
                if (testOSCMessage(message, SetClientAddressArguments))
                {
                    string ip = (string)message.values[0];
                    int port = (int)message.values[1];
                    if (port < 0 || port > 65535)
                    {
                        Debug.LogWarning($"Invalid port number received in {Address} message: {port}");
                    }
                    else
                    {
                        osc.outIP = ip;
                        osc.outPort = port;
                        Debug.Log($"Client address set to {ip}:{port}");
                    }
                }
            });
        }

        osc.SetAllMessageHandler((OscMessage message) =>
        {
            if (logReceivedMessages)
            {
                Debug.Log($"OSC Message received: {message.ToString()}");
            }
        });
    }

    private void playVideo(VideoPlayer videoPlayer, string absolutePath)
    {
        videoPlayer.Stop();
        videoPlayer.url = absolutePath;
        videoPlayer.Play();
    }

    private bool testOSCMessage(OscMessage message, (System.Type type, string description)[] expectedArguments)
    {
        bool isError = message.values.Count != expectedArguments.Length;
        if (!isError)
        {
            for (int i = 0; i < message.values.Count; i++)
            {
                if (message.values[i].GetType() != expectedArguments[i].type)
                {
                    isError = true;
                }
            }
        }
        if (isError)
        {
            string correctFormat = "";
            foreach ((System.Type type, string description) in expectedArguments)
            {
                correctFormat += $"<{type}> ({description}), ";
            }
            string receivedFormat = "";
            foreach (object o in message.values)
            {
                receivedFormat += $"<{o.GetType()}> ({o.ToString()}), ";
            }
            Debug.LogWarning($"Received OSC message with address {message.address} of incorrect format.\nCorrect format: {correctFormat}\nReceived format: {receivedFormat}");
            return false;
        }
        else
        {
            return true;
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
