using Meta.XR.MRUtilityKit;
using UnityEngine;

public class QrCodeManager : MonoBehaviour
{
    private static QrCodeManager _instance;

    [SerializeField] private QrCode qrCodePrefab;

    private int _activeCount;
    public static int ActiveTrackedCount => _instance?._activeCount ?? 0;
    private static bool IsSupported => OVRAnchor.TrackerConfiguration.QRCodeTrackingSupported;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"Duplicate {nameof(QrCodeManager)} on {name}. Destroying.");
            Destroy(this);
            return;
        }

        _instance = this;

        if (IsSupported)
        {
            return;
        }

        Debug.LogError("QR-code tracking not supported on this device / OS.");
        enabled = false;
    }

    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }

    public void OnTrackableAdded(MRUKTrackable trackable)
    {
        if (trackable.TrackableType != OVRAnchor.TrackableType.QRCode) return;

        if (qrCodePrefab == null)
        {
            Debug.LogError("QrCode prefab not assigned.");
            return;
        }

        var qr = Instantiate(qrCodePrefab, trackable.transform);
        qr.Initialize(trackable);
        _activeCount++;
    }

    public void OnTrackableRemoved(MRUKTrackable trackable)
    {
        if (trackable.TrackableType != OVRAnchor.TrackableType.QRCode) return;
        _activeCount--;
    }
}