using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FeedbackEffect : MonoBehaviour
{
    const int TotalSlices = 10;
    //public float DelaySeconds = 2;

    public float BoostThreshold = 1;
    public float UVMultiplier = 0.9f;
    public Vector3 BoostAmount = new Vector3(1.1f, 1.1f, 1.1f);
    public float BlendAlpha = 0.5f;

    CommandBuffer _buf, _clear;
    Camera _cam;
    Material _mat;
    RenderTexture _history;
    RenderTexture _history2;

    private void Awake()
    {
        _cam = GetComponent<Camera>();

        try
        {
            _mat = new Material(Shader.Find("Hidden/FeedbackEffect"));
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            _mat = null;
            return;
        }
        int rtid = Shader.PropertyToID("_CopyOfBuffer");
        //sliceID = Shader.PropertyToID("_CurrentSliceIndex");
        //prevSliceID = Shader.PropertyToID("_PrevSliceIndex");
        var format = _cam.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;

        _history = new RenderTexture(Screen.width / 4, Screen.height / 4, 0, format);
        _history2 = new RenderTexture(Screen.width / 4, Screen.height / 4, 0, format);
        _clear = new CommandBuffer();
        _clear.name = "History Clear";
        _clear.SetRenderTarget(_history);
        _clear.ClearRenderTarget(true, true, Color.black);
        _clear.SetRenderTarget(_history2);
        _clear.ClearRenderTarget(true, true, Color.black);
        ClearHistory();

        _buf = new CommandBuffer();
        _buf.name = "Feedback Effect";

        //_buf.GetTemporaryRT(rtid, -1, -1, 0, FilterMode.Bilinear, format);

        //_buf.SetRenderTarget(rtid);
        //_buf.ClearRenderTarget(true, true, Color.black);
        //_buf.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);

        //_buf.CopyTexture(BuiltinRenderTextureType.CameraTarget, rtid); // XXX fails badly when MSAA is on

        _buf.Blit(_history, _history2, _mat);
        _buf.Blit(_history2, _history, _mat);
        _buf.Blit(_history, _history2, _mat);
        _buf.Blit(BuiltinRenderTextureType.CameraTarget, _history, _mat);

        _buf.Blit(_history2, BuiltinRenderTextureType.CameraTarget, _mat);

        //_buf.ReleaseTemporaryRT(rtid);
    }

    private void Update()
    {
        if (!_mat) return;
        _mat.SetFloat("_BoostThreshold", BoostThreshold);
        _mat.SetFloat("_UVMultiplier", UVMultiplier);
        _mat.SetFloat("_BlendAlpha", BlendAlpha);
        _mat.SetVector("_BoostAmount", new Vector4(BoostAmount.x, BoostAmount.y, BoostAmount.z));
    }

    public void ClearHistory()
    {
        Graphics.ExecuteCommandBuffer(_clear);
    }

    public void OnEnable()
    {
        _cam.AddCommandBuffer(CameraEvent.BeforeImageEffects, _buf);
    }

    public void OnDisable()
    {
        _cam.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, _buf);
    }

    private void OnDestroy()
    {
        _history.Release();
        _history2.Release();
    }

    private void OnGUI()
    {
        if (_mat == null)
        {
            GUI.Label(new Rect(50, 50, 500, 50), "Hidden/FeedbackEffect shader probably missing. You gotta include that in: Edit->Project Settings->Graphics->Always Included Shaders");
        }
    }
}
