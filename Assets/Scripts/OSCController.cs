using UnityEngine;
using UnityEngine.Video;
using UnityOSC;

public class OSCController : MonoBehaviour
{
    private OSCReceiver osc = new OSCReceiver();
    public int listenPort = 7100;
    public bool logReceivedMessages;
    //public string videoDirectory;

    public VideoPlayer[] videoPlayers;

    class MessageSpecification
    {
        public string address;
        public (System.Type type, string description)[] arguments;
    }

    static private readonly (System.Type, string)[] videoMessageArguments = {
        (typeof(string), "Absolute path to video file")
    };

    private readonly MessageSpecification[] videoMessageSpecifications =
    {
        new MessageSpecification { address="/video/background", arguments=videoMessageArguments },
        new MessageSpecification { address="/video/1", arguments=videoMessageArguments },
        new MessageSpecification { address="/video/2", arguments=videoMessageArguments },
        new MessageSpecification { address="/video/3", arguments=videoMessageArguments },
    };

    private readonly MessageSpecification setClientAddressMessageSpecification = new MessageSpecification
    {
        address = "/set_client_address",
        arguments = new (System.Type, string)[] {
                (typeof(string), "Client IP"),
                (typeof(int), "Client port"),
        },
    };



    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"Opening OSC server on port {listenPort}.");
        osc.Open(listenPort);
    }

    private void playVideo(VideoPlayer videoPlayer, string absolutePath)
    {
        videoPlayer.Stop();
        videoPlayer.url = absolutePath;
        videoPlayer.Play();
    }


    // If address matches but not arguments then will return false and print a warning
    private bool isMatch(OSCMessage message, MessageSpecification specification)
    {
        if (message.Address != specification.address)
        {
            return false;
        }

        bool isError = message.Data.Count != specification.arguments.Length;
        if (!isError)
        {
            for (int i = 0; i < message.Data.Count; i++)
            {
                if (message.Data[i].GetType() != specification.arguments[i].type)
                {
                    isError = true;
                }
            }
        }
        if (isError)
        {
            string correctFormat = "";
            foreach ((System.Type type, string description) in specification.arguments)
            {
                correctFormat += $"<{type}> ({description}), ";
            }
            string receivedFormat = "";
            foreach (object o in message.Data)
            {
                receivedFormat += $"<{o.GetType()}> ({o.ToString()}), ";
            }
            Debug.LogWarning($"Received OSC message with address {message.Address} of incorrect format.\nCorrect format: {correctFormat}\nReceived format: {receivedFormat}");
            return false;
        }
        else
        {
            return true;
        }
    }

    private void ProcessMessage(OSCMessage message)
    {
        for (int i = 0; i < videoMessageSpecifications.Length; i++)
        {
            if (isMatch(message, videoMessageSpecifications[i]))
            {
                Debug.Assert(message.Data.Count >= 1);
                if (videoPlayers.Length < i)
                {
                    Debug.LogError($"No player set for {message.Address} video. Please set videoPlayers[{i}] on this component.");
                }
                else
                {
                    videoPlayers[i].Stop();
                    videoPlayers[i].url = (string)message.Data[0];
                    videoPlayers[i].Play();
                    Debug.Log($"{message.Address} set video player {i} to {(string)message.Data[0]}");
                }
                return;
            }
        }

        if (isMatch(message, setClientAddressMessageSpecification))
        {
            string ip = (string)message.Data[0];
            int port = (int)message.Data[1];
            if (port < 0 || port > 65535)
            {
                Debug.LogWarning($"Invalid port number received in {message.Address} message: {port}");
            }
            else
            {
                GetComponent<OSCSender>().ClientIP = ip;
                GetComponent<OSCSender>().Port = port;
            }
            return;
        }

        Debug.Log($"OSC Message with unrecognised address received: {message.ToString()}");
    }


    // Update is called once per frame
    void Update()
    {
        while (osc.hasWaitingMessages())
        {
            ProcessMessage(osc.getNextMessage());
        }
    }
}
