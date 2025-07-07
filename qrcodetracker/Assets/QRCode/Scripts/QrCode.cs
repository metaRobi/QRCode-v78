using Meta.XR.MRUtilityKit;
using UnityEngine.UI;
using UnityEngine;
using System;

public sealed class QrCode : MonoBehaviour
{
    [SerializeField] private Text payloadText, trackingText;
    [SerializeField] private LineRenderer line;
    [SerializeField] private RectTransform canvas;

    private MRUKTrackable _trackable;
    private Rect _cachedRect;
    private bool _lastTracked;

    public void Initialize(MRUKTrackable trackedQrCode)
    {
        _trackable = trackedQrCode;
        payloadText.text = !string.IsNullOrEmpty(trackedQrCode.MarkerPayloadString)
            ? $"\"{trackedQrCode.MarkerPayloadString}\""
            : FormatBytes(trackedQrCode.MarkerPayloadBytes);

        line.loop = true;
    }

    private void Update()
    {
        if (!_trackable)
        {
            return;
        }

        trackingText.text = _trackable.IsTracked ? "Tracked" : "Untracked";

        if (_trackable.PlaneRect is not { } r || r == _cachedRect)
        {
            return;
        }

        _cachedRect = r;
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
            new Vector3(_cachedRect.xMin, _cachedRect.yMin, 0),
            new Vector3(_cachedRect.xMax, _cachedRect.yMin, 0),
            new Vector3(_cachedRect.xMax, _cachedRect.yMax, 0),
            new Vector3(_cachedRect.xMin, _cachedRect.yMax, 0)
        });
    }

    private void MoveCanvas()
    {
        if (!canvas)
        {
            return;
        }

        canvas.localPosition = new Vector3(
            _cachedRect.center.x * canvas.localScale.x,
            _cachedRect.yMin * canvas.localScale.y,
            canvas.localPosition.z);
    }
}