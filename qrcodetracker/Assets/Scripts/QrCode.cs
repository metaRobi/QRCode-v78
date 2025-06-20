using Meta.XR.MRUtilityKit;
using UnityEngine;
using UnityEngine.UI;
using System;

[DisallowMultipleComponent]
public sealed class QrCode : MonoBehaviour
{
    [SerializeField] Text payloadText, trackingText;
    [SerializeField] LineRenderer line;
    [SerializeField] RectTransform canvas;

    MRUKTrackable trackable;
    Rect cachedRect;
    bool lastTracked;

    public void Initialize(MRUKTrackable t)
    {
        trackable = t;
        payloadText.text = !string.IsNullOrEmpty(t.MarkerPayloadString)
            ? $"\"{t.MarkerPayloadString}\""
            : FormatBytes(t.MarkerPayloadBytes);

        line.loop = true; // LineRenderer closes the square for us
    }

    private void Update()
    {
        if (trackable == null) return;

        // tracking state text
        if (trackable.IsTracked != lastTracked)
            trackingText.text = (lastTracked == trackable.IsTracked) ? "Tracked" : "Untracked";

        if (trackable.PlaneRect is not { } r || r == cachedRect)
        {
            return;
        }
        
        cachedRect = r;
        DrawBox();
        MoveCanvas();
    }

    private static string FormatBytes(byte[] bytes)
    {
        if (bytes is null or { Length: 0 }) return "(no payload)";

        const int preview = 16;
        var bitStr = BitConverter
            .ToString(bytes, 0, Math.Min(bytes.Length, preview))
            .Replace('-', ' ');

        return $"Binary([{bitStr}{(bytes.Length > preview ? " …" : "")}], len={bytes.Length})";
    }

    private void DrawBox()
    {
        line.SetPositions(new[]
        {
            new Vector3(cachedRect.xMin, cachedRect.yMin, 0),
            new Vector3(cachedRect.xMax, cachedRect.yMin, 0),
            new Vector3(cachedRect.xMax, cachedRect.yMax, 0),
            new Vector3(cachedRect.xMin, cachedRect.yMax, 0)
        });
    }

    private void MoveCanvas()
    {
        if (!canvas)
        {
            return;
        }
        
        canvas.localPosition = new Vector3(
            cachedRect.center.x * canvas.localScale.x,
            cachedRect.yMin * canvas.localScale.y,
            canvas.localPosition.z);
    }
}