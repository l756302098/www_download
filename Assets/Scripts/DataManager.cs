using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class DataManager : MonoBehaviour {

    public static bool isFinshed;
    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        LoadData("EffectArray", Excel_EffectArray.Instance().LoadData);
        LoadData("Effect", Excel_Effect.Instance().LoadData);
        Debug.Log("DataManager add  Finshed ");
    }

    private void LoadData(string name, Action<string> OnLoaded)
    {
        StartCoroutine(Load("Config/" + name, OnLoaded));
    }

    private IEnumerator Load(string path, Action<string> OnLoaded)
    {
        ResourceRequest loadRequest = Resources.LoadAsync(path);
        yield return loadRequest;
        string str = ((TextAsset)loadRequest.asset).text;
        OnLoaded(str);
    }

}
