using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DottedLine
{
    public class DottedLine : MonoBehaviour
    {
        // Inspector fields
        public Sprite Dot;
        public Sprite EndDot;
        [Range(0.01f, 1f)]
        public float Size;
        [Range(0.01f, 2f)]
        public float Delta;

        //Static Property with backing field
        private static DottedLine instance;
        public static DottedLine Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<DottedLine>();
                return instance;
            }
        }

        //Utility fields
        List<Vector3> positions = new List<Vector3>();
        private List<GameObject> dots = new List<GameObject>();
        private List<GameObject> dotPool = new List<GameObject>(); //where all our different bullets are stored
        private List<float> zDirections = new List<float>();
        private List<Color> colors = new List<Color>();

        //  UI Fields
        public List<Vector3> uiPositions = new List<Vector3>();
        private List<GameObject> uiDots = new List<GameObject>();
        private List<GameObject> uiDotPool = new List<GameObject>(); //where all our different bullets are stored
        private List<float> uiZDirections = new List<float>();
        private List<Color> uiColors = new List<Color>();

        private void Start()
        {
            GrowPool();
            GrowUIPool();
        }
        void FixedUpdate()
        {
            if(Time.timeScale > 0)
            {
                if (positions.Count > 0)
                {
                    HideDots();
                }
                if (uiPositions.Count > 0)
                {
                    HideUIDots();
                }
            }
        }


        // Sprite Dots

        private void HideDots()
        {
            foreach(GameObject dot in dots)
            {
                AddToPool(dot);
            }
            dots.Clear();
            positions.Clear();
            zDirections.Clear();
            colors.Clear();
        }
        public void DrawDottedLine(Vector3 start, Vector3 end, Color c, Sprite sp)
        {
            Vector3 point = end;
            Vector3 dir = (start - end).normalized;

            while ((start - end).magnitude > (point - end).magnitude)
            {
                positions.Add(point);
                zDirections.Add(HM.GetAngle2DBetween(end, start));
                colors.Add(c);
                point += (dir * Delta);
            }
            Render(sp);
        }
        private void Render(Sprite sp = null)
        {
            int length = positions.Count;
            for (int i = 0; i < length; i++)
            {
                var g = GetDotFromPool();
                Vector3 vec3d = new Vector3(positions[i].x, positions[i].y, 1);
                g.transform.position = vec3d;
                g.transform.localScale = Vector3.one * Size;
                if(sp != null) g.GetComponent<SpriteRenderer>().sprite = sp;
                g.GetComponent<SpriteRenderer>().color = colors[i];
                HM.RotateLocalTransformToAngle(g.transform, new Vector3(0, 0, zDirections[i]));
                dots.Add(g);
            }
        }

        //  Pooling

        private GameObject CreateDot()
        {
            var go = new GameObject();
            go.transform.localScale = Vector3.one * Size;
            go.transform.parent = transform;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Effects";
            sr.sprite = Dot;
            return go;
        }
        private void GrowPool()
        {
            for (int i = 0; i < 500; i++)
            {
                var dot = CreateDot(); //create more of the prefab
                dot.transform.SetParent(transform); //add all the created bullets to the pool so they aren't loose in the scene
                AddToPool(dot);
            }
        }
        public void AddToPool(GameObject dot)
        {
            dot.SetActive(false); //disable the gameobjects so we don't need to see them
            dotPool.Add(dot); //add the projectile to the entire pool
        }
        public GameObject GetDotFromPool() //take the tag of the prefab we need, and return an instance of it
        {
            for (int i = 0; i < dotPool.Count; i++) //go through the entire pool of all bullets to find one we need
            {
                if (!dotPool[i].activeInHierarchy) //only pull objects which are currently inactive, and have the same tags
                {
                    dotPool[i].SetActive(true); //if we found the correct bullet, activate it
                    return dotPool[i];          //...and now return it!
                }
            }

            //this is only called if we havent found a correct bullet
            GrowPool();
            return GetDotFromPool(); // now get the new projectile, since we created some
        }

        //  UI Dots

        private void HideUIDots()
        {
            foreach (GameObject dot in uiDots)
            {
                AddToUIPool(dot);
            }
            uiDots.Clear();
            uiPositions.Clear();
            uiZDirections.Clear();
            uiColors.Clear();
        }
        public void DrawDottedUILine(Vector3 start, Vector3 end, Color c, Sprite sp)
        {
            Vector3 point = end;
            Vector3 dir = (start - end).normalized;

            while ((start - end).magnitude > (point - end).magnitude)
            {
                uiPositions.Add(point);
                uiZDirections.Add(HM.GetAngle2DBetween(end, start));
                uiColors.Add(c);
                point += (dir * 45);
            }
            RenderUI(sp);
        }

        private void RenderUI(Sprite sp = null)
        {
            int length = uiPositions.Count;
            for (int i = 0; i < length; i++)
            {
                var g = GetDotFromUIPool();
                Vector3 vec3d = new Vector3(uiPositions[i].x, uiPositions[i].y, 1);
                RectTransform rect = g.GetComponent<RectTransform>();
                rect.anchoredPosition = vec3d;

                Image img = g.GetComponent<Image>();
                if (sp != null)
                {
                    img.sprite = sp;
                    rect.sizeDelta = new Vector2(sp.rect.width, sp.rect.height);
                    rect.transform.localScale = Vector3.one * 2;
                }
                img.color = uiColors[i];
                HM.RotateLocalTransformToAngle(g.transform, new Vector3(0, 0, uiZDirections[i]));
                uiDots.Add(g);
            }
        }

        //  Pooling

        private GameObject CreateUIDot()
        {
            var go = new GameObject();
            //go.transform.localScale = Vector3.one * Size;
            go.transform.parent = Ref.UI._dottedLinesParent;

            var sr = go.AddComponent<Image>();
            sr.sprite = Dot;
            return go;
        }

        private void GrowUIPool()
        {
            for (int i = 0; i < 500; i++)
            {
                var dot = CreateUIDot(); //create more of the prefab
                dot.transform.SetParent(Ref.UI._dottedLinesParent); //add all the created bullets to the pool so they aren't loose in the scene
                AddToUIPool(dot);
            }
        }
        public void AddToUIPool(GameObject dot)
        {
            dot.SetActive(false); //disable the gameobjects so we don't need to see them
            uiDotPool.Add(dot); //add the projectile to the entire pool
        }
        public GameObject GetDotFromUIPool() //take the tag of the prefab we need, and return an instance of it
        {
            for (int i = 0; i < uiDotPool.Count; i++) //go through the entire pool of all bullets to find one we need
            {
                if (!uiDotPool[i].activeInHierarchy) //only pull objects which are currently inactive, and have the same tags
                {
                    uiDotPool[i].SetActive(true); //if we found the correct bullet, activate it
                    return uiDotPool[i];          //...and now return it!
                }
            }

            //this is only called if we havent found a correct bullet
            GrowUIPool();
            return GetDotFromUIPool(); // now get the new projectile, since we created some
        }
    }
}