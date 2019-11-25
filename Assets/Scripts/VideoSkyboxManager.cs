using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Experimental.Rendering;

public class VideoSkyboxManager : MonoBehaviour
{
    public string VideoPath;
    public Material TargetMaterial;

    void Awake()
    {
        VideoPlayer player = GetComponent<VideoPlayer>();
        player.url = VideoPath;// Path.Combine(Application.persistentDataPath, $"{VideoName}.mp4");

        player.prepareCompleted += (source) =>
        {
            RenderTexture renderTexture = new RenderTexture((int)player.width, (int)player.height, 0);
            player.targetTexture = renderTexture;
            TargetMaterial.mainTexture = renderTexture;
            player.Play();
        };

        player.Prepare();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
