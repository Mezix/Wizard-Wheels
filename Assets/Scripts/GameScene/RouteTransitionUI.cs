using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteTransitionUI : MonoBehaviour
{
    public HorizontalLayoutGroup _routeLayoutGroup;
    private int nodesToSpawn;
    public Button _loadEventButton;

    private RouteNode _routeNodePrefab;
    private RouteNodeConnector _routeConnectorPrefab;
    private GameObject _routePointer;

    private List<RouteNode> _routeNodes = new List<RouteNode>();
    private List<RouteNodeConnector> _routeConnectors = new List<RouteNodeConnector>();
    private void Awake()
    {
        _routeNodePrefab = Resources.Load(GS.UIPrefabs("RouteNode"), typeof (RouteNode)) as RouteNode;
        _routeConnectorPrefab = Resources.Load(GS.UIPrefabs("RouteNodeConnector"), typeof (RouteNodeConnector)) as RouteNodeConnector;
        _routePointer = Instantiate(Resources.Load(GS.UIPrefabs("RoutePointer"), typeof (GameObject)) as GameObject, transform, false);
        _routePointer.SetActive(false);

        _loadEventButton.onClick.AddListener(() => LoadEvent());
    }

    private void Start()
    {
        nodesToSpawn = 5;
        SpawnRoute();
        InitializeNodes(3);
        StartCoroutine(MoveBetweenNodes(2));
    }
    private void SpawnRoute()
    {
        for(int i = 0; i < nodesToSpawn; i++)
        {
            if (i != 0)
            {
                RouteNodeConnector tmpConnector = Instantiate(_routeConnectorPrefab, _routeLayoutGroup.transform, false);
                _routeConnectors.Add(tmpConnector);
            }
            RouteNode tmpNode = Instantiate(_routeNodePrefab, _routeLayoutGroup.transform, false);
            _routeNodes.Add(tmpNode);
        }
    }
    private void InitializeNodes(int lastNodeToInit)
    {
        for(int i = 0; i < lastNodeToInit; i++)
        {
            _routeNodes[i].ShowNode();
            _routeConnectors[i].ShowConnectors();
        }
    }

    private IEnumerator MoveBetweenNodes(int startNode)
    {
        float position = 0f;
        _routeConnectors[startNode]._visitedConnector.gameObject.SetActive(true);
        _routePointer.transform.position = _routeNodes[startNode].transform.position + Vector3.up;
        _routePointer.SetActive(true);
        while (position < 1f)
        {
            position += Time.deltaTime;
            _routeConnectors[startNode]._visitedConnector.fillAmount = Mathf.Min(1, position);
            _routePointer.transform.position = Vector3.Lerp(_routeNodes[startNode].transform.position,_routeNodes[startNode+1].transform.position, position) + Vector3.up;
            yield return new WaitForFixedUpdate();
        }
        _routeNodes[startNode+1].FadeInNode();
    }
    private void LoadEvent()
    {
        Loader.Load(Loader.Scene.CombatScene);
    }
}
