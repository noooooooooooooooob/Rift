using UnityEngine;

public class Active_Point : MonoBehaviour
{
    public GameObject unCountImage;
    public GameObject countImage;
    public void SetActivePoint(bool isCounted)
    {
        if (isCounted)
        {
            countImage.SetActive(true);
            unCountImage.SetActive(false);
        }
        else
        {
            countImage.SetActive(false);
            unCountImage.SetActive(true);
        }
    }
}