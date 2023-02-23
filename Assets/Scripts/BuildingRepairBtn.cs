using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BuildingRepairBtn : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private ResourceTypeSO goldResourceType;
    private void Awake()
    {
        transform.Find("button").GetComponent<Button>().onClick.AddListener(()
            =>{
                int missingHealth = healthSystem.GetHealthAmountMax() - healthSystem.GetHealthAmount();
                int repairCost = missingHealth / 4;

                ResourceAmount[] resourceAmountCost = new ResourceAmount[] { new ResourceAmount { resourceType = goldResourceType, amount = repairCost } };

                if (ResourceManager.instance.CanAfford(resourceAmountCost))
                {

                    //can afford repairs
                    healthSystem.HealFull();
                    ResourceManager.instance.SpendResources(resourceAmountCost);
                }
                else
                {
                    //cannot afford
                    TooltipUi.instance.Show("Cannot Afford repair costs", new TooltipUi.TooltipTimer { timer = 2f });
                }

        });
    }
}
