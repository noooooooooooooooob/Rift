using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage_Manager : MonoBehaviour
{
    public static Stage_Manager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Stage Select Scene")  // 씬 이름 확인
        {
            if(mapContainer != null)
            ShowMap();
        }
        else
        {
            HideMap();
        }
    }
    [Header("Runtime")]
    public Stage_Node currentStage;
    public List<Stage_Node> allStages = new List<Stage_Node>();

    [Header("Map Configuration")]
    public int totalLayers = 5; // 총 5개 레이어 (마지막은 보스)
    public int nodesPerLayer = 3; // 레이어당 노드 수
    public float layerSpacing = 5f; // 레이어 간 간격
    public float nodeSpacing = 3f; // 같은 레이어 내 노드 간격

    [Header("Visuals")]
    private Transform mapContainer;  // 자동 생성됨
    public GameObject nodePrefab;
    public LineRenderer connectionLinePrefab;
    public Sprite battleIcon;
    public Sprite shopIcon;
    public Sprite eventIcon;
    public Sprite restIcon;
    public Sprite bossIcon;
    public Sprite startIcon;
    public Color lockedColor = Color.gray;
    public Color unlockedColor = Color.white;
    public Color completedColor = Color.green;
    public Color currentColor = Color.yellow;
    public void ShowMap()
    {
        mapContainer.gameObject.SetActive(true);
    }

    public void HideMap()
    {
        mapContainer.gameObject.SetActive(false);
    }

    public void SelectNode(Stage_Node newStage)
    {
        switch (newStage.nodeType)
        {
            case Stage_Type.BATTLE:
                Debug.Log("Battle Node Selected");
                SceneManager.LoadScene("Battle Scene");
                break;
            case Stage_Type.SHOP:
                Debug.Log("Shop Node Selected");
                SceneManager.LoadScene("Shop Scene");
                break;
            case Stage_Type.EVENT:
                Debug.Log("Event Node Selected");
                SceneManager.LoadScene("Event Scene");
                break;
            case Stage_Type.REST:
                Debug.Log("Rest Node Selected");
                SceneManager.LoadScene("Rest Scene");
                break;
            case Stage_Type.BOSS:
                Debug.Log("Boss Node Selected");
                // SceneManager.LoadScene("Boss Scene");
                SceneManager.LoadScene("Battle Scene");
                break;
        }
        currentStage.isCurrentNode = false;
        currentStage.UpdateVisual();
        currentStage = newStage;
        foreach (Stage_Node node in allStages)
        {
            if (node.gridPosition.x == currentStage.gridPosition.x)
            {
                node.isCompleted = true;
                node.isUnlocked = false;
                node.UpdateVisual();
            }
        }
        currentStage.isCurrentNode = true;
        currentStage.isCompleted = false;
        currentStage.UpdateVisual();
    }

    void Start()
    {
        CreateMapContainer();
        GenerateMap();
    }

    void CreateMapContainer()
    {
        GameObject container = new GameObject("MapContainer");
        container.transform.SetParent(transform);
        container.transform.localPosition = Vector3.zero;
        mapContainer = container.transform;
    }

    void GenerateMap()
    {
        allStages.Clear();

        // Layer 0: Start Node
        Stage_Node startNode = CreateNode(Stage_Type.START, 0, 1);
        startNode.isCompleted = true;
        startNode.isCurrentNode = true;
        startNode.UpdateVisual();
        currentStage = startNode;

        List<Stage_Node> previousLayer = new List<Stage_Node> { startNode };
        // Layer 1~totalLayers-1
        for (int layer = 1; layer < totalLayers; layer++)
        {
            List<Stage_Node> currentLayer = new List<Stage_Node>();

            // 마지막 레이어: 보스
            if (layer == totalLayers - 1)
            {
                Stage_Node bossNode = CreateNode(Stage_Type.BOSS, layer, 1);
                currentLayer.Add(bossNode);
            }
            // 보스 전 레이어: 휴식 (노드 1개만)
            else if (layer == totalLayers - 2)
            {
                Stage_Node restNode = CreateNode(Stage_Type.REST, layer, 1);
                currentLayer.Add(restNode);
            }
            // 나머지: 랜덤 노드
            else
            {
                for (int i = 0; i < nodesPerLayer; i++)
                {
                    Stage_Type randomType = GetRandomNodeType();
                    Stage_Node node = CreateNode(randomType, layer, i);
                    currentLayer.Add(node);
                }
            }

            // 이전 레이어와 연결
            ConnectLayers(previousLayer, currentLayer);
            previousLayer = currentLayer;
        }
    }

    Stage_Node CreateNode(Stage_Type type, int layer, int index)
    {
        // 위치 계산
        Vector3 position = CalculateNodePosition(layer, index);

        GameObject nodeObj = Instantiate(nodePrefab, position, Quaternion.identity, mapContainer);
        Stage_Node node = nodeObj.GetComponent<Stage_Node>();

        node.nodeType = type;
        node.gridPosition = new Vector2(layer, index);
        node.SetVisual(type);

        allStages.Add(node);
        return node;
    }

    Vector3 CalculateNodePosition(int layer, int index)
    {
        float x = layer * layerSpacing;

        // 같은 레이어 내 노드들을 중앙 정렬
        float totalWidth = (nodesPerLayer - 1) * nodeSpacing;
        float startY = -totalWidth / 2f;
        float y = startY + (index * nodeSpacing);

        return new Vector3(x, y, 0);
    }

    void ConnectLayers(List<Stage_Node> prevLayer, List<Stage_Node> currLayer)
    {
        if (prevLayer.Count == 1)
        {
            foreach (var currNode in currLayer)
            {
                CreateConnection(prevLayer[0], currNode);
                // 시작 노드에서만 다음 레이어 unlock
                if (prevLayer[0].nodeType == Stage_Type.START)
                {
                    currNode.isUnlocked = true;
                    currNode.UpdateVisual();
                }
            }
            return;
        }
        if (currLayer.Count == 1)
        {
            foreach (var prevNode in prevLayer)
            {
                CreateConnection(prevNode, currLayer[0]);
            }
            return;
        }

        // Step 1: prevLayer의 모든 노드가 최소 1개 연결을 보내도록 보장
        foreach (var prevNode in prevLayer)
        {
            int connectionCount = Random.Range(1, 3); // 1~2개 연결
            List<Stage_Node> shuffled = new List<Stage_Node>(currLayer);

            // 셔플
            for (int i = 0; i < shuffled.Count; i++)
            {
                int rand = Random.Range(i, shuffled.Count);
                var temp = shuffled[i];
                shuffled[i] = shuffled[rand];
                shuffled[rand] = temp;
            }

            for (int i = 0; i < Mathf.Min(connectionCount, shuffled.Count); i++)
            {
                CreateConnection(prevNode, shuffled[i]);
            }
        }

        // Step 2: currLayer에서 연결이 없는 노드가 있으면 연결 추가
        foreach (var currNode in currLayer)
        {
            if (!HasIncomingConnection(currNode, prevLayer))
            {
                // 랜덤으로 prevLayer 노드 선택해서 연결
                Stage_Node randomPrev = prevLayer[Random.Range(0, prevLayer.Count)];
                CreateConnection(randomPrev, currNode);
            }
        }
    }

    bool HasIncomingConnection(Stage_Node targetNode, List<Stage_Node> prevLayer)
    {
        foreach (var prevNode in prevLayer)
        {
            foreach (var connection in prevNode.nextNodes)
            {
                if (connection.targetNode == targetNode)
                    return true;
            }
        }
        return false;
    }

    void CreateConnection(Stage_Node fromNode, Stage_Node toNode)
    {
        LineRenderer line = Instantiate(connectionLinePrefab, mapContainer);
        line.SetPosition(0, fromNode.transform.position);
        line.SetPosition(1, toNode.transform.position);

        NodeConnection connection = new NodeConnection
        {
            targetNode = toNode,
            connectionLine = line
        };

        fromNode.nextNodes.Add(connection);
    }
    Stage_Type GetRandomNodeType()
    {
        int rand = Random.Range(0, 10);

        if (rand < 7)
            return Stage_Type.BATTLE;
        else if (rand < 9)
            return Stage_Type.EVENT;
        else
            return Stage_Type.SHOP;
    }
    public void CompleteCurrentNode()
    {
        if (currentStage != null)
        {
            foreach (Stage_Node node in allStages)
            {
                if (node.gridPosition.x == currentStage.gridPosition.x)
                {
                    node.isUnlocked = false;
                    node.UpdateVisual();
                }
            }
            foreach (var connection in currentStage.nextNodes)
            {
                connection.targetNode.isUnlocked = true;
                connection.targetNode.UpdateVisual();
            }
        }
    }
}