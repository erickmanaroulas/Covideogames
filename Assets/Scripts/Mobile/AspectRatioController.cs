using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AspectRatioController : MonoBehaviour
{
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private float phoneReferenceWidth = 1080;
    [SerializeField] private float tabletReferenceWidth = 1500;
    
    private bool IsTablet => DeviceDiagonalSizeInInches() > 6.5f && AspectRatio < 2f;
    private static float AspectRatio => 
        (float) Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);

    public static float DeviceDiagonalSizeInInches ()
    {
        float screenWidth = Screen.width / Screen.dpi;
        float screenHeight = Screen.height / Screen.dpi;
        float diagonalInches = Mathf.Sqrt (Mathf.Pow (screenWidth, 2) + Mathf.Pow (screenHeight, 2));

        Debug.Log ("Getting device inches: " + diagonalInches);

        return diagonalInches;
    }

    private void Start()
    {
        if (canvasScaler)
        {
            var referenceWidth = /*IsTablet ? tabletReferenceWidth :*/ phoneReferenceWidth;
            canvasScaler.referenceResolution = new Vector2(referenceWidth, canvasScaler.referenceResolution.y);
        }
        else
        {
            Debug.LogError("Canvas scaler reference isn't assigned, please do so in editor");
        }
    }
}
