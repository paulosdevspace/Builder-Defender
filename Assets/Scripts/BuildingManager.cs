using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingManager : MonoBehaviour {

    public static BuildingManager Instance { get; private set; }

    public event EventHandler<OnActiveBuildingTypeChangedEventArgs> OnActiveBuildingTypeChanged;
    public class OnActiveBuildingTypeChangedEventArgs : EventArgs
    {
        public BuildingTypeSO activeBuildingType;
    }

    [SerializeField] private Building hqBuilding;

    private BuildingTypeSO activeBuildingType;
    private BuildingTypeListSO buildingTypeList;


    private Camera mainCamera;


    private void Awake()
    {
        Instance = this;

        buildingTypeList = Resources.Load<BuildingTypeListSO>(typeof(BuildingTypeListSO).Name);
    }
    private void Start()
    {
        mainCamera = Camera.main;

        hqBuilding.GetComponent<HealthSystem>().OnDied += HQ_OnDied; ;

    }

    private void HQ_OnDied(object sender, EventArgs e)
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.GameOver);

        GameOverUI.Instance.Show();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (activeBuildingType != null)
            {
                if (CanSpawnBuilding(activeBuildingType, UtilsClass.GetWorldMousePosition(), out string errorMessage))
                {
                    if (ResourceManager.instance.CanAfford(activeBuildingType.constructionResourceCostArray))
                    {
                        ResourceManager.instance.SpendResources(activeBuildingType.constructionResourceCostArray);
                        //Instantiate(activeBuildingType.prefab, UtilsClass.GetWorldMousePosition(), Quaternion.identity);
                        BuildingConstruction.Create(UtilsClass.GetWorldMousePosition(), activeBuildingType);
                        SoundManager.Instance.PlaySound(SoundManager.Sound.BuildingPlaced);
                    }
                    else
                    {
                        TooltipUi.instance.Show("Cannot afford" + activeBuildingType.GetConstructionResourceCostString(), new TooltipUi.TooltipTimer { timer = 2f });
                    }
                } else{
                    TooltipUi.instance.Show(errorMessage, new TooltipUi.TooltipTimer { timer = 2f});
                }
            }

        }
    }
    public void SetActiveBuildingType(BuildingTypeSO buildingType)
    {
        activeBuildingType = (buildingType);
        OnActiveBuildingTypeChanged?.Invoke(this, new OnActiveBuildingTypeChangedEventArgs { activeBuildingType = activeBuildingType});
    }
    public BuildingTypeSO GetActiveBuildingType()
    {
        return activeBuildingType;
    }

    private bool CanSpawnBuilding(BuildingTypeSO buildingType, Vector3 position, out string errorMessage) {
        BoxCollider2D boxCollider2D = buildingType.prefab.GetComponent<BoxCollider2D>();

        Collider2D[] collider2DArray = Physics2D.OverlapBoxAll(position + (Vector3)boxCollider2D.offset, boxCollider2D.size, 0);

        bool isAreaClear = collider2DArray.Length == 0;
        if (!isAreaClear) {
            errorMessage = "Area is not clear";
            return false; 

        }

        collider2DArray = Physics2D.OverlapCircleAll(position, buildingType.minConstructionRadius);
        foreach (Collider2D collider2D in collider2DArray)
        {
            //collider inside construction radius
            BuildingTypeHolder buildingTypeHolder = collider2D.GetComponent<BuildingTypeHolder>();
            if (buildingTypeHolder != null)
            {
                //has buildind type holder
                if (buildingTypeHolder.buildingType == buildingType)
                {
                    //there is another building of this type within radius
                    errorMessage = "Too close to another building of same type";
                    return false;
                }
            }
        }

        float maxConstructionRadius = 25;
        collider2DArray = Physics2D.OverlapCircleAll(position, maxConstructionRadius);
        foreach (Collider2D collider2D in collider2DArray)
        {
            //collider inside construction radius
            BuildingTypeHolder buildingTypeHolder = collider2D.GetComponent<BuildingTypeHolder>();
            if (buildingTypeHolder != null)
            {
                //its a building 
                errorMessage = null;
                return true;
            }
        }
        errorMessage = "Too far from any building";
        return false;
    }
    public Building GetHQBuilding()
    {
        return hqBuilding;
    }
}
