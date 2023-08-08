using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteNode : MonoBehaviour
{
    public Image _visitedNode;
    void Awake()
    {
        _visitedNode.gameObject.SetActive(false);
    }
    public void ShowNode(bool show)
    {
        _visitedNode.gameObject.SetActive(show);
        _visitedNode.color = new Color(1, 1, 1, 1);
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
