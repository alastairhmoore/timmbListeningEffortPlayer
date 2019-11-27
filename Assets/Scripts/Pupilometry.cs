using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using ViveSR.anipal.Eye;

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
    private Data pupilometryData;
	public bool logChanges = false;

	private bool isCallbackAdded;

    public event EventHandler<Data> DataChanged;

	// Start is called before the first frame update
	void Start()
	{
		
	}


	void Update()
	{
		if (isCallbackAdded != SRanipal_Eye_Framework.Instance.EnableEyeDataCallback)
		{
			if (isCallbackAdded)
			{
				SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
				isCallbackAdded = false;
			}
			else
			{
				SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
				isCallbackAdded = true;
			}
		}
		Debug.Assert(isCallbackAdded == SRanipal_Eye_Framework.Instance.EnableEyeDataCallback);
	}

	private void EyeCallback(ref EyeData_v2 eye_data)
    {
        pupilometryData = new Data
        {
            hasUser = !eye_data.no_user,
            leftPupilDiameterMm = eye_data.verbose_data.left.pupil_diameter_mm,
            rightPupilDiameterMm = eye_data.verbose_data.right.pupil_diameter_mm,
            isLeftPupilDiameterValid = eye_data.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY),
            isRightPupilDiameterValid = eye_data.verbose_data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_PUPIL_DIAMETER_VALIDITY),
        };

		if (logChanges)
		{
			Debug.Log("Pupilometry data: " + pupilometryData);
		}

        DataChanged?.Invoke(this, pupilometryData);
    }


}
