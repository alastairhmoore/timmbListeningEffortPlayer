using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VideoSelectionUI : MonoBehaviour
{
    public Text StatusText;
    public string VideoName;
    private UnityWebRequest _request;

    // Start is called before the first frame update
    void Start()
    {
        string PrefsKey = $"{VideoName}_url)";
        InputField inputField = GetComponentInChildren<InputField>();


        inputField.onEndEdit.AddListener((string url) =>
        {
            PlayerPrefs.SetString(PrefsKey, url);
            StartCoroutine(downloadVideo(url));
        });

        if (PlayerPrefs.HasKey(PrefsKey))
        {
            inputField.text = PlayerPrefs.GetString(PrefsKey, "");
            inputField.onEndEdit.Invoke(inputField.text);
        }

    }

    IEnumerator downloadVideo(string url)
    {
        StatusText.text = "Connecting...";

        _request = new UnityWebRequest(url);
        string savePath = Path.Combine(Application.persistentDataPath, $"{VideoName}.mp4");
        _request.downloadHandler = new DownloadHandlerFile(savePath)
        {
            removeFileOnAbort = true
        };
        _request.timeout = 30;
        yield return _request.SendWebRequest();

        if (_request.isNetworkError || _request.isHttpError)
        {
            StatusText.text = "Download error: " + _request.error;
            _request = null;
        }
        else
        {
            Debug.Assert(_request.downloadHandler.isDone);
            _request = null;
            StatusText.text = "Download complete";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_request != null)
        {
            if (_request.downloadProgress > 0)
            {
                StatusText.text = $"Downloading... {(int)(100 * _request.downloadProgress)}% complete";
            }
        }
    }
}
