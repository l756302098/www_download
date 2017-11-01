using UnityEngine;
using System.Collections;

public class ScreenFade : MonoBehaviour {

    private float fadeTime = 1.0f;
    public Color fadeColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);

    private Material fadeMaterial;
    public Material FadeMaterical {
        get {
            if(fadeMaterial==null)
            fadeMaterial = new Material(Shader.Find("Hidden/OVRScreenFade"));
            return fadeMaterial;
        }
    }
    private bool isFading = false;
    private YieldInstruction fadeInstruction = new WaitForEndOfFrame();
    public delegate void FinishDele();
    public event FinishDele FinishEvent;


    void Awake()
    {
        
    }

    void OnEnable()
    {
        //StartCoroutine(FadeIn());
    }

    void OnLevelWasLoaded(int level)
    {
        //StartCoroutine(FadeIn());
    }

    public void OnFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    public void OnFadeOut() {
      //  Debug.Log("OnFadeOut.................");
        StartCoroutine(FadeOut());
    }

    void OnDestroy()
    {
        if (fadeMaterial != null)
        {
            Destroy(fadeMaterial);
        }
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0.0f;
        FadeMaterical.color = fadeColor;
        Color color = fadeColor;
        isFading = true;
        color.a = 1;
        FadeMaterical.color = color;
        yield return new WaitForSeconds(0.5f);
        while (elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            float percent= 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            //Debug.Log("elapsedTime:"+ elapsedTime+ " percent:" + percent);
            color.a = percent;
            FadeMaterical.color = color;
        }

        isFading = false;
        Destroy(this);
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0.0f;
        FadeMaterical.color = fadeColor;
        Color color = fadeColor;
        isFading = true;
        while (elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            float percent =  Mathf.Clamp01(elapsedTime / fadeTime);
            //Debug.Log("elapsedTime:" + elapsedTime + " percent:" + percent);
            color.a = percent;
            FadeMaterical.color = color;
        }
        yield return new WaitForSeconds(1.0f);
        isFading = false;
        if (FinishEvent != null) {
            Debug.Log("FinishEvent.................");
            FinishEvent();
            FinishEvent = null;
        }
    }

    void OnPostRender()
    {
        if (isFading)
        {
            FadeMaterical.SetPass(0);
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Color(FadeMaterical.color);
            GL.Begin(GL.QUADS);
            GL.Vertex3(0f, 0f, -12f);
            GL.Vertex3(0f, 1f, -12f);
            GL.Vertex3(1f, 1f, -12f);
            GL.Vertex3(1f, 0f, -12f);
            GL.End();
            GL.PopMatrix();
        }
    }

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(10, 20, 100, 50), "FadeIn"))
    //    {
    //        StartCoroutine(FadeIn());
    //    }
    //    if (GUI.Button(new Rect(10, 100, 100, 50), "FadeOut"))
    //    {
    //        StartCoroutine(FadeOut());
    //    }
    //}
}
