using UnityEngine;

public class PriceUI : MonoBehaviour
{
    [SerializeField] private CostUI costUIPrefab;
    [SerializeField] public DictionaryOfCost dictionaryOfCost;

    private void OnEnable()
    {
        foreach(var element in dictionaryOfCost.GetPairs())
        {
            Debug.Log(element.Key);
        }
    }
}