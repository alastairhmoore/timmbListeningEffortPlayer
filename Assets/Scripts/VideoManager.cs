using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public string VideoPath;
    public Material TargetMaterial;
    private RenderTexture renderTexture;

    void Awake()
    {
        VideoPlayer player = GetComponent<VideoPlayer>();
        player.url = VideoPath;

        player.prepareCompleted += (source) =>
        {
			if (player.targetTexture!=null && (player.targetTexture.width != player.width || player.targetTexture.height != player.height))
			{
				Debug.Log("Destroying render texture");
				Debug.Assert(player.targetTexture == renderTexture);
				renderTexture.Release();
				player.targetTexture = null;
			}
			if (player.targetTexture == null)
			{
				Debug.Log("Creating render texture");
				renderTexture = new RenderTexture((int)player.width, (int)player.height, 0);
				player.targetTexture = renderTexture;

				TargetMaterial.mainTexture = renderTexture;
				GetComponentInChildren<MeshRenderer>().material = TargetMaterial;
			}
            player.Play();
        };

		player.started += (source) =>
		{
			GetComponentInChildren<MeshRenderer>().enabled = true;
		};

		player.loopPointReached += (source) =>
		{
			if (!player.isLooping)
			{
				GetComponentInChildren<MeshRenderer>().enabled = false;
			}
		};

        Debug.Log("Preparing player");
        player.Prepare();

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
