using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleUpgradeUI : MonoBehaviour
{
    [Header("Background settings")]
    [SerializeField] private Color enabledColor;
    [SerializeField] private Color disabledColor;
    
    [Header("Child objects references")]
    

    private Image _background;

    private void Awake()
    {
        _background ??= GetComponent<Image>();

        //TODO: Add check if player has enough resource to buy upgrade and change color of background
        _background.color = enabledColor;
    }
}