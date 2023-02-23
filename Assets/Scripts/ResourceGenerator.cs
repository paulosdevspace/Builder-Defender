using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{

    public static int GetNearbyResourceAmount(ResourceGeneratorData resourceGeneratorData, Vector3 position)
    {
        int nearbyresourceAmount = 0;
    Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(position, resourceGeneratorData.resourceDetectionRadius);
        foreach (Collider2D collider2D in collider2DArray)
        {
            ResourceNode resourceNode = collider2D.GetComponent<ResourceNode>();
            if (resourceNode != null)
            {
                //é um ponto de recurso

                if (resourceNode.resourceType == resourceGeneratorData.resourceType)
                {
                    //é tipo de node é igual ao de gerador
                    nearbyresourceAmount++;

                }
            }
        }

        nearbyresourceAmount = Mathf.Clamp(nearbyresourceAmount, 0, resourceGeneratorData.maxResourceAmount);
        return nearbyresourceAmount;
    }

    private ResourceGeneratorData resourceGeneratorData;
    private float timer;
    private float timerMax;

    private void Awake()
    {
        resourceGeneratorData = GetComponent<BuildingTypeHolder>().buildingType.resourceGeneratorData;
        timerMax = resourceGeneratorData.timerMax;
    }


    private void Start()
    {
        int nearbyresourceAmount = GetNearbyResourceAmount(resourceGeneratorData, transform.position);

        if (nearbyresourceAmount == 0)
        {
            //não ha pontos de recurso perto
           //desabilita o script
            enabled = false;
        }
        else
        {
            //logica para a geração de recursos, a formula calcula quanto mais nodes de recurso tem perto, mais rapido ele gera recursos
            timerMax = (resourceGeneratorData.timerMax/2f) + resourceGeneratorData.timerMax * (1- (float)nearbyresourceAmount/resourceGeneratorData.maxResourceAmount);
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer += timerMax;
            ResourceManager.instance.AddResource(resourceGeneratorData.resourceType, 1);
        }
    }
    public ResourceGeneratorData GetResourceGeneratorData()
    {
        return resourceGeneratorData;
    }

    public float GetTimerNormalized()
    {
        return timer/timerMax;
    }

    public float GetAmountGeneratedPerSecond()
    {
        return 1 / timerMax;
    }

}
