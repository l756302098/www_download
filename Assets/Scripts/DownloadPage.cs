using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.Video;

public class DownloadPage : MonoBehaviour {

    public Image image;
    public VideoPlayer videoPlayer;
    private int addIndex;
    private int index;
    private ScreenFade sf;
    public int startIndex;
    public bool hadNew;

    void Awake() {
        sf = gameObject.GetComponent<ScreenFade>();
        if (sf == null) sf = gameObject.AddComponent<ScreenFade>();
    }

    // Use this for initialization
    void Start () {
        InvokeRepeating("AutoPlay",1,5);
    }

    void OnDestroy() {
        CancelInvoke();
    }

    private void AutoPlay() {
        //Debug.Log("AutoPlay.................");
        if (NetWorkManager.netUrl.Count == 0) {
            return;
        }
        addIndex+=4;
        index = (addIndex + startIndex) % NetWorkManager.newNetUrl.Count;
        if (hadNew)
        {
            if (index > NetWorkManager.newNetUrl.Count - 1)
            {
                hadNew = false;
                index = 0;
            }
            NetWorkManager.Instance.AddDownloadTask(NetWorkManager.netUrl[index], this.Finish);
        }
        else {
            if (index > NetWorkManager.netUrl.Count - 1)
            {
                index = 0;
            }
            NetWorkManager.Instance.AddDownloadTask(NetWorkManager.netUrl[index], this.Finish);
        }
    }


    public void Finish(byte[] data, bool isError, FileType type, string url)
    {
        //Debug.Log("Finish......................");
        if (!isError)
        {
            if (type == FileType.Image)
            {
                sf.OnFadeOut();
                videoPlayer.gameObject.SetActive(false);
                image.gameObject.SetActive(true);
                if (NetWorkManager.imageSize.ContainsKey(url))
                {
                   
                    Vector2 size = NetWorkManager.imageSize[url];
                    Texture2D newTexture = new Texture2D((int)size.x, (int)size.y);
                    //Debug.Log("SizeToFit....................w "+ size.x+" h:"+ size.y);
                    newTexture.LoadImage(data);
                    if (image != null)
                    {
                        image.sprite = Sprite.Create(newTexture, new Rect(0, 0, (int)size.x, (int)size.y), new Vector2(0.5f, 0.5f));
                        image.GetComponent<RectTransform>().sizeDelta = new Vector2((int)size.x, (int)size.y);
                    }
                    //SizeToFit
                    float defaultPixel =   (float)defaultWidth/defaultHeight;
                    float pixel = size.x / size.y;
                    if (pixel < defaultPixel)
                    {
                        //宽小 以宽为基准
                        if (defaultWidth > size.x)
                        {
                            float multiple = defaultWidth / size.x;
                            image.GetComponent<RectTransform>().localScale = new Vector3(multiple, multiple, 1);
                            //Debug.Log("宽小 multiple " + multiple);
                        }
                        else
                        {
                            float multiple = 1;
                            image.GetComponent<RectTransform>().localScale = new Vector3(multiple, multiple, 1);
                            //Debug.Log("宽小 multiple " + multiple);
                        }
                    }
                    else if (pixel > defaultPixel)
                    {
                        //高小
                        if (defaultHeight > size.y)
                        {
                            float multiple = defaultHeight / size.y;
                            image.GetComponent<RectTransform>().localScale = new Vector3(1 / multiple, 1 / multiple, 1);
                           // Debug.Log("高小 multiple " + multiple);
                        }
                        else
                        {
                            float multiple = 1;
                            image.GetComponent<RectTransform>().localScale = new Vector3(multiple, multiple, 1);
                            //Debug.Log("高小 multiple " + multiple);
                        }
                    }
                    else {
                        if (defaultHeight > size.y)
                        {
                            float multiple = defaultHeight / size.y;
                            image.GetComponent<RectTransform>().localScale = new Vector3( 1/multiple, 1/multiple, 1);
                        }
                        else
                        {
                            float multiple = defaultHeight/ size.y;
                            image.GetComponent<RectTransform>().localScale = new Vector3(multiple, multiple, 1);
                        }
                    }
                }
                else {
                    //Debug.Log("Default...........................");
                    Texture2D newTexture = new Texture2D(defaultWidth, defaultHeight);
                    newTexture.LoadImage(data);
                    if (image != null)
                    {
                        image.sprite = Sprite.Create(newTexture, new Rect(0, 0, defaultWidth, defaultHeight), new Vector2(0.5f, 0.5f));
                    }
                }
               
            }
            else if (type == FileType.Video)
            {
                sf.OnFadeOut();
                videoPlayer.url = NetWorkManager.VideoPath + "/" + url;
                videoPlayer.gameObject.SetActive(true);
                image.gameObject.SetActive(false);
                videoPlayer.Play();
            }
        }
    }

    private int defaultWidth=1920;
    private int defaultHeight = 1080;

}


public enum FileType {
Video,
Image,
Other
} 

