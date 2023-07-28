using System.Collections.Generic;
using UnityEngine;

public class OcclusionCulling2D : MonoBehaviour
{
    [System.Serializable]
    public class ObjectSettings
    {
        [HideInInspector] public string title;
        public GameObject _gameObjectToHide;
        public TilemapChunk _tileMapChunk;

        public Vector2 _size = Vector2.one;
        public Vector2 _offset = Vector2.zero;
        public bool _multiplySizeByTransformScale = true;

        public Vector2 Sized { get; set; }
        public Vector2 Center { get; set; }
        public Vector2 TopRight { get; set; }
        public Vector2 TopLeft { get; set; }
        public Vector2 BottomLeft { get; set; }
        public Vector2 BottomRight { get; set; }
        public float Right { get; set; }
        public float Left { get; set; }
        public float Top { get; set; }
        public float Bottom { get; set; }

        public Color DrawColor = Color.white;
        public bool showBorders = true;

        public void InitObjectSettingProperties()
        {
            Sized = _size * (_multiplySizeByTransformScale ? new Vector2(Mathf.Abs(_gameObjectToHide.transform.localScale.x), Mathf.Abs(_gameObjectToHide.transform.localScale.y)) : Vector2.one);
            Center = (Vector2) _gameObjectToHide.transform.position + _offset;

            TopRight = new Vector2(Center.x + Sized.x, Center.y + Sized.y);
            TopLeft = new Vector2(Center.x - Sized.x, Center.y + Sized.y);
            BottomLeft = new Vector2(Center.x - Sized.x, Center.y - Sized.y);
            BottomRight = new Vector2(Center.x + Sized.x, Center.y - Sized.y);

            Right = Center.x + Sized.x;
            Left = Center.x - Sized.x;
            Top = Center.y + Sized.y;
            Bottom = Center.y - Sized.y;
        }
    }

    public List<ObjectSettings> objectSettings = new List<ObjectSettings>();

    private Camera camComponent;
    private float cameraHalfWidth;

    public float updateRateInSeconds = 0.1f;

    private float timer;
    float cameraRight;
    float cameraLeft;
    float cameraTop;
    float cameraBottom;
    void Awake()
    {
        camComponent = GetComponent<Camera>();
        UpdateCameraWidth();
        /*
        foreach (ObjectSettings o in objectSettings)
        {
            o.sized = o.size * (o.multiplySizeByTransformScale ? new Vector2(Mathf.Abs(o.theGameObject.transform.localScale.x), Mathf.Abs(o.theGameObject.transform.localScale.y)) : Vector2.one);
            o.center = (Vector2)o.theGameObject.transform.position + o.offset;

            o.TopRight = new Vector2(o.center.x + o.sized.x, o.center.y + o.sized.y);
            o.TopLeft = new Vector2(o.center.x - o.sized.x, o.center.y + o.sized.y);
            o.BottomLeft = new Vector2(o.center.x - o.sized.x, o.center.y - o.sized.y);
            o.BottomRight = new Vector2(o.center.x + o.sized.x, o.center.y - o.sized.y);

            o.right = o.center.x + o.sized.x;
            o.left = o.center.x - o.sized.x;
            o.top = o.center.y + o.sized.y;
            o.bottom = o.center.y - o.sized.y;
        }*/
    }

    public void UpdateCameraWidth()
    {
        cameraHalfWidth = camComponent.orthographicSize * ((float)Screen.width / (float)Screen.height);
    }
    void OnDrawGizmosSelected()
    {
        foreach (ObjectSettings o in objectSettings)
        {
            if (o._gameObjectToHide)
            {
                o.title = o._gameObjectToHide.name;

                if (o.showBorders)
                {
                    o.TopRight = new Vector2(o.Center.x + o.Sized.x, o.Center.y + o.Sized.y);
                    o.TopLeft = new Vector2(o.Center.x - o.Sized.x, o.Center.y + o.Sized.y);
                    o.BottomLeft = new Vector2(o.Center.x - o.Sized.x, o.Center.y - o.Sized.y);
                    o.BottomRight = new Vector2(o.Center.x + o.Sized.x, o.Center.y - o.Sized.y);
                    Gizmos.color = o.DrawColor;
                    Gizmos.DrawLine(o.TopRight, o.TopLeft);
                    Gizmos.DrawLine(o.TopLeft, o.BottomLeft);
                    Gizmos.DrawLine(o.BottomLeft, o.BottomRight);
                    Gizmos.DrawLine(o.BottomRight, o.TopRight);
                }
            }
        }
    }

    void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer > updateRateInSeconds) timer = 0;
        else return;

        cameraRight = camComponent.transform.position.x + cameraHalfWidth;
        cameraLeft = camComponent.transform.position.x - cameraHalfWidth;
        cameraTop = camComponent.transform.position.y + camComponent.orthographicSize;
        cameraBottom = camComponent.transform.position.y - camComponent.orthographicSize;

        foreach (ObjectSettings o in objectSettings)
        {
            if (o._gameObjectToHide)
            {
                bool IsObjectVisibleInCastingCamera = o.Right > cameraLeft & o.Left < cameraRight & // check horizontal
                                                      o.Top > cameraBottom & o.Bottom < cameraTop; // check vertical

                if (o._tileMapChunk)
                {
                    if(o._tileMapChunk._shown && !IsObjectVisibleInCastingCamera) o._tileMapChunk.Show(false);
                    else if (!o._tileMapChunk._shown && IsObjectVisibleInCastingCamera) o._tileMapChunk.Show(true);
                }
                else o._gameObjectToHide.SetActive(IsObjectVisibleInCastingCamera);
            }
        }
    }
}