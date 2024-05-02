using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRule : MonoBehaviour
{
    public GameObject rules;
    public void ShowRules()
    {
        rules.SetActive(true);
    }

    public void HideRules()
    {
        rules.SetActive(false);
    }
}
