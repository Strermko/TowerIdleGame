using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class ResourceGenerator : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int clickValue = 1;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameEventManager.Instance.AddResource(resourceType, clickValue);
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