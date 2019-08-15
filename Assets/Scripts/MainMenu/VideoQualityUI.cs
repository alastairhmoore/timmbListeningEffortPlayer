using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoQualityUI : MonoBehaviour
{
    public string videoName;

    // Start is called before the first frame update
    void Start()
    {
        Toggle toggleHD = null;
        Toggle toggle4K = null;
        foreach (Toggle toggle in GetComponentsInChildren<Toggle>())
        {
            Text label = toggle.GetComponentInChildren<Text>();
            if (label && label.text=="HD")
            {
                toggleHD = toggle;
            }
            else if (label && label.text=="4K")
            {
                toggle4K = toggle;
            }
            else
            {
                Debug.LogWarning($"Unrecognised toggle: \"{(label? label.text : "No label found")}\".", this);
            }
        }
        Debug.Assert(toggleHD != null);
        Debug.Assert(toggle4K != null);

        string key = videoName + "_quality";
        int defaultValue = toggle4K.isOn ? 1 : 0;
        // Save a 1 for 4K, 0 for HD
        if (PlayerPrefs.HasKey(key))
        {
            int prefValue = PlayerPrefs.GetInt(key);
            Debug.Log($"Found saved preference for {key}: {prefValue}");
            if (prefValue == 0)
            {
                // Set these manually rathert than relying on the toggle group
                // as the group doesn't work yet (possibly due to script execution
                // order)
                toggleHD.isOn = true;
                toggle4K.isOn = false;
            }
            else
            {
                toggleHD.isOn = false;
                toggle4K.isOn = true;
            }
        }

        toggle4K.onValueChanged.AddListener((bool is4K) => {
            int newPrefValue = is4K ? 1 : 0;
            //Debug.Log($"Saving preference for {key} as {newPrefValue}");
            PlayerPrefs.SetInt(key, newPrefValue);
        });
    }

}
