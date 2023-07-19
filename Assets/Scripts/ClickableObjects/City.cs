using UnityEngine;
using UnityEngine.EventSystems;

public class City : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UpgradesUI upgradesUI;

        public void OnPointerClick(PointerEventData eventData)
    {
        upgradesUI.Open();
    }
}
