using System;
using UnityEngine;

public class EyeMakupParams : MonoBehaviour
{
    public bool applyMakeup = true;
    public bool flipImage = true;
    public bool drawLandmarks = false;

    public readonly int NodeSize = 5;

    [Header("Stability")]
    [Range(0, 1)]
    public float filterStability = 0.45f;
    [Range(0, 1)]
    public float linerStability = 0.2f;

    [Header("LinerKernel")]
    [Range(0, 20)]
    public int linerKernelW = 2;
    [Range(0, 20)]
    public int linerKernelH = 3;
    [Range(1, 20)]
    public int linerThickness = 1;
    [Range(1, 20)]
    public int borderDecrement = 6;

    [Header("EyePointsTop")]
    [Range(0, 360)]
    public int pointsTopH = 296;
    [Range(0, 100)]
    public int pointsTopS = 33;
    [Range(0, 100)]
    public int pointsTopV = 59;
    [Range(0, 100)]
    public int pointsTopBlending = 35;
    [Range(0, 50)]
    public int pointsTopBorderSmooth = 50;

    [Header("EyePointsMid")]
    [Range(0, 360)]
    public int pointsMidH = 339;
    [Range(0, 100)]
    public int pointsMidS = 58;
    [Range(0, 100)]
    public int pointsMidV = 51;
    [Range(0, 100)]
    public int pointsMidBlending = 58;
    [Range(0, 50)]
    public int pointsMidBorderSmooth = 27;

    [Header("EyePointsBot")]
    [Range(0, 360)]
    public int pointsBotH = 330;
    [Range(0, 100)]
    public int pointsBotS = 71;
    [Range(0, 100)]
    public int pointsBotV = 42;
    [Range(0, 100)]
    public int pointsBotBlending = 53;
    [Range(0, 50)]
    public int pointsBotBorderSmooth = 32;

    [Header("EyePointsUnd")]
    [Range(0, 360)]
    public int pointsUndH = 248;
    [Range(0, 100)]
    public int pointsUndS = 67;
    [Range(0, 100)]
    public int pointsUndV = 1;
    [Range(0, 100)]
    public int pointsUndBlending = 40;
    [Range(0, 50)]
    public int pointsUndBorderSmooth = 16;

    [Header("EyePointsTop2")]
    [Range(0, 360)]
    public int pointsTop2H = 296;
    [Range(0, 100)]
    public int pointsTop2S = 33;
    [Range(0, 100)]
    public int pointsTop2V = 59;
    [Range(0, 100)]
    public int pointsTop2Blending = 35;
    [Range(0, 50)]
    public int pointsTop2BorderSmooth = 50;

    [Header("EyePointsMid2")]
    [Range(0, 360)]
    public int pointsMid2H = 339;
    [Range(0, 100)]
    public int pointsMid2S = 58;
    [Range(0, 100)]
    public int pointsMid2V = 51;
    [Range(0, 100)]
    public int pointsMid2Blending = 58;
    [Range(0, 50)]
    public int pointsMid2BorderSmooth = 27;

    [Header("EyePointsBot2")]
    [Range(0, 360)]
    public int pointsBot2H = 330;
    [Range(0, 100)]
    public int pointsBot2S = 71;
    [Range(0, 100)]
    public int pointsBot2V = 42;
    [Range(0, 100)]
    public int pointsBot2Blending = 53;
    [Range(0, 50)]
    public int pointsBot2BorderSmooth = 32;

    [Header("EyePointsUnd2")]
    [Range(0, 360)]
    public int pointsUnd2H = 332;
    [Range(0, 100)]
    public int pointsUnd2S = 19;
    [Range(0, 100)]
    public int pointsUnd2V = 7;
    [Range(0, 100)]
    public int pointsUnd2Blending = 33;
    [Range(0, 50)]
    public int pointsUnd2BorderSmooth = 15;

    public int GetH(int index)
    {

        return index switch
        {
            0 => pointsTopH,
            1 => pointsMidH,
            2 => pointsBotH,
            3 => pointsUndH,
            4 => pointsTop2H,
            5 => pointsMid2H,
            6 => pointsBot2H,
            7 => pointsUnd2H,
            _ => throw new ArgumentException("Index should be [0, 7]")
        };
    }

    public int GetS(int index)
    {

        return index switch
        {
            0 => pointsTopS,
            1 => pointsMidS,
            2 => pointsBotS,
            3 => pointsUndS,
            4 => pointsTop2S,
            5 => pointsMid2S,
            6 => pointsBot2S,
            7 => pointsUnd2S,
            _ => throw new ArgumentException("Index should be [0, 7]")
        };
    }

    public int GetV(int index)
    {

        return index switch
        {
            0 => pointsTopV,
            1 => pointsMidV,
            2 => pointsBotV,
            3 => pointsUndV,
            4 => pointsTop2V,
            5 => pointsMid2V,
            6 => pointsBot2V,
            7 => pointsUnd2V,
            _ => throw new ArgumentException("Index should be [0, 7]")
        };
    }

    public int GetBlending(int index)
    {

        return index switch
        {
            0 => pointsTopBlending,
            1 => pointsMidBlending,
            2 => pointsBotBlending,
            3 => pointsUndBlending,
            4 => pointsTop2Blending,
            5 => pointsMid2Blending,
            6 => pointsBot2Blending,
            7 => pointsUnd2Blending,
            _ => throw new ArgumentException("Index should be [0, 7]")
        };
    }

    public int GetBorderSmooth(int index)
    {

        return index switch
        {
            0 => pointsTopBorderSmooth,
            1 => pointsMidBorderSmooth,
            2 => pointsBotBorderSmooth,
            3 => pointsUndBorderSmooth,
            4 => pointsTop2BorderSmooth,
            5 => pointsMid2BorderSmooth,
            6 => pointsBot2BorderSmooth,
            7 => pointsUnd2BorderSmooth,
            _ => throw new ArgumentException("Index should be [0, 7]")
        };
    }
}
