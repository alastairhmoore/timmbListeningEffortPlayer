using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityOSC;
using IPAddress = System.Net.IPAddress;

public class OSCSender : MonoBehaviour
{
	public Transform UserHeadPosition;
	public Pupilometry pupilometry;
	public bool LogSentOscMessages;
	public string ClientIP = "127.0.0.1";
	public int Port = 6789;
	// ClientIP that was used to set up the OSC Client, cached so we can detect change
	private string currentClientIP;

	private OSCClient oscClient;

	private OSCController oscController;
	private List<VideoPlayer> videoPlayersWithCallbacksRegistered = new List<VideoPlayer>();

	void Awake()
	{
		oscController = GetComponent<OSCController>();
	}

	// Start is called before the first frame update
	void Start()
	{
		oscClient = new OSCClient(IPAddress.Parse(ClientIP), Port);
		currentClientIP = ClientIP;

	}

	void OnEnable()
	{
		Pupilometry.DataChanged += OnPupilometryDataChanged;

		Debug.Assert(videoPlayersWithCallbacksRegistered.Count == 0);
		OSCController controller = GetComponent<OSCController>();
		for (int i = 0; i < controller.videoPlayers.Length; i++)
		{
			controller.videoPlayers[i].prepareCompleted += OnVideoPlayerPrepared;

			controller.videoPlayers[i].sendFrameReadyEvents = true;
			controller.videoPlayers[i].frameReady += OnVideoPlayerFrameReady;

			videoPlayersWithCallbacksRegistered.Add(controller.videoPlayers[i]);

		}
	}

	void OnDisable()
	{
		Pupilometry.DataChanged -= OnPupilometryDataChanged;

		foreach (VideoPlayer player in videoPlayersWithCallbacksRegistered)
		{
			player.prepareCompleted -= OnVideoPlayerPrepared;
			player.frameReady -= OnVideoPlayerFrameReady;
		}
		videoPlayersWithCallbacksRegistered.Clear();
	}

	private void OnVideoPlayerPrepared(VideoPlayer player)
	{
		string oscAddress = oscController.GetOSCAddressForVideoPlayer(player);
		Send($"{oscAddress}/prepared", new ArrayList { player.url });
	}

	private void OnVideoPlayerFrameReady(VideoPlayer player, long frameIndex)
	{
		if (frameIndex == 0)
		{
			string oscAddress = oscController.GetOSCAddressForVideoPlayer(player);
			Send($"{oscAddress}/first_frame", new ArrayList { player.url });
		}
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


	private void OnPupilometryDataChanged(object sender, Pupilometry.Data data)
	{
		Send("/pupilometry", new ArrayList
		{
			data.hasUser? 1 : 0,
			data.leftPupilDiameterMm,
			data.rightPupilDiameterMm,
			data.isLeftPupilDiameterValid? 1 : 0,
			data.isRightPupilDiameterValid? 1 : 0,
		});
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
			Send("/head_rotation", new ArrayList{
				UserHeadPosition.rotation.eulerAngles.x,
				UserHeadPosition.rotation.eulerAngles.y,
				UserHeadPosition.rotation.eulerAngles.z,
			});
			UserHeadPosition.hasChanged = false;
		}
	}


	private void Send(string address, ArrayList arguments)
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
