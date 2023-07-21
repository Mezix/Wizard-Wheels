using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SpeedDisplay : MonoBehaviour
{
    public Text NumberOne;
    public Text NumberTwo;
    public Text NumberThree;
    public Text NumberOneExtra;
    public Text NumberTwoExtra;
    public Text NumberThreeExtra;

    private void Awake()
    {
        REF.SD = this;
    }
    public void SetSpeed(float speed)
    {
        speed *= 10;
        string spd = Mathf.FloorToInt(speed).ToString();
        if(spd.Length > 3)
        {
            NumberOne.text = "9";
            NumberTwo.text = "9";
            NumberThree.text = "9";
        }
        else if (spd.Length == 2)
        {
            NumberOne.text = "0";
            NumberTwo.text = spd[0].ToString();
            NumberThree.text = spd[1].ToString();
        }
        else if(spd.Length == 1)
        {
            NumberOne.text = "0";
            NumberTwo.text = "0";
            NumberThree.text = spd[0].ToString();
        }
        else
        {
            NumberOne.text = spd[0].ToString();
            NumberTwo.text = spd[1].ToString();
            NumberThree.text = spd[2].ToString();
        }
    }
}
