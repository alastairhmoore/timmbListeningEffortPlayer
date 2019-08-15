using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class MainScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadDevice());
    }

    IEnumerator LoadDevice()
    {
        XRSettings.LoadDeviceByName("cardboard");
        yield return null;
        XRSettings.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetButtonDown("Fire1"))
		{
			Debug.Log("Fire1 button: returning to main menu");
			SceneManager.LoadSceneAsync("MainMenu");
		}
	}
}
