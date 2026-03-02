using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public List<GameObject> tutorialSteps;
    private int currentStepIndex = 0;
    void Start()
    {
        ShowCurrentStep();
    }

    private void ShowCurrentStep()
    {
        for (int i = 0; i < tutorialSteps.Count; i++)
        {
            tutorialSteps[i].SetActive(i == currentStepIndex);
        }
    }
    public void NextStep()
    {
        if (currentStepIndex < tutorialSteps.Count - 1)
        {
            currentStepIndex++;
            ShowCurrentStep();
        }
    }

    public void PreviousStep()
    {
        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            ShowCurrentStep();
        }
    }
}