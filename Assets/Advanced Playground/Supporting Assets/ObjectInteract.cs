using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectInteract : MonoBehaviour
{
    [SerializeField] private UnityEvent OnClick;
    [SerializeField] private UnityEvent OnHoverEnter;
    [SerializeField] private UnityEvent OnHoverExit;
    
    [SerializeField] private AudioSource audioPlayer;
    [SerializeField] private AudioClip audioOnSelect;

    void OnMouseDown()
    {
        if(OnClick!=null)
            OnClick.Invoke();

        if (audioOnSelect && audioPlayer)
        {
            audioPlayer.clip = audioOnSelect;
            audioPlayer.Play();
        }
    }

    private void OnMouseEnter()
    {
        OnHoverEnter.Invoke();
    }

    private void OnMouseExit()
    {
        OnHoverExit.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
