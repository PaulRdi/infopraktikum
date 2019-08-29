using UnityEngine;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using UnityEngine.UI;
using System;

public class EmguTest : MonoBehaviour
{
    [SerializeField]
    Image img;
    [SerializeField]
    [Range(0f, 1f)]
    float noiseScale;

    [SerializeField]
    [Range(0.001f, 0.01f)]
    float noiseGranularity;
    Mat invert;
    byte[,,]data;
    Mat inputImage;
    Image<Bgr, byte> t_img;
    Texture2D tex;
    Sprite theSprite;
    private void Start()
    {
        invert = new Mat();
        string path = Application.streamingAssetsPath + "/test.jpg";
        inputImage = CvInvoke.Imread(path, Emgu.CV.CvEnum.ImreadModes.AnyColor);
        CvInvoke.BitwiseNot(inputImage, invert);
        t_img = inputImage.ToImage<Bgr, byte>();
        data = t_img.Data;
        tex = new Texture2D(invert.Width, invert.Height);
        img.rectTransform.sizeDelta = new Vector2(invert.Width, invert.Height);
        int l1 = data.GetLength(0);
        int l2 = data.GetLength(1);
        for (int y = l1 - 1; y >= 0; y--)
        {
            for (int x = 0; x < l2; x++)
            {
                float r = (byte)data[y, x, 1] / 255.0f;
                float g = (byte)data[y, x, 2] / 255.0f;
                float b = (byte)data[y, x, 0] / 255.0f;
                Color col = new Color(r, g, b);
                float noiseSample = (Mathf.PerlinNoise(x * noiseGranularity, y * noiseGranularity) * noiseScale);
                col = col - new Color(noiseSample, noiseSample, noiseSample);
                col.a = 1.0f;
                tex.SetPixel(Math.Abs(x - l2), Math.Abs(y - l1), col);

            }
        }
        tex.Apply();
        theSprite = Sprite.Create(tex, new Rect(0f, 0f, (float)invert.Width, (float)invert.Height), Vector2.zero);
        img.sprite = theSprite;
    }
}
