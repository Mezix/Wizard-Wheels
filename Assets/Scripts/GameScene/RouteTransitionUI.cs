using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouteTransitionUI : MonoBehaviour
{
    public GameObject _allObjects;

    public HorizontalLayoutGroup _routeLayoutGroup;
    private int nodesToSpawn;
    public Button _loadEventButton;
    public Image _nextEventImage;

    private RouteNode _routeNodePrefab;
    private RouteNodeConnector _routeConnectorPrefab;
    private GameObject _routePointer;

    private List<RouteNode> _routeNodes = new List<RouteNode>();
    private List<RouteNodeConnector> _routeConnectors = new List<RouteNodeConnector>();

    //  Misc
    private PlayerData playerData;

    private void Awake()
    {
        _routeNodePrefab = Resources.Load(GS.UIPrefabs("RouteNode"), typeof (RouteNode)) as RouteNode;
        _routeConnectorPrefab = Resources.Load(GS.UIPrefabs("RouteNodeConnector"), typeof (RouteNodeConnector)) as RouteNodeConnector;
    }
    private void Start()
    {
        playerData = SavePlayerData.LoadPlayer(0);
        playerData.CurrentEventPath = new List<PlayerData.EventNode>()
        {
           new PlayerData.EventNode(PlayerData.EventType.Combat, true),
           new PlayerData.EventNode(PlayerData.EventType.Construction, true),
           new PlayerData.EventNode(PlayerData.EventType.Shop, false),
           new PlayerData.EventNode(PlayerData.EventType.NewWizard, false),
        };
        SpawnRouteNodes(playerData);
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

            if(i == 0)
            {
                _routePointer = Instantiate(Resources.Load(GS.UIPrefabs("RoutePointer"), typeof(GameObject)) as GameObject, transform, false);
                _routePointer.transform.position = tmpNode.transform.position + Vector3.up;
            }
        }

        int nodeIndex;
        for (nodeIndex = 0; nodeIndex < data.CurrentEventPath.Count;)
        {
            _routeNodes[nodeIndex].ShowNode(true);
            if(nodeIndex > 0) _routeConnectors[nodeIndex-1].ShowConnectors();

            if (data.CurrentEventPath[nodeIndex]._visited)
            {
                nodeIndex++;
                continue;
            }
            else
            {
                if (data.CurrentEventPath[nodeIndex]._event.Equals(PlayerData.EventType.Dialogue)) _loadEventButton.onClick.AddListener(() => Loader.Load(Loader.Scene.EventScene));
                else if (data.CurrentEventPath[nodeIndex]._event.Equals(PlayerData.EventType.FreeLoot)) _loadEventButton.onClick.AddListener(() => Loader.Load(Loader.Scene.EventScene));
                else if (data.CurrentEventPath[nodeIndex]._event.Equals(PlayerData.EventType.NewWizard)) _loadEventButton.onClick.AddListener(() => Loader.Load(Loader.Scene.EventScene));
                else if (data.CurrentEventPath[nodeIndex]._event.Equals(PlayerData.EventType.Shop)) _loadEventButton.onClick.AddListener(() => Loader.Load(Loader.Scene.EventScene));
                else if (data.CurrentEventPath[nodeIndex]._event.Equals(PlayerData.EventType.Construction)) _loadEventButton.onClick.AddListener(() => Loader.Load(Loader.Scene.ConstructionScene));
                else if (data.CurrentEventPath[nodeIndex]._event.Equals(PlayerData.EventType.Combat)) _loadEventButton.onClick.AddListener(() => Loader.Load(Loader.Scene.CombatScene));
                break;
            }
        }
        StartCoroutine(MoveBetweenNodes(nodeIndex));
    }
    private IEnumerator MoveBetweenNodes(int nodeToEndAt)
    {
        if (nodeToEndAt < 0) yield break;
        if      (playerData.CurrentEventPath[nodeToEndAt]._event.Equals(PlayerData.EventType.Combat)) _nextEventImage.sprite = Resources.Load(GS.UIGraphics("Event Nodes/Combat_Node"), typeof (Sprite)) as Sprite;
        else if (playerData.CurrentEventPath[nodeToEndAt]._event.Equals(PlayerData.EventType.Shop)) _nextEventImage.sprite = Resources.Load(GS.UIGraphics("Event Nodes/Shop_Node"), typeof(Sprite)) as Sprite;
        else if (playerData.CurrentEventPath[nodeToEndAt]._event.Equals(PlayerData.EventType.NewWizard)) _nextEventImage.sprite = Resources.Load(GS.UIGraphics("Event Nodes/New_Wizard"), typeof(Sprite)) as Sprite;
        else if (playerData.CurrentEventPath[nodeToEndAt]._event.Equals(PlayerData.EventType.Construction)) _nextEventImage.sprite = Resources.Load(GS.UIGraphics("Event Nodes/Construction_Node"), typeof(Sprite)) as Sprite;
        //else if (playerData.CurrentEventPath[startNode]._event.Equals(PlayerData.EventType.FreeLoot)) _nextEventImage.sprite = Resources.Load(GS.UIGraphics("Event Nodes/FreeLoot_Node"), typeof(Sprite)) as Sprite;
        //else if (playerData.CurrentEventPath[startNode]._event.Equals(PlayerData.EventType.Dialogue)) _nextEventImage.sprite = Resources.Load(GS.UIGraphics("Event Nodes/Dialogue_Node"), typeof(Sprite)) as Sprite;

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
