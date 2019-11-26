using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class OSCController : MonoBehaviour
{
    private OSC osc;
    public string videoDirectory;

    public VideoPlayer[] videoPlayers;

    // Start is called before the first frame update
    void Start()
    {
        osc = GetComponent<OSC>();

        {
            const string Address = "/set/video_directory";
            (System.Type, string)[] Arguments = {
                (typeof(string), "Path to root directory containing video files")
            };
            osc.SetAddressHandler(Address, (OscMessage message) =>
            {
                if (testOSCMessage(message, Arguments))
                {
                    videoDirectory = (string)message.values[0];
                }
            });
        }

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
            for (int i = 0; i < Addresses.Length; i++)
            {
                osc.SetAddressHandler(Addresses[i], (OscMessage message) =>
                {
                    if (testOSCMessage(message, VideoArguments))
                    {
                        if (videoPlayers.Length < i)
                        {
                            Debug.LogError($"No player set for {Addresses[i]} video. Please set videoPlayers[{i}] on this component.");
                        }
                        else
                        {
                            playVideo(videoPlayers[i], (string)message.values[0]);
                        }
                    }
                });
            }
        }
    }

    private void playVideo(VideoPlayer videoPlayer, string relativePath)
    {

    }

    private bool testOSCMessage(OscMessage message, (System.Type type, string description)[] expectedArguments)
    {
        bool isError = message.values.Count == expectedArguments.Length;
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
                receivedFormat += $"<{o.GetType()}>: {o.ToString()}, ";
            }
            Debug.LogWarning($"Received OSC message with address ${message.address} of incorrect format.\nCorrect format: ${correctFormat}\nReceived format: ${receivedFormat}");
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
