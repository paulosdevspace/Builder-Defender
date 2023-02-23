using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionTimerUI : MonoBehaviour
{
    private Image constructionProgressImage;
    [SerializeField] private BuildingConstruction buildingConstruction;

    private void Awake()
    {
        constructionProgressImage = transform.Find("mask").Find("image").GetComponent<Image>();
    }

    private void Update()
    {
        constructionProgressImage.fillAmount = buildingConstruction.GetConstructionTimerNormalized();
    }

}
