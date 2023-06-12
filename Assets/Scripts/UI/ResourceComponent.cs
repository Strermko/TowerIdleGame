using UnityEngine;

public class ResourceComponent :MonoBehaviour
{
    [SerializeField] public ResourceType resourceType;

    private TMPro.TextMeshProUGUI textField;
    public int currentValue { get; private set; } = 0;

    private void Awake()
    {
        textField = GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    public void UpdateComponent(int value)
    {
        currentValue += value;
        textField.text = currentValue.ToString();
    }
}