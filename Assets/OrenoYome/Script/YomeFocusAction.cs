using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class YomeFocusAction : MonoBehaviour, IFocusable {

    public static bool isYomeGazed = true;

    public void OnFocusEnter()
    {
        isYomeGazed = true;
}

    public void OnFocusExit()
    {
        isYomeGazed = false;
    }


}
