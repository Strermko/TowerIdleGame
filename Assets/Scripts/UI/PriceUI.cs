using UnityEngine;

public class PriceUI : MonoBehaviour
{
    [SerializeField] private CostUI costUIPrefab;
    [SerializeField] public DictionaryOfCost dictionaryOfCost;

    private void OnEnable()
    {
        foreach(var element in dictionaryOfCost.GetPairs())
        {
            CostUI costUI = Instantiate(costUIPrefab, transform);
            costUI.CreateComponent(element.Key, element.Value);
            costUI.transform.SetParent(gameObject.transform);
        }
    }
}