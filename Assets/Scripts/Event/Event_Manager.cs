using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Event_Manager : MonoBehaviour
{
    public static Event_Manager instance;

    [Header("Event Data")]
    public EventSceneData currentEvent;

    [Header("UI References")]
    public Image eventImage;
    public TextMeshProUGUI descriptionText;
    public Button[] choiceButtons;
    public Button endButton;
    public TextMeshProUGUI[] choiceTexts;

    void Awake()
    {
        if (instance == null)
            instance = this;
        endButton.gameObject.SetActive(false);
        endButton.onClick.AddListener(() =>
        {
            Stage_Manager.instance.CompleteCurrentNode();
        });
    }

    void Start()
    {
        LoadRandomEvent();
    }

    public void LoadRandomEvent()
    {
        EventSceneData[] events = Resources.LoadAll<EventSceneData>("Events");
        if (events.Length > 0)
        {
            int randomIndex = Random.Range(0, events.Length);
            LoadEvent(events[randomIndex]);
        }
        else
        {
            Debug.LogWarning("Events 폴더에 EventSceneData가 없습니다.");
        }
    }

    public void LoadEvent(EventSceneData eventData)
    {
        currentEvent = eventData;

        if (eventImage != null && eventData.image != null)
            eventImage.sprite = eventData.image;

        if (descriptionText != null)
            descriptionText.text = eventData.description;

        // 선택지 버튼 설정
        for (int i = choiceButtons.Length - 1; i >= 0; i--)
        {
            if (i < eventData.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceTexts[i].text = eventData.choices[i].choiceText;

                int index = i; // 클로저용 로컬 변수
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(index));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
        for(int i = eventData.choices.Length; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
        }
    }

    public void OnChoiceSelected(int choiceIndex)
    {
        if (currentEvent == null || choiceIndex >= currentEvent.choices.Length) return;
        
        for(int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
        }
        descriptionText.text = currentEvent.endDescription;
        eventImage.sprite = currentEvent.endImage;

        EventChoice choice = currentEvent.choices[choiceIndex];
        ApplyEffect(choice.effectType, choice.value);
        endButton.gameObject.SetActive(true);
    }

    private void ApplyEffect(EventEffectType effectType, int value)
    {
        switch (effectType)
        {
            case EventEffectType.HealPercent:
                Party_Manager.instance.HealPartyPercent(value / 100f);
                break;
            case EventEffectType.HealFixed:
                Party_Manager.instance.HealPartyFixed(value);
                break;
            case EventEffectType.DamagePercent:
                Party_Manager.instance.DamagePartyPercent(value / 100f);
                break;
            case EventEffectType.DamageFixed:
                Party_Manager.instance.DamagePartyFixed(value);
                break;
            case EventEffectType.None:
            default:
                break;
        }
    }
}
