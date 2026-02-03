using UnityEngine;

public class Afterimage_Effect : MonoBehaviour
{
    [Header("잔상 설정")]
    [SerializeField] private Sprite afterimageSprite;
    public AfterImage_Effect_Manager afterimageManager;

    private void OnDisable()
    {
        CreateAfterimage();
    }

    public void CreateAfterimage()
    {
        if (afterimageSprite == null) return;
        afterimageManager.CreateAfterimage(afterimageSprite);
    }
}
