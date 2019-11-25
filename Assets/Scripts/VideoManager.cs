using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Experimental.Rendering;

public class VideoManager : MonoBehaviour
{
    public string VideoPath;
    //public VideoClip VideoClipHD;
    //public VideoClip VideoClip4K;
    //public RenderTexture RenderTextureHD;
    //public RenderTexture RenderTexture4K;
    public Material TargetMaterial;
    //public string TargetTextureNameInShader;

    void Awake()
    {
        VideoPlayer player = GetComponent<VideoPlayer>();
        player.url = VideoPath;// Path.Combine(Application.persistentDataPath, $"{VideoName}.mp4");

        player.prepareCompleted += (source) =>
        {
            RenderTexture renderTexture = new RenderTexture((int)player.width, (int)player.height, 0);
            player.targetTexture = renderTexture;
            //TargetMaterial.SetTexture(TargetTextureNameInShader, renderTexture);
            TargetMaterial.mainTexture = renderTexture;
            player.Play();
        };

        player.Prepare();




        //string qualityKey = $"{VideoName}_quality";
        //Debug.Assert(PlayerPrefs.HasKey(qualityKey));
        //if (PlayerPrefs.GetInt(qualityKey) == 0)
        //{
        //    player.clip = VideoClipHD;
        //    player.targetTexture = RenderTextureHD;
        //    TargetMaterial.SetTexture(TargetTextureNameInShader, RenderTextureHD);
        //}
        //else
        //{
        //    player.clip = VideoClip4K;
        //    player.targetTexture = RenderTexture4K;
        //    TargetMaterial.SetTexture(TargetTextureNameInShader, RenderTexture4K);
        //}
        //player.Play();
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
