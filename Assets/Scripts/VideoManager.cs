using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public string VideoPath;
    //public VideoClip VideoClipHD;
    //public VideoClip VideoClip4K;
    //public RenderTexture RenderTextureHD;
    //public RenderTexture RenderTexture4K;
    //public bool CreateRenderTextureAndBindToTargetMaterial;
    //public Material MaterialPrototype;
    public Material TargetMaterial;
    //public string TargetTextureNameInShader;
    private RenderTexture renderTexture;

    void Awake()
    {
        VideoPlayer player = GetComponent<VideoPlayer>();
        player.url = VideoPath;// Path.Combine(Application.persistentDataPath, $"{VideoName}.mp4");

        player.prepareCompleted += (source) =>
        {
            //if (CreateRenderTextureAndBindToTargetMaterial)
            //{
            Debug.Log("creating render texture");
            renderTexture = new RenderTexture((int)player.width, (int)player.height, 0);
            player.targetTexture = renderTexture;
            //TargetMaterial.SetTexture(TargetTextureNameInShader, renderTexture);
            //Debug.Log("creating material");
            //Material myTargetMaterial = new Material(MaterialPrototype);
            //myTargetMaterial.mainTexture = renderTexture;
            TargetMaterial.mainTexture = renderTexture;
            GetComponentInChildren<MeshRenderer>().material = TargetMaterial;
            //}
            player.Play();
        };

        Debug.Log("preparing player");
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

    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            Debug.Log("Destroying render texture");
            renderTexture.Release();
        }
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
