using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/// <summary>
/// 相机随机特效
/// </summary>
public class CameraRadomEffect : MonoBehaviour {

    public GameObject target;

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("RandomEffect",1, 5);
    }

    // Update is called once per frame
    //void OnGUI () {
    //       if (GUI.Button(new Rect(10,20,100,30),"Random")) {
    //           RemoveForeScripts(target);
    //           AddScripts(target,Excel_EffectArray.Instance().GetRandom());
    //       }
    //}


    public void RandomEffect() {
        RemoveForeScripts(target);
        AddScripts(target, Excel_EffectArray.Instance().GetRandom());
    }

    public static void RemoveForeScripts(GameObject go) {
        MonoBehaviour[] mbs= go.GetComponents<MonoBehaviour>();
        for (int i = 0; i < mbs.Length; i++)
        {
            if ((mbs[i] as ScreenFade) != null) continue;
            if ((mbs[i] as DownloadPage) != null) continue;
            Destroy(mbs[i]);
        }
    }

    public static void AddScripts(GameObject go,Data_EfffectArray data) {
        for (int i = 0; i < data.effects.Count; i++)
        {
            int eid = data.effects[i];
            Data_Effect de = Excel_Effect.Instance().GetById(eid);
            Type t = Type.GetType(de.name);
            Component c = go.AddComponent(t);
            FieldInfo[] fis = t.GetFields();
            foreach (FieldInfo fi in fis)
            {
                foreach (var item in de.effect)
                {
                    if (fi.Name == item.Key)
                    {
                        fi.SetValue(c, item.Value);
                    }
                }
            }
        }
       
    }

}
