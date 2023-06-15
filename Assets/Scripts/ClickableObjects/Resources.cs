using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Resources : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private PopupTextUI popupTextPrefab;
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int clickValue = 1;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameEventManager.Instance.AddResource(resourceType, clickValue);

        if (!Camera.main) return;
        var ray = Camera.main.ScreenPointToRay(eventData.position);
        
        if (!Physics.Raycast(ray, out var raycastHit)) return;
        
        var popupText = Instantiate(popupTextPrefab, raycastHit.point, Quaternion.identity);
        popupText.Setup(clickValue);
    }

    private void OnMouseEnter()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}