using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class BuildingTypeSelectUI : MonoBehaviour
{
    [SerializeField] private Sprite arrowSprite;
    [SerializeField] private List<BuildingTypeSO> ignoreBuildingTypeList;

    private Transform arrowBtn;

    private Dictionary<BuildingTypeSO, Transform> btnTransformDictionary;

    private void Awake()
    {
        Transform btnTemplate = transform.Find("btnTemplate");
        btnTemplate.gameObject.SetActive(false);

        BuildingTypeListSO buildingTypeList = Resources.Load<BuildingTypeListSO>(typeof(BuildingTypeListSO).Name);

        btnTransformDictionary = new Dictionary<BuildingTypeSO, Transform>();

        int index = 0;

        arrowBtn = Instantiate(btnTemplate, transform);
        arrowBtn.gameObject.SetActive(true);

        arrowBtn.Find("image").GetComponent<Image>().sprite = arrowSprite;
        arrowBtn.Find("image").GetComponent<RectTransform>().sizeDelta = new Vector2 (0, -30);

        float offsetAmount = 120f;
        arrowBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(offsetAmount * index, 0);


        arrowBtn.GetComponent<Button>().onClick.AddListener(() => {
            BuildingManager.Instance.SetActiveBuildingType(null);
        });

        MouseExitEnterEvents mouseExitEnterEvents = arrowBtn.GetComponent<MouseExitEnterEvents>();
        mouseExitEnterEvents.onMouseEnter += (object sender, EventArgs e) =>
        {
            TooltipUi.instance.Show("Arrow");

        };

        mouseExitEnterEvents.onMouseExit += (object sender, EventArgs e) =>
        {
            TooltipUi.instance.Hide();
        };

        index++;

        foreach (BuildingTypeSO buildingType in buildingTypeList.List)
        {
            if (ignoreBuildingTypeList.Contains(buildingType)) continue;
            Transform btnTransform = Instantiate(btnTemplate, transform);
            btnTransform.gameObject.SetActive(true);

            btnTransform.Find("image").GetComponent<Image>().sprite = buildingType.sprite;

            offsetAmount = 120f;
            btnTransform.GetComponent<RectTransform>().anchoredPosition = new Vector2(offsetAmount * index,0);


            btnTransform.GetComponent<Button>().onClick.AddListener(() => {
                BuildingManager.Instance.SetActiveBuildingType(buildingType);
                });

            mouseExitEnterEvents = btnTransform.GetComponent<MouseExitEnterEvents>();
            mouseExitEnterEvents.onMouseEnter += (object sender, EventArgs e) =>
            {
                TooltipUi.instance.Show(buildingType.nameString + "\n" + buildingType.GetConstructionResourceCostString());

            };

            mouseExitEnterEvents.onMouseExit += (object sender, EventArgs e) =>
            {
                TooltipUi.instance.Hide();
            };

            btnTransformDictionary[buildingType] = btnTransform;

            index++;
        }
    }
    private void Start()
    {
        BuildingManager.Instance.OnActiveBuildingTypeChanged += BuildingManager_OnActiveBuildingTypeChanged;

        UpdateActiveBuildingType();
    }

    private void BuildingManager_OnActiveBuildingTypeChanged(object sender, BuildingManager.OnActiveBuildingTypeChangedEventArgs e)
    {
        UpdateActiveBuildingType();
    }

    private void UpdateActiveBuildingType()
    {
        arrowBtn.Find("selected").gameObject.SetActive(false);
        foreach(BuildingTypeSO buildingType in btnTransformDictionary.Keys)
        {
            Transform btnTransform = btnTransformDictionary[buildingType];
            btnTransform.Find("selected").gameObject.SetActive(false);
        }
        BuildingTypeSO activeBuildingType = BuildingManager.Instance.GetActiveBuildingType();
        if (activeBuildingType == null)
        {
            arrowBtn.Find("selected").gameObject.SetActive(true);

        }
        else
        {
            btnTransformDictionary[activeBuildingType].Find("selected").gameObject.SetActive(true);
        }
    }
}
