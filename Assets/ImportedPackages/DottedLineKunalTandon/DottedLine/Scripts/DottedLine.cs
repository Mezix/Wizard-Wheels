using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DottedLine
{
    public class DottedLine : MonoBehaviour
    {
        // Inspector fields
        public Sprite Dot;
        [Range(0.01f, 1f)]
        public float Size;
        [Range(0.1f, 2f)]
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

        private void Start()
        {
            GrowPool();
        }
        // Update is called once per frame
        void FixedUpdate()
        {
            if (positions.Count > 0)
            {
                HideDots();
            }
        }
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
        public void DrawDottedLine(Vector3 start, Vector3 end, Color c)
        {
            //HideDots();

            Vector3 point = start;
            Vector3 dir = (end - start).normalized;

            while ((end - start).magnitude > (point - start).magnitude)
            {
                positions.Add(point);
                zDirections.Add(HM.GetAngle2DBetween(start, end));
                colors.Add(c);
                point += (dir * Delta);
            }
            Render();
        }
        private void Render()
        {
            int length = positions.Count;
            for (int i = 0; i < length; i++)
            {
                var g = GetDotFromPool();
                Vector3 vec3d = new Vector3(positions[i].x, positions[i].y, 1);
                g.transform.position = vec3d;
                g.GetComponent<SpriteRenderer>().color = colors[i];
                HM.RotateLocalTransformToAngle(g.transform, new Vector3(0, 0, zDirections[i]));
                dots.Add(g);
            }
        }

        //  Pooling
        private GameObject CreateDot()
        {
            var gameObject = new GameObject();
            gameObject.transform.localScale = Vector3.one * Size;
            gameObject.transform.parent = transform;

            var sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Effects";
            sr.sprite = Dot;
            return gameObject;
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
    }
}