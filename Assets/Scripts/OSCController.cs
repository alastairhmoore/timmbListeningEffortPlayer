using System;
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
		(typeof(int), "Video player ID (0 for background)"),
		(typeof(string), "Absolute path to video file")

	};

	private readonly MessageSpecification videoMessageSpecification = new MessageSpecification
	{
		address = "/video/play",
		arguments = videoMessageArguments
	};

	private readonly MessageSpecification setClientAddressMessageSpecification = new MessageSpecification
	{
		address = "/set_client_address",
		arguments = new (System.Type, string)[] {
				(typeof(string), "Client IP"),
				(typeof(int), "Client port"),
		},
	};

	// This is used by OSCSender
	public int GetIDForVideoPlayer(VideoPlayer player)
	{
		//Debug.Assert(videoPlayers.Length == videoMessageSpecifications.Length);
		for (int i = 0; i < videoPlayers.Length; i++)
		{
			if (videoPlayers[i] == player)
			{
				return i;
			}
		}
		Debug.LogError($"VideoPlayer {player} is not registered with this OSCController.");
		return -404;
	}

	void Start()
	{
		Debug.Assert(videoPlayers.Length == 4);
		Debug.Log($"Opening OSC server on port {listenPort}.");
		osc.Open(listenPort);
	}

	//void OnDisable()
	//{
	//    Debug.Log($"Closing OSC server.");
	//    osc.Close();
	//}

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
		if (isMatch(message, videoMessageSpecification))
		{
			Debug.Assert(message.Data.Count >= 2);
			int i = (int)message.Data[0];
			if (i < 0 || videoPlayers.Length < i)
			{
				Debug.LogError($"{message.Address} message received for video player ID {i}. Valid video player IDs are at least 0 and at most {videoPlayers.Length-1}");
			}
			else
			{
				videoPlayers[i].Stop();
				videoPlayers[i].url = (string)message.Data[1];
				videoPlayers[i].Prepare();
				//videoPlayers[i].Play();
				// videoPlayers[i] will play automatically due to VideoController
				Debug.Log($"{message.Address} set video player {i} to {(string)message.Data[1]}");
			}
			return;
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
