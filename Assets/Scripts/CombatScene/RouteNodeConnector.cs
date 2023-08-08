using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteNodeConnector : MonoBehaviour
{
    public Image _visitedConnector;
    void Awake()
    {
        _visitedConnector.gameObject.SetActive(false);
    }
    public void ShowConnectors()
    {
        _visitedConnector.gameObject.SetActive(true);
        _visitedConnector.fillAmount = 1;
    }
}
