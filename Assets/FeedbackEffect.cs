using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FeedbackEffect : MonoBehaviour
{
    CommandBuffer _buf;
    Camera _cam;
    Material _mat;

    private void Awake()
    {
        _mat = new Material(Shader.Find("Hidden/FeedbackEffect"));
        int rtid = Shader.PropertyToID("_CopyOfBuffer");
        _buf = new CommandBuffer();
        _buf.name = "Feedback Effect";

        _buf.GetTemporaryRT(rtid, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.Default);
        _buf.CopyTexture(BuiltinRenderTextureType.CameraTarget, rtid);

        _cam = GetComponent<Camera>();
    }

    public void OnEnable()
    {
        _cam.AddCommandBuffer(CameraEvent.BeforeImageEffects, _buf);
    }

    public void OnDisable()
    {
        _cam.RemoveCommandBuffer(CameraEvent.BeforeImageEffects, _buf);
    }
}
