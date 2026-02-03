using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// VFX 이펙트를 전역적으로 관리하는 싱글톤 매니저
/// 오브젝트 풀링을 사용하여 파티클 효과를 재활용
/// </summary>
public class VFX_Manager : MonoBehaviour
{
    private static VFX_Manager instance;
    public static VFX_Manager Instance
    {
        get
        {
            return instance;
        }
    }

    [Header("Preload Effects")]
    [SerializeField] private GameObject[] preloadEffects;

    [Header("Pool Settings")]
    [SerializeField] private int initialPoolSize = 3;

    [Header("Damage Text")]
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private int damageTextPoolSize = 15;
    private Queue<DamageText> damageTextPool;
    private Transform damageTextContainer;

    // 이펙트 프리팹 딕셔너리
    private Dictionary<string, GameObject> effectsDictionary;

    // 오브젝트 풀
    private Dictionary<string, Queue<GameObject>> effectPools;

    // 풀 컨테이너
    private Transform poolContainer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        effectsDictionary = new Dictionary<string, GameObject>();
        effectPools = new Dictionary<string, Queue<GameObject>>();

        // 풀 컨테이너 생성
        poolContainer = new GameObject("VFXPool").transform;
        poolContainer.SetParent(transform);

        // 프리로드 이펙트 등록 및 풀 초기화
        foreach (GameObject effect in preloadEffects)
        {
            if (effect == null) continue;

            effectsDictionary.Add(effect.name, effect);
            effectPools.Add(effect.name, new Queue<GameObject>());

            // 초기 풀 생성
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreatePooledEffect(effect.name);
            }
        }

        // 대미지 텍스트 풀 초기화
        InitializeDamageTextPool();
    }

    private void InitializeDamageTextPool()
    {
        if (damageTextPrefab == null) return;

        damageTextPool = new Queue<DamageText>();
        damageTextContainer = new GameObject("DamageTextPool").transform;
        damageTextContainer.SetParent(transform);

        for (int i = 0; i < damageTextPoolSize; i++)
        {
            CreateDamageText();
        }
    }

    private DamageText CreateDamageText()
    {
        GameObject obj = Instantiate(damageTextPrefab, damageTextContainer);
        DamageText damageText = obj.GetComponent<DamageText>();
        damageText.Initialize(this);
        obj.SetActive(false);
        damageTextPool.Enqueue(damageText);
        return damageText;
    }

    /// <summary>
    /// 대미지 텍스트 표시
    /// </summary>
    public void PlayDamageText(Vector3 position, int value, DamageTextType type)
    {
        if (damageTextPool == null) return;

        // 풀에서 가져오기
        if (damageTextPool.Count == 0)
        {
            CreateDamageText();
        }

        DamageText damageText = damageTextPool.Dequeue();
        damageText.transform.SetParent(null);
        damageText.transform.position = position;
        damageText.gameObject.SetActive(true);
        damageText.Play(value, type);
    }

    /// <summary>
    /// 대미지 텍스트 풀로 반환
    /// </summary>
    public void ReturnDamageText(DamageText damageText)
    {
        if (damageText == null) return;

        damageText.gameObject.SetActive(false);
        damageText.transform.SetParent(damageTextContainer);
        damageTextPool.Enqueue(damageText);
    }

    private GameObject CreatePooledEffect(string effectName)
    {
        if (!effectsDictionary.TryGetValue(effectName, out GameObject prefab))
        {
            Debug.LogError($"[VFX_Manager] {effectName}이 존재하지 않습니다.");
            return null;
        }

        GameObject obj = Instantiate(prefab, poolContainer);
        obj.name = effectName;

        // 자동 반환 컴포넌트 추가
        TemporaryVFXPlayer player = obj.GetComponent<TemporaryVFXPlayer>();
        if (player == null)
        {
            player = obj.AddComponent<TemporaryVFXPlayer>();
        }
        player.Initialize(this, effectName);

        obj.SetActive(false);
        effectPools[effectName].Enqueue(obj);

        return obj;
    }

    private GameObject GetFromPool(string effectName)
    {
        if (!effectPools.ContainsKey(effectName))
        {
            Debug.LogError($"[VFX_Manager] {effectName} 풀이 존재하지 않습니다.");
            return null;
        }

        // 풀에 오브젝트가 없으면 새로 생성
        if (effectPools[effectName].Count == 0)
        {
            CreatePooledEffect(effectName);
        }

        GameObject obj = effectPools[effectName].Dequeue();
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 풀로 이펙트 반환
    /// </summary>
    public void ReturnToPool(string effectName, GameObject effect)
    {
        if (!effectPools.ContainsKey(effectName))
        {
            Destroy(effect);
            return;
        }

        effect.SetActive(false);
        effect.transform.SetParent(poolContainer);
        effectPools[effectName].Enqueue(effect);
    }

    /// <summary>
    /// 지정된 위치에 이펙트 재생
    /// </summary>
    public GameObject Play(string effectName, Vector3 position)
    {
        GameObject effect = GetFromPool(effectName);
        if (effect == null) return null;

        effect.transform.SetParent(null);
        effect.transform.position = position;

        TemporaryVFXPlayer player = effect.GetComponent<TemporaryVFXPlayer>();
        player?.Play();

        return effect;
    }

    /// <summary>
    /// 부모 Transform에 이펙트 재생
    /// </summary>
    public GameObject Play(string effectName, Transform parent, Vector3 localPosition = default)
    {
        GameObject effect = GetFromPool(effectName);
        if (effect == null) return null;

        effect.transform.SetParent(parent);
        effect.transform.localPosition = localPosition;

        TemporaryVFXPlayer player = effect.GetComponent<TemporaryVFXPlayer>();
        player?.Play();

        return effect;
    }

    /// <summary>
    /// 이펙트를 즉시 중지하고 풀로 반환
    /// </summary>
    public void Stop(GameObject effect)
    {
        if (effect == null) return;

        TemporaryVFXPlayer player = effect.GetComponent<TemporaryVFXPlayer>();
        if (player != null)
        {
            player.Stop();
        }
        else
        {
            Destroy(effect);
        }
    }
}
