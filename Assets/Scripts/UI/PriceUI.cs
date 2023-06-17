using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PriceUI : MonoBehaviour
{
    [SerializeField] private CostUI costUIPrefab;
    [SerializeField] private DictionaryOfCosts dictionaryOfCosts;
    
    private void OnEnable()
    {
        foreach (var cost in dictionaryOfCosts)
        {
            Debug.Log(cost.Key + " " + cost.Value);
        }
    }
}
