using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoQualityManager : MonoBehaviour
{
    public string videoName;
    public VideoClip VideoClipHD;
    public VideoClip VideoClip4K;
    public RenderTexture RenderTextureHD;
    public RenderTexture RenderTexture4K;
    public Material TargetMaterial;
    public string TargetTextureNameInShader;

    void Awake()
    {
        VideoPlayer player = GetComponent<VideoPlayer>();

        string qualityKey = $"{videoName}_quality";
        Debug.Assert(PlayerPrefs.HasKey(qualityKey));
        if (PlayerPrefs.GetInt(qualityKey) == 0)
        {
            player.clip = VideoClipHD;
            player.targetTexture = RenderTextureHD;
            TargetMaterial.SetTexture(TargetTextureNameInShader, RenderTextureHD);
        }
        else
        {
            player.clip = VideoClip4K;
            player.targetTexture = RenderTexture4K;
            TargetMaterial.SetTexture(TargetTextureNameInShader, RenderTexture4K);
        }
        player.Play();
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
