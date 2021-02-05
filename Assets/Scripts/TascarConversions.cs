using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TascarConversions
{
    // Unity rotation to TASCAold_ro's rotation
    public static void ToTascarEulerZYX(this Quaternion unityQuaternion,
                                        ref float rotZDeg,
                                        ref float rotYDeg,
                                        ref float rotXDeg)
    {
        // assignment order is x, y, z, w
        // map during creation
        Vector4 tascarQr = new Vector4(
            -unityQuaternion.z,
             unityQuaternion.x,
            -unityQuaternion.y,
             unityQuaternion.w);

        //
        // *** convert to rotation matrix ***
        //
        // N.B. subscript n notation refers to matlab indexing of vectorised matrix 
        float two_over_mag = 2.0f / tascarQr.magnitude;

        float p_2 = tascarQr.w * tascarQr.x * two_over_mag;
        float p_3 = tascarQr.w * tascarQr.y * two_over_mag;
        float p_4 = tascarQr.w * tascarQr.z * two_over_mag;

        float p_6 = tascarQr.x * tascarQr.x * two_over_mag;
        float p_11 = tascarQr.y * tascarQr.y * two_over_mag;
        float p_16 = tascarQr.z * tascarQr.z * two_over_mag;

        float p_10 = tascarQr.x * tascarQr.y * two_over_mag;
        float p_14 = tascarQr.x * tascarQr.z * two_over_mag;
        float p_15 = tascarQr.y * tascarQr.z * two_over_mag;

        // rotation matrix needs 9 elements
        // to avoid bugs due to messing up the indexing we here
        // ignore the 0 element and keep 1-based indexing, as 
        // our matlab reference code
        float[] ro = new float[10];

        // diagonals
        ro[1] = 1 - p_11 - p_16;
        ro[5] = 1 - p_16 - p_6;
        ro[9] = 1 - p_6 - p_11;
        // above diagonal
        ro[4] = p_10 - p_4;
        ro[8] = p_15 - p_2;
        ro[3] = p_14 - p_3;
        // below diagonal
        ro[2] = p_10 + p_4;
        ro[6] = p_15 + p_2;
        ro[7] = p_14 + p_3;

        //
        // *** successively unwind the rotation matrix according to the tascar convention ***
        //
        rotXDeg = UnwindX(ro);
        rotYDeg = UnwindY(ro);
        rotZDeg = UnwindZ(ro);


    }

    private static float UnwindX(float[] ro)
    {
        float y = ro[8];
        float x = ro[9];
        TrigIdentities trig = new TrigIdentities(y, x);
        float rotXDegrees = -trig.t * Mathf.Rad2Deg;

        // make copy of old values before unwinding
        float[] old_ro = new float[10];
        ro.CopyTo(old_ro, 0);

        ro[2] = trig.c * old_ro[2] - trig.s * old_ro[3];
        ro[3] = trig.c * old_ro[3] + trig.s * old_ro[2];
        ro[5] = trig.c * old_ro[5] - trig.s * old_ro[6];
        ro[6] = trig.c * old_ro[6] + trig.s * old_ro[5];
        ro[8] = trig.c * old_ro[8] - trig.s * old_ro[9];
        ro[9] = trig.c * old_ro[9] + trig.s * old_ro[8];

        return rotXDegrees;
    }

    private static float UnwindY(float[] ro)
    {
        float y = ro[7];
        float x = ro[9];
        TrigIdentities trig = new TrigIdentities(y, x);
        float rotYDegrees = trig.t * Mathf.Rad2Deg;

        // make copy of old values before unwinding
        float[] old_ro = new float[10];
        ro.CopyTo(old_ro, 0);

        ro[3] = trig.c * old_ro[3] + trig.s * old_ro[1];
        ro[1] = trig.c * old_ro[1] - trig.s * old_ro[3];
        ro[6] = trig.c * old_ro[6] + trig.s * old_ro[4];
        ro[4] = trig.c * old_ro[4] - trig.s * old_ro[6];
        ro[9] = trig.c * old_ro[9] + trig.s * old_ro[7];
        ro[7] = trig.c * old_ro[7] - trig.s * old_ro[9];

        return rotYDegrees;
    }

    private static float UnwindZ(float[] ro)
    {
        float y = ro[2];
        float x = ro[1];
        TrigIdentities trig = new TrigIdentities(y, x);
        float rotZDegrees = trig.t * Mathf.Rad2Deg;

        // make copy of old values before unwinding
        float[] old_ro = new float[10];
        ro.CopyTo(old_ro, 0);

        ro[1] = trig.c * old_ro[1] + trig.s * old_ro[2];
        ro[2] = trig.c * old_ro[2] - trig.s * old_ro[1];
        ro[4] = trig.c * old_ro[4] + trig.s * old_ro[5];
        ro[5] = trig.c * old_ro[5] - trig.s * old_ro[4];
        ro[7] = trig.c * old_ro[7] + trig.s * old_ro[8];
        ro[8] = trig.c * old_ro[8] - trig.s * old_ro[7];

        return rotZDegrees;
    }

    private class TrigIdentities
    {
        public readonly float s = 0.0f;         // sin value
        public readonly float c = float.NaN;    // cosine value
        public readonly float t = float.NaN;    // atan2 value

        public TrigIdentities(float y, float x)
        {
            if (y == 0)
            {
                t = System.Convert.ToSingle(x < 0.0f);
                c = 1.0f - 2.0f * t;
                //r = abs(x);
                t = t * Mathf.PI;
            }
            else
            {
                if (Mathf.Abs(y) > Mathf.Abs(x)) //abs(y) > abs(x)
                {
                    float q = x / y;
                    float u = Mathf.Sqrt(1.0f + Mathf.Pow(q, 2.0f)) * Mathf.Sign(y);
                    s = 1.0f / u;
                    c = s * q;
                    // r = y * u;
                }
                else
                {
                    float q = y / x;
                    float u = Mathf.Sqrt(1.0f + Mathf.Pow(q, 2.0f)) * Mathf.Sign(x);
                    c = 1.0f / u;
                    s = c * q;
                    //  r = x * u;
                }
                t = Mathf.Atan2(s, c);
            }
        }
    }
}