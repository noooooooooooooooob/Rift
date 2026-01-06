using UnityEngine;

[System.Serializable]
public class ForcedNode
{
    public int layer;
    public Stage_Type nodeType;
}

[CreateAssetMenu(fileName = "StageMapConfig", menuName = "Scriptable Objects/StageMapConfig")]
public class StageMapConfig : ScriptableObject
{
    [Header("Map Structure")]
    public int totalLayers;
    public int nodesPerLayer;
    [Header("Layer Probabilities")]
    public LayerProbability[] layerProbabilities;

    [System.Serializable]
    public class LayerProbability
    {
        [Range(1, 10)] public int layerIndex = 1;

        [Header("Node Type Weights")]
        [Range(0f, 100f)] public float battleWeight = 50f;
        [Range(0f, 100f)] public float shopWeight = 25f;
        [Range(0f, 100f)] public float eventWeight = 25f;

        [Header("Preview")]
        public string totalWeight; // Inspector용 표시

        [Header("Forced Nodes")]
        public ForcedNode[] forcedNodes;
        public Stage_Type GetRandomNodeType()
        {
            float total = battleWeight + shopWeight + eventWeight;
            float rand = Random.Range(0f, total);

            if (rand < battleWeight)
                return Stage_Type.BATTLE;
            else if (rand < battleWeight + shopWeight)
                return Stage_Type.SHOP;
            else
                return Stage_Type.EVENT;
        }

        // Inspector에서 확률 표시용
        public void UpdatePreview()
        {
            float total = battleWeight + shopWeight + eventWeight;
            if (total > 0)
            {
                float battlePercent = (battleWeight / total) * 100f;
                float shopPercent = (shopWeight / total) * 100f;
                float eventPercent = (eventWeight / total) * 100f;

                totalWeight = $"전투:{battlePercent:F1}% 상점:{shopPercent:F1}% 이벤트:{eventPercent:F1}%";
            }
        }
    }

    public Stage_Type GetNodeTypeForLayer(int layer)
    {
        // 해당 레이어의 확률 찾기
        foreach (var prob in layerProbabilities)
        {
            if (prob.layerIndex == layer)
                return prob.GetRandomNodeType();
        }

        // 기본값
        return Stage_Type.BATTLE;
    }
}
