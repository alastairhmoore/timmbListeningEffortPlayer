using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using ViveSR.anipal.Eye;

// Some strange stuff happens to the instance of this class with the callback being
// called on a null instance. It might be to do with the Marshal function pointer.
// Anyway, to avoid it, many things are kept as static.
public class Pupilometry : MonoBehaviour
{
    [Serializable]
    public struct Data
    {
        public bool hasUser;
        public float leftPupilDiameterMm, rightPupilDiameterMm;
        public bool isLeftPupilDiameterValid, isRightPupilDiameterValid;
    }
    [SerializeField]
    //private Data pupilometryData;
	public bool logChanges = false;
	private static bool sLogChanges = false;

	private static bool isCallbackAdded;

    public static event EventHandler<Data> DataChanged;

	// Start is called before the first frame update
	void Start()
	{
		//SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
	}

	//private void OnApplicationQuit()
	//{
	//	Debug.Log("OnApplicationQuit");
	//}
	//private void OnDisable()
	//{
	//	Debug.Log("OnDisable");
	//}

	//private void OnDestroy()
	//{
	//	Debug.Log("OnDestroy");
	//}


	void Update()
	{
		sLogChanges = logChanges;

		if (!isCallbackAdded && (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING || SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT))
		{
			SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
			isCallbackAdded = true;

		}

		//if (isCallbackAdded != SRanipal_Eye_Framework.Instance.EnableEyeDataCallback)
		//{
		//	if (isCallbackAdded)
		//	{
		//		SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
		//		isCallbackAdded = false;
		//	}
		//	else
		//	{
		//		SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
		//		isCallbackAdded = true;
		//	}
		//}
		//Debug.Assert(isCallbackAdded == SRanipal_Eye_Framework.Instance.EnableEyeDataCallback);
	}

	private void EyeCallback(ref EyeData_v2 eye_data)
	{
		//if (this == null)
		//{
		//	Debug.LogWarning("EyeCallback called on null object");
		//	return;
		//}
        Data data = new Data
        {
            hasUser = !eye_data.no_user,
            leftPupilDiameterMm = eye_data.verbose_data.left.pupil_diameter_mm,
            rightPupilDiameterMm = eye_data.verbose_data.right.pupil_diameter_mm,
            isLeftPupilDiameterValid = eye_data.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY),
            isRightPupilDiameterValid = eye_data.verbose_data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY),
        };

		if (sLogChanges)
		{
			Debug.Log("Pupilometry data: " + data);
		}

        DataChanged?.Invoke(this, data);
    }


}
