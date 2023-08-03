using UnityEngine;
using UnityEngine.UI;

public class SingleUpgradeUI : MonoBehaviour
{
    [Header("Background settings")]
    [SerializeField] private Color enabledColor;
    [SerializeField] private Color disabledColor;
    
    [Header("Price settings")]
    [SerializeField] private Transform priceComponent; 
    [SerializeField] private CostUI costPrefab;
    [SerializeField] public DictionaryOfCost dictionaryOfCost;
    [SerializeField] public float scale = 1.2f;
    
    
    private bool _setPrice = false;
    private Image _background;

    private void Awake()
    {
        _background ??= GetComponent<Image>();

        //TODO: Add check if player has enough resource to buy upgrade and change color of background
        _background.color = enabledColor;

        if (!_setPrice)
        {
            RenderPrice();
            _setPrice = true;
        }
    }
    
    public void BuyUpgrade()
    {
        dictionaryOfCost.UpCost(scale);
        for (int i = 0; i < priceComponent.childCount; i++)
        {
            Destroy(priceComponent.GetChild(i).gameObject);
        }
        RenderPrice();
    }
    
    private void RenderPrice()
    {
        foreach (var element in dictionaryOfCost.GetPairs())
        {
            var costUI = Instantiate(costPrefab, transform);
            costUI.CreateComponent(element.Key, element.Value);
            costUI.transform.SetParent(priceComponent);
        }
    }
}