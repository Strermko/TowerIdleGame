using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UpgradesUI : MonoBehaviour
{
    [SerializeField] private string animationParameterName;
    
    private Animator _animator;

    private void Awake()
    {
        if(animationParameterName.Length <= 0) Debug.LogError("Parameter name in not defined!");
        
        _animator ??= GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        _animator.SetBool(animationParameterName, true);
    }

    public void Close()
    {
        _animator.SetBool(animationParameterName, false);
    }
    
    public void Disable()
    {
        gameObject.SetActive(false);
    }
}