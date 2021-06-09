using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassengerCountWidget : MonoBehaviour {
    private void Start() {
        Text text = GetComponent<Text>();

        GameObject ferry = GameObject.FindGameObjectWithTag("Player");
        PassengerBoarder boarder = ferry.GetComponent<PassengerBoarder>();
        boarder.OnBoarding.AddListener(() => {
            text.text = boarder.PassengerCount.ToString();
        });

        ferry.GetComponent<FerryController>().OnConnectToDock.AddListener(() => text.text = "0");
    }
}
