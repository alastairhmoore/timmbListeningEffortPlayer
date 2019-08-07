using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.Linq;

public class FPSDrawer : MonoBehaviour
{
    //public VideoPlayer video0;
    private TMPro.TextMeshProUGUI Text;
    private VideoFPSCounter[] videoFPSCounters;

    // Start is called before the first frame update
    void Start()
    {
        VideoPlayer[] videoPlayers = FindObjectsOfType<VideoPlayer>();
        videoFPSCounters = new VideoFPSCounter[videoPlayers.Length];
        for (int i=0; i<videoPlayers.Length; i++)
        {
            videoFPSCounters[i] = videoPlayers[i].GetComponent<VideoFPSCounter>();
            Debug.Assert(videoFPSCounters[i] != null);
        }

        Text = GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        Text.text = "";
        for (int i = 0; i < videoFPSCounters.Length; i++)
        {
            Text.text += $"video{i}: {videoFPSCounters[i].CurrentFPS:0.} fps\n";
        }
    }
}
