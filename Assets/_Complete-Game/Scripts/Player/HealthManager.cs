using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HealthManager : MonoBehaviour
{
    Slider slider;
    void Awake()
    {
        slider = GetComponent<Slider>();
    }
    void Update()
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Score"))
            return;

        slider.value = (int)PhotonNetwork.LocalPlayer.CustomProperties["Health"];
    }
}
