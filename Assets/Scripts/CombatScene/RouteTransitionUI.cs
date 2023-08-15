using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteTransitionUI : MonoBehaviour
{
    public GameObject _allObjects;

    public HorizontalLayoutGroup _routeLayoutGroup;
    public Button _loadEventButton;
    public Button _returnToMenuButton;
    public Image _nextEventImage;

    private RouteNode _routeNodePrefab;
    private RouteNodeConnector _routeConnectorPrefab;
    private GameObject _routePointer;

    private List<RouteNode> _routeNodes = new List<RouteNode>();
    private List<RouteNodeConnector> _routeConnectors = new List<RouteNodeConnector>();

    private void Awake()
    {
        _routeNodePrefab = Resources.Load(GS.UIPrefabs("RouteNode"), typeof (RouteNode)) as RouteNode;
        _routeConnectorPrefab = Resources.Load(GS.UIPrefabs("RouteNodeConnector"), typeof (RouteNodeConnector)) as RouteNodeConnector;
        _returnToMenuButton.onClick.AddListener(() => Loader.Load(Loader.Scene.MenuScene));
    }
    private void Start()
    {
        SpawnRouteNodes(DataStorage.Singleton.playerData);
    }
    private void SpawnRouteNodes(PlayerData data)
    {
        for (int i = 0; i < data.CurrentEventPath.Count; i++)
        {
            if (i != 0)
            {
                RouteNodeConnector tmpConnector = Instantiate(_routeConnectorPrefab, _routeLayoutGroup.transform, false);
                _routeConnectors.Add(tmpConnector);
            }
            RouteNode tmpNode = Instantiate(_routeNodePrefab, _routeLayoutGroup.transform, false);
            _routeNodes.Add(tmpNode);

        }
        int nodeIndex;
        for (nodeIndex = 0; nodeIndex < data.CurrentEventPath.Count;)
        {
            _routeNodes[nodeIndex].ShowNode(true);
            if(nodeIndex > 0) _routeConnectors[nodeIndex-1].ShowConnectors();

            if (nodeIndex == 0)
            {
                _routePointer = Instantiate(Resources.Load(GS.UIPrefabs("RoutePointer"), typeof(GameObject)) as GameObject, transform, false);
                _routePointer.transform.position = _routeNodes[0].transform.position + Vector3.up;
            }

            if (data.CurrentEventPath[nodeIndex]._visited)
            {
                nodeIndex++;
                continue;
            }
            else
            {
                _loadEventButton.onClick.AddListener(() => Loader.Load(Loader.EventNodeToScene(data.CurrentEventPath[data.CurrentEventPathIndex]._event)));
                break;
            }
        }
        StartCoroutine(MoveBetweenNodes(nodeIndex));
    }
    private IEnumerator MoveBetweenNodes(int nodeToEndAt)
    {
        if (nodeToEndAt < 0) yield break;
        if      (DataStorage.Singleton.playerData.CurrentEventPath[nodeToEndAt]._event.Equals(PlayerData.NodeEventType.Combat)) _nextEventImage.sprite = Resources.Load(GS.UIGraphics("Event Nodes/Combat_Node"), typeof (Sprite)) as Sprite;
        else if (DataStorage.Singleton.playerData.CurrentEventPath[nodeToEndAt]._event.Equals(PlayerData.NodeEventType.Shop)) _nextEventImage.sprite = Resources.Load(GS.UIGraphics("Event Nodes/Shop_Node"), typeof(Sprite)) as Sprite;
        else if (DataStorage.Singleton.playerData.CurrentEventPath[nodeToEndAt]._event.Equals(PlayerData.NodeEventType.FreeWizard)) _nextEventImage.sprite = Resources.Load(GS.UIGraphics("Event Nodes/FreeWizard_Node"), typeof(Sprite)) as Sprite;
        else if (DataStorage.Singleton.playerData.CurrentEventPath[nodeToEndAt]._event.Equals(PlayerData.NodeEventType.Construction)) _nextEventImage.sprite = Resources.Load(GS.UIGraphics("Event Nodes/Construction_Node"), typeof(Sprite)) as Sprite;
        else if (DataStorage.Singleton.playerData.CurrentEventPath[nodeToEndAt]._event.Equals(PlayerData.NodeEventType.FreeLoot)) _nextEventImage.sprite = Resources.Load(GS.UIGraphics("Event Nodes/FreeLoot_Node"), typeof(Sprite)) as Sprite;
        else if (DataStorage.Singleton.playerData.CurrentEventPath[nodeToEndAt]._event.Equals(PlayerData.NodeEventType.Dialogue)) _nextEventImage.sprite = Resources.Load(GS.UIGraphics("Event Nodes/Dialogue_Node"), typeof(Sprite)) as Sprite;
        if (nodeToEndAt < 1) yield break;

        _routeNodes[nodeToEndAt].ShowNode(false);
        _routeConnectors[nodeToEndAt - 1]._visitedConnector.gameObject.SetActive(true);
        //_routePointer.transform.position = _routeNodes[nodeToEndAt - 1].transform.position + Vector3.up;


        float position = 0f;
        while (position < 1f)
        {
            position += Time.deltaTime;
            _routeConnectors[nodeToEndAt - 1]._visitedConnector.fillAmount = Mathf.Min(1, position);
            _routePointer.transform.position = Vector3.Lerp(_routeNodes[nodeToEndAt - 1].transform.position,_routeNodes[nodeToEndAt].transform.position, position) + Vector3.up;
            yield return new WaitForFixedUpdate();
        }
        _routeNodes[nodeToEndAt].FadeInNode();
    }
}
