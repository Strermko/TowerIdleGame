using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PopupTextUI : MonoBehaviour
{
    [SerializeField] private float disappearTime = 1f;
    [SerializeField] private float axesRange = 3f;
    [SerializeField] private float movingSpeed = 5f;
    
    private Vector3 _randomVector;
    
    private void Start()
    {
        _randomVector = new Vector3(Random.Range(-axesRange, axesRange), movingSpeed);
    }
    
    public void Setup(int value)
    {
        GetComponent<TMPro.TextMeshPro>().text = $"+{value.ToString()}";
        Destroy(gameObject, disappearTime);
    }

    private void Update()
    {
        transform.position += _randomVector * Time.deltaTime;
    }
}