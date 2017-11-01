using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.Video;

public class DownloadPage : MonoBehaviour {

    public static string PathURL;
    public static string LocationPath;
    public static string VideoPath;
    private Dictionary<string, string> imageDic = new Dictionary<string, string>();
    const string BaseURL = "http://192.168.4.179:1024/list";
    const string FileUrl= "http://192.168.4.179:1024/get?name=";
    private List<String> netUrl = new List<string>();      //原有的图片
    private List<String> newNetUrl = new List<string>();   //新加入的图片
    public Image image;
    public VideoPlayer videoPlayer;
    private int index;
    private bool isChecking, isDowning;
    private bool hadNew;
    private ScreenFade sf;

    void Awake() {
        //test
        //netUrl.Add("BingWallpaper-2017-08-14.jpg");
        //netUrl.Add("BingWallpaper-2017-10-09.jpg");
        sf = gameObject.GetComponent<ScreenFade>();
        if (sf == null) sf = gameObject.AddComponent<ScreenFade>();
        PathURL = Path.Combine(new DirectoryInfo(Application.dataPath).Parent.FullName, "Images");
        LocationPath = "file:///"+ PathURL;
        VideoPath = "file://" + PathURL.Replace('\\','/');
    }

    // Use this for initialization
    void Start () {
        //检查本地文件
        CheckImage();
        //获取网络文件 /对比
        InvokeRepeating("GetNetData",0,60);
        InvokeRepeating("AutoPlay",1,5);
    }

    void OnDestroy() {
        CancelInvoke();
    }

    private void AutoPlay() {
        Debug.Log("AutoPlay.................");
        if (isChecking || isDowning) return;
        if (netUrl.Count == 0) {
            return;
        }
        index++;
        if (hadNew)
        {
            if (index > newNetUrl.Count - 1)
            {
                hadNew = false;
                index = 0;
            }
            //Debug.Log("index:"+ index+" Count:"+newNetUrl.Count);
            StartCoroutine(DownloadImages(newNetUrl[index]));
        }
        else {
            if (index > netUrl.Count - 1)
            {
                index = 0;
            }
            StartCoroutine(DownloadImages(netUrl[index]));
        }
    }

    private void GetNetData() {
        Debug.Log("GetNetData.................");
        StartCoroutine(GetPath(BaseURL));
    }

    public IEnumerator DownloadImages(string url) {
        bool isNet = false;
        isDowning = true;
        WWW www = null;
        if (imageDic.ContainsKey(url))
        {
            //替换本地路径
            //Debug.Log("Load from location............");
            www = new WWW(LocationPath + "/"+ url);

        }
        else {
            isNet = true;
            //Debug.Log("Load from network............"+ (FileUrl + WWW.EscapeURL(url)));
            //www = new WWW(BaseURL + "get?name=" + url);
            www = new WWW(FileUrl +WWW.EscapeURL(url));
        }
        //定义www为WWW类型并且等于所下载下来的WWW中内容。  
        yield return www;
        isDowning = false;
        if (www.error != null)
        {
            Debug.LogError(www.error);
        }else
        {
            if (isNet) imageDic.Add(url, "");
            if (url.EndsWith(".mp4"))
            {
                sf.OnFadeOut();
                Debug.Log("mp4.................");
                SaveData(www.bytes, url);
                videoPlayer.url = VideoPath + "/" + url;
                videoPlayer.gameObject.SetActive(true);
                image.gameObject.SetActive(false);
                videoPlayer.Play();
            }
            else if (url.EndsWith(".jpg") || url.EndsWith("png"))
            {
                sf.OnFadeOut();
                videoPlayer.gameObject.SetActive(false);
                image.gameObject.SetActive(true);
                Texture2D newTexture = www.texture;
                byte[] imageData = newTexture.EncodeToJPG();
                if (image != null)
                {
                    image.sprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f));
                }
                SaveData(imageData,url);
            }
            else {
                Debug.Log("url"+url+" 格式不支持!");
            }
        }      
    }

    private void SaveData(byte[] data,string name) {
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
        else {
            string names = www.text;
            //Debug.Log("www.data:"+www.text);
            isChecking = false;
            //get
            string[] allUrl = names.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (allUrl != null && allUrl.Length != 0) {
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
                    if (!isContain) {
                        if (addUrl == null)
                            addUrl = new List<string>();
                        addUrl.Add(allUrl[i]);
                    }
                }
                if (addUrl != null) newNetUrl = addUrl;
                netUrl = new List<String>(allUrl);
                if (newNetUrl.Count != 0) {
                    index = -1;
                    hadNew = true;
                }
                Debug.Log("newNetUrl Count:"+ newNetUrl.Count);
                //for (int i = 0; i < newNetUrl.Count; i++)
                //{
                //    Debug.Log("index "+i+ " data:"+ newNetUrl[i]);
                //}
            }
        }
    }

    private void CheckImage() {        
        string path = PathURL;
        path.Replace('/','\\');
        if (Directory.Exists(PathURL)) {
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

}


