using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class PriceUI : MonoBehaviour
{
    [SerializeField] private CostUI costUIPrefab;
    [SerializeField] private DictionaryOfCost dictionaryOfCost;
    
    private void OnEnable()
    {
        foreach (var cost in dictionaryOfCost)
        {
            Debug.Log(cost.Key + " " + cost.Value);
        }
    }
}
