using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using System;
public class EmguUI : MonoBehaviour
{
    [SerializeField] Image uiImage;
    [SerializeField] Button generateButton, saveButton;
    [SerializeField] string path;
    [SerializeField] string outputPath;

    byte[,,] currentBytes;
    Mat currentEmguImage;
    EmguProcessor processor;
    Sprite mySprite;
    Image<Bgr, byte> original;
    Image<Bgr, byte> testImage0;
    Image<Bgr, byte> testImage1;
    private void Awake()
    {
        generateButton.onClick.AddListener(Generate);
        saveButton.onClick.AddListener(Save);

    }

    private void Save()
    {
        testImage0.ToBitmap().Save(Application.streamingAssetsPath + "/" + outputPath);
    }

    private void Generate()
    {
        currentEmguImage = CvInvoke.Imread(Application.streamingAssetsPath + "/" + path);
        original = currentEmguImage.Clone().ToImage<Bgr, byte>();
        testImage0 = currentEmguImage.ToImage<Bgr, byte>();
        currentEmguImage = CvInvoke.Imread(Application.streamingAssetsPath + "/test.jpg");
        testImage1 = currentEmguImage.ToImage<Bgr, byte>();
        currentBytes = testImage0.Data;
        uiImage.rectTransform.sizeDelta = new Vector2(testImage0.Width, testImage0.Height);
        processor = GetComponent<EmguProcessor>();
        processor.Init(testImage0);
        //ExtractDamagePoints();
        //processor.DrawGrid(40, 3);
        //TestCustomFloodfill();
        processor.FindTopLeftPointByFloodfill();
        //processor.FindTopLeftPointBySweepline();
        Display(processor.GetTex());
    }

    private void TestCustomFloodfill()
    {
        Vector3Int offsetTargetRGB = new Vector3Int(7, 7, 7);
        Vector3Int newColorRGB = new Vector3Int(0, 0, 0);
        double counter = 0;
        HashSet<int> theSet = new HashSet<int>();
        try
        {
            processor.CustomFloodfill(18, 18, ref offsetTargetRGB, ref newColorRGB, ref theSet, ref counter);
        }
        catch (StackOverflowException e)
        {
            Debug.LogError(e.Message);
            Debug.LogError(e.Data.ToString());
            Debug.Log(counter);
        }
    }

    private void ExtractDamagePoints()
    {
        //204, 196, 165
        processor.RemoveColor(new Vector3Int(204, 196, 165), 35);
        //130, 125, 119
        processor.RemoveColor(new Vector3Int(130, 125, 119), 35);
        processor.RemoveNoiseNextToBlack(.3f, 0.3f, 255);
    }

    private void Display(Texture2D obj)
    {
        Rect rect = new Rect(0f, 0f, testImage0.Width, testImage0.Height);

        mySprite = Sprite.Create(obj, rect, Vector2.zero);
        uiImage.sprite = mySprite;

    }
}
