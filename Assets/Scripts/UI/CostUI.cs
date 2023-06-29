using UnityEngine;
using UnityEngine.UI;

public class CostUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMPro.TextMeshProUGUI textField;
    
    public void CreateComponent(ResourceType resourceType, int value)
    {
        textField.text = value.ToString();
        image.sprite = UnityEngine.Resources.Load<Sprite>($"Sprites/{resourceType.ToString()}");
    }
}