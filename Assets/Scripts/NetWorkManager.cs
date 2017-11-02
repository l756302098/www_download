using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Drawing;

public delegate void DownloadFinish(byte[] data, bool isError, FileType type,string url);

public class NetWorkManager : MonoBehaviour {

    public static NetWorkManager Instance;

    public DownloadPage[] pages;
    public static string PathURL;
    public static string LocationPath;
    public static string VideoPath;
    private static Dictionary<string, string> imageDic = new Dictionary<string, string>();
    public static Dictionary<string, Vector2> imageSize = new Dictionary<string, Vector2>();
    const string BaseURL = "http://192.168.4.179:1024/list";
    const string FileUrl = "http://192.168.4.179:1024/get?name=";
    public static List<String> netUrl = new List<string>();      //原有的图片
    public static List<String> newNetUrl = new List<string>();   //新加入的图片
    public static bool isChecking, isDowning;
    private bool hadNew {
        set {
            if (value) {
                if (pages != null && pages.Length != 0) {
                    for (int i = 0; i < pages.Length; i++)
                    {
                        pages[i].hadNew = true;
                    }
                }
            }
        }
    }


    void Awake()
    {
        Instance = this;
        PathURL = Path.Combine(new DirectoryInfo(Application.dataPath).Parent.FullName, "Images");
        LocationPath = "file:///" + PathURL;
        VideoPath = "file://" + PathURL.Replace('\\', '/');
    }


    void Update() {
        if (urlTask.Count == 0) return;
        foreach (var item in urlTask)
        {
            StartCoroutine(DownloadImages(item.Value,callBackTask[item.Key]));
        }
        ClearDownloadTask();
    }

    // Use this for initialization
    void Start()
    {
        //检查本地文件
        CheckImage();
        //获取网络文件 /对比
        InvokeRepeating("GetNetData", 0, 60);
    }

    void OnDestroy()
    {
        Instance = null;
        CancelInvoke();
    }

    private void GetNetData()
    {
        StartCoroutine(GetPath(BaseURL));
    }
    public static int mainKey;
    public static Dictionary<int, string> urlTask = new Dictionary<int, string>();
    //public static Dictionary<int, string> finishUrlTask = new Dictionary<int, string>();
    public static Dictionary<int, DownloadFinish> callBackTask = new Dictionary<int, DownloadFinish>();

    public void AddDownloadTask(string url,DownloadFinish callback) {
        lock (this)
        {
            mainKey++;
            urlTask.Add(mainKey, url);
            callBackTask.Add(mainKey, callback);
        }
    }

    public void ClearDownloadTask()
    {
        lock (this)
        {
            urlTask.Clear();
            callBackTask.Clear();
        }
    }

    public IEnumerator DownloadImages(string url, DownloadFinish callback)
    {
        bool isNet = false;
        isDowning = true;
        WWW www = null;
        if (imageDic.ContainsKey(url))
        {
            //替换本地路径
            //Debug.Log("Load from location............");
            www = new WWW(LocationPath + "/" + url);

        }
        else
        {
            isNet = true;
            //Debug.Log("Load from network............" + (FileUrl + WWW.EscapeURL(url)));
            www = new WWW(FileUrl + WWW.EscapeURL(url));
        }
        //定义www为WWW类型并且等于所下载下来的WWW中内容。  
        yield return www;
        isDowning = false;

        if (www.error != null)
        {
            Debug.LogError(www.error);
            if(callback!=null) callback(null,true,FileType.Other,"");
        }
        else
        {
            if (isNet) imageDic.Add(url, "");
            SaveData(www.bytes, url);
			//if (callback != null) callback(www.bytes, false, FileType.Image);
            if (url.EndsWith(".mp4")) {
				if (callback != null) callback(www.bytes,false, FileType.Video, url);
            }
            else if (url.EndsWith(".jpg") || url.EndsWith("png"))
            {
                SaveSize(url,www.bytes);
                if (callback != null) callback(www.bytes, false, FileType.Image, url);
            }
            else
            {
				if (callback != null) callback(www.bytes, false, FileType.Other, url);
            }
            //if (url.EndsWith(".mp4"))
            //{
            //    //sf.OnFadeOut();
            //    SaveData(www.bytes, url);
            //    //videoPlayer.url = VideoPath + "/" + url;
            //    //videoPlayer.gameObject.SetActive(true);
            //    //image.gameObject.SetActive(false);
            //    //videoPlayer.Play();
            //}
            //else if (url.EndsWith(".jpg") || url.EndsWith("png"))
            //{
            //    //sf.OnFadeOut();
            //    //videoPlayer.gameObject.SetActive(false);
            //    //image.gameObject.SetActive(true);
            //    Texture2D newTexture = www.texture;
            //    byte[] imageData = newTexture.EncodeToJPG();
            //    //if (image != null)
            //    //{
            //    //    image.sprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f));
            //    //}
            //    SaveData(imageData, url);
            //}
            //else
            //{
            //    Debug.Log("url" + url + " 格式不支持!");
            //}
        }
    }

    private void SaveData(byte[] data, string name)
    {
        if (!Directory.Exists(PathURL))
        {
            Directory.CreateDirectory(PathURL);
        }
        try
        {
            string path = PathURL + "/" + name;
            File.WriteAllBytes(path, data);
        }
        catch (IOException e)
        {
            print(e);
        }
    }

    public IEnumerator GetPath(string url)
    {
        isChecking = true;
        //Debug.Log("GetPath url:"+ url);
        WWW www = new WWW(url);
        //定义www为WWW类型并且等于所下载下来的WWW中内容。  
        yield return www;
        if (www.error != null)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string names = www.text;
            //Debug.Log("www.data:"+www.text);
            isChecking = false;
            //get
            string[] allUrl = names.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (allUrl != null && allUrl.Length != 0)
            {
                //compare add
                List<string> addUrl = null;
                for (int i = allUrl.Length - 1; i >= 0; i--)
                {
                    bool isContain = false;
                    for (int j = 0; j < netUrl.Count; j++)
                    {
                        if (netUrl[j] == allUrl[i])
                        {
                            isContain = true;
                            continue;
                        }
                    }
                    if (!isContain)
                    {
                        if (addUrl == null)
                            addUrl = new List<string>();
                        addUrl.Add(allUrl[i]);
                    }
                }
                if (addUrl != null) newNetUrl = addUrl;
                netUrl = new List<String>(allUrl);
                if (newNetUrl.Count != 0)
                {
                    hadNew = true;
                }
                Debug.Log("newNetUrl Count:" + newNetUrl.Count);
                //for (int i = 0; i < newNetUrl.Count; i++)
                //{
                //    Debug.Log("index "+i+ " data:"+ newNetUrl[i]);
                //}
            }
        }
    }

    private void CheckImage()
    {
        string path = PathURL;
        path.Replace('/', '\\');
        if (Directory.Exists(PathURL))
        {
            var files = Directory.GetFiles(@PathURL, "*.png");
            foreach (var file in files)
            {
                Debug.Log(file);
                string[] spStr = file.Split('\\');
                if (spStr != null)
                {
                    String cName = spStr[spStr.Length - 1];
                    //Debug.Log("name:"+ cName+ " file:" + file);
                    imageDic.Add(cName, "");
                }
            }
        }

    }

    public void SaveSize(string  url, byte[] data) {
        MemoryStream ms1 = new MemoryStream(data);
        Bitmap bm = (Bitmap)System.Drawing.Image.FromStream(ms1);
        ms1.Close();
        //Debug.Log("Width:"+bm.Width+" height:"+bm.Height);
        if (!imageSize.ContainsKey(url)) {
            imageSize.Add(url,new Vector2(bm.Width,bm.Height));
        }
    }
}
