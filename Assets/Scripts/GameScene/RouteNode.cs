using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteNode : MonoBehaviour
{
    public Image _visitedNode;
    void Start()
    {
        _visitedNode.gameObject.SetActive(false);
    }
    public void FadeInNode()
    {
        StartCoroutine(VisitNode());
    }
    private IEnumerator VisitNode()
    {
        float index = 0;
        _visitedNode.gameObject.SetActive(true);
        while (index < 1)
        {
            index += Time.deltaTime * 2;
            _visitedNode.color = new Color(1, 1, 1, Mathf.Min(1, index));
            yield return new WaitForFixedUpdate();
        }
    }
}
