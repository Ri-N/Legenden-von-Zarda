using UnityEngine;
using UnityEngine.UI;

public class BackgroundCover : MonoBehaviour
{
    [Tooltip("If set, this camera is used. Otherwise Camera.main is used.")]
    [SerializeField] private Camera targetCamera;


    private Image _image;
    private RectTransform _rt;
    private RectTransform _parentRt;


    private SpriteRenderer _sr;

    private Vector2 _lastParentSize;
    private float _lastCamAspect;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _rt = GetComponent<RectTransform>();
        _parentRt = _rt != null ? _rt.parent as RectTransform : null;

        _sr = GetComponent<SpriteRenderer>();

        if (_image != null)
        {
            _image.preserveAspect = false;

            if (_rt != null)
            {
                _rt.anchorMin = _rt.anchorMax = new Vector2(0.5f, 0.5f);
                _rt.pivot = new Vector2(0.5f, 0.5f);
            }
        }
    }

    private void Start()
    {
        Fit();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Fit();
    }
#endif

    private void Update()
    {
        if (NeedsRefit())
        {
            Fit();
        }
    }

    private bool NeedsRefit()
    {
        Camera cam = targetCamera != null ? targetCamera : Camera.main;

        if (_image != null && _parentRt != null)
        {
            Vector2 parentSize = _parentRt.rect.size;
            if (parentSize != _lastParentSize) return true;
        }

        if (cam != null)
        {
            if (!Mathf.Approximately(cam.aspect, _lastCamAspect)) return true;
        }

        return false;
    }

    private void Fit()
    {
        Camera cam = targetCamera != null ? targetCamera : Camera.main;


        if (_image != null && _rt != null)
        {
            Sprite sprite = _image.sprite;
            if (sprite == null) return;


            float targetW;
            float targetH;
            if (_parentRt != null)
            {
                Vector2 parentSize = _parentRt.rect.size;
                targetW = parentSize.x;
                targetH = parentSize.y;
                _lastParentSize = parentSize;
            }
            else
            {
                targetW = Screen.width;
                targetH = Screen.height;
                _lastParentSize = new Vector2(targetW, targetH);
            }


            float spriteW = sprite.rect.width;
            float spriteH = sprite.rect.height;
            if (spriteW <= 0f || spriteH <= 0f) return;


            float scale = Mathf.Max(targetW / spriteW, targetH / spriteH);

            float newW = spriteW * scale;
            float newH = spriteH * scale;

            _rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newW);
            _rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newH);


            _rt.anchoredPosition = Vector2.zero;


            if (cam != null) _lastCamAspect = cam.aspect;

            return;
        }


        if (_sr != null)
        {
            if (_sr.sprite == null) return;
            if (cam == null) return;


            float worldScreenHeight = cam.orthographicSize * 2f;
            float worldScreenWidth = worldScreenHeight * cam.aspect;

            Vector2 spriteSize = _sr.sprite.bounds.size;
            float scale = Mathf.Max(worldScreenWidth / spriteSize.x, worldScreenHeight / spriteSize.y);

            transform.localScale = new Vector3(scale, scale, 1f);
            transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, transform.position.z);

            _lastCamAspect = cam.aspect;
        }
    }
}
