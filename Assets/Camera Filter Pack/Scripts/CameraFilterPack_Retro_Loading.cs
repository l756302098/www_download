///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2015 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Retro/Loading")]
public class CameraFilterPack_Retro_Loading : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0.1f, 10f)]
public float Speed = 1f;

public static float ChangeValue;

#endregion
#region Properties
Material material
{
get
{
if(SCMaterial == null)
{
SCMaterial = new Material(SCShader);
SCMaterial.hideFlags = HideFlags.HideAndDontSave;
}
return SCMaterial;
}
}
#endregion
void Start ()
{
		ChangeValue = Speed;

SCShader = Shader.Find("CameraFilterPack/Retro_Loading");
if(!SystemInfo.supportsImageEffects)
{
enabled = false;
return;
}
}

void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
{
if(SCShader != null)
{
TimeX+=Time.deltaTime;
if (TimeX>100)  TimeX=0;
material.SetFloat("_TimeX", TimeX);
			material.SetFloat("_Value", Speed);

material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);
}
}
	void OnValidate(){ChangeValue=Speed;} void Update ()
{
if (Application.isPlaying)
{
			Speed = ChangeValue;

}
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/Retro_Loading");
}
#endif
}
void OnDisable ()
{
if(SCMaterial)
{
DestroyImmediate(SCMaterial);
}
}
}
