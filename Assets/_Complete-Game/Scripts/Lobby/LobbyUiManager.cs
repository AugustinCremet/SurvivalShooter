using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LobbyUiManager : MonoBehaviour
{
    [SerializeField] TMP_InputField nickName;
    [SerializeField] GameObject emptyNickName;
    [SerializeField] GameObject ShortNickName;
    [SerializeField] GameObject LongNickName;
    [SerializeField] GameObject transitionCanvas;
    
    public static Action feedBack;
    private void Awake()
    {
        feedBack = Feedback;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Return))
            Join();
    }

    public void Join()
    {
        if (nickName.text.Length < 4)
        {
            nickName.GetComponentInParent<Animator>().SetBool("Wiggle", true);
        }
        else if (nickName.text.Length > 10)
        {
            nickName.GetComponentInParent<Animator>().SetBool("Wiggle", true);
        }
        else
        {
            GetComponentInParent<LobbyManager>().Connect();
            transitionCanvas.SetActive(true);
            transitionCanvas.GetComponent<Animator>().SetTrigger("Enter");
        }

        this.enabled = false;
    }

    public void Feedback()
    {
        if (nickName.text.Length < 4)
        {
            ShortNickName.SetActive(true);
        }
        else if (nickName.text.Length > 9)
        {
            LongNickName.SetActive(true);
        }
        else
        {
            emptyNickName.SetActive(true);
        }
    }

    public void OnPlayerNickNameValueChanged()
    {       
        if (nickName.text.Length > 3 && nickName.text.Length < 10)
        {
            GetComponentInParent<LobbyManager>().SetNickname(nickName.text);
        }
    }

    public void TextStartEditing()
    {
        emptyNickName.SetActive(false);
        ShortNickName.SetActive(false);
        LongNickName.SetActive(false);
    }
}
