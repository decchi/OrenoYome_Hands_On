﻿using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.Networking;
using System;
using System.Text;
using HoloToolkit.Unity.SpatialMapping;

[RequireComponent(typeof(YomeControler))]
public class ConversationManager : Singleton<ConversationManager> {
    [SerializeField]
    public Text answerTextField;
    public Text inputTextField;
    public Image XPic;
    public Image MicPic;
    public TextToSpeechManager textToSpeech;
    DictationRecognizer m_DictationRecognizer;
    public GameObject Speeker;
    private string URL = "https://api.api.ai/v1/query?v=20150910";
    string str1 = "{\"query\":\"";
    string str2 = "\",\"lang\":\"en\",\"sessionId\":\"123456789\"}";
    private string FollowMe_intent = "ComeHere";
    private const float RayCastLength = 10.0f;

    // Use this for initialization
    void Start () {

    }

    public void Initialize()
    {

        m_DictationRecognizer = new DictationRecognizer();

        m_DictationRecognizer.DictationResult += (text, confidence) =>
        {

            inputTextField.text = text;
            StartCoroutine(GetReply(URL, str1 + text + str2));


        };
        m_DictationRecognizer.Start();
        XPic.gameObject.SetActive(false);
    }

    IEnumerator GetReply(string url, string bodyJsonString)
    {

        var request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer 6e69b55263ca4831aec888e3a1c97a4b");


        yield return request.Send();

        Debug.Log("Status Code: " + request.responseCode);
        Debug.Log("Result: " + request.downloadHandler.text);
        ApiaiJson respose = JsonUtility.FromJson<ApiaiJson>(request.downloadHandler.text);

        answerTextField.text = respose.result.fulfillment.speech;
        textToSpeech.SpeakText(respose.result.fulfillment.speech.Replace("."," "));

        if (respose.result.metadata.intentName.Equals(FollowMe_intent))
        {
            YomeControler.Instance.ComeHere();
        }

    }



    // Update is called once per frame
    void Update () {

        if (m_DictationRecognizer != null)
        {
            if (m_DictationRecognizer.Status == SpeechSystemStatus.Stopped)
            {


                if (YomeFocusAction.isYomeGazed)
                {
                    m_DictationRecognizer.Start();
                    XPic.gameObject.SetActive(false);
                }
                else
                {

                    if (!XPic.IsActive())
                    {
                        XPic.gameObject.SetActive(true);
                    }


                }


            }
            else if (m_DictationRecognizer.Status == SpeechSystemStatus.Running)
            {
                if (XPic.IsActive())
                {
                    XPic.gameObject.SetActive(false);
                }

            }
        }




    }

    [Serializable]
    public class ApiaiJson
    {
        public string id;
        public string timestamp;
        public string lang;
        public ApiaiJsonResult result;
        public string sessionId;
    }

    [Serializable]
    public class ApiaiJsonResult
    {
        public string source;
        public string resolvedQuery;
        public string action;
        public bool actionIncomplete;
        public ApiaiJsonFulfillment fulfillment;
        public ApiaiJsonMetadata metadata;
        public float score;

    }

    [Serializable]
    public class ApiaiJsonFulfillment
    {
        public string speech;
    }

    [Serializable]
    public class ApiaiJsonMetadata
    {
        public string intentId;
        public string webhookUsed;
        public string webhookForSlotFillingUsed;
        public string intentName;
    }
}
