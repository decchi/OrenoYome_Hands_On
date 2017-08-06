using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using UnityEngine.Networking;
using System;
using System.Text;
using HoloToolkit.Unity.SpatialMapping;

public class DictationManager : Singleton<DictationManager> {
    [SerializeField]
    public Text answerTextField;
    public Text inputTextField;
    public Image XPic;
    public Image MicPic;
    private Color MicPicStopColor = Color.gray;
    private Color MicPicRunningColor = Color.green;
    private Color MicPicErrorColor = Color.red;
    public TextToSpeechManager textToSpeech;
    public DictationRecognizer m_DictationRecognizer;
    public GameObject Speeker;
    private string URL = "https://api.api.ai/v1/query?v=20150910";
    string str1 = "{\"query\":\"";
    string str2 = "\",\"lang\":\"en\",\"sessionId\":\"123456789\"}";
    private string MeshOn_intent = "MeshOn";
    private string MeshOff_intent = "MeshOff";
    private string FollowMe_intent = "ComeHere";
    string AnalyzeSentimentstr1 = "{\"document\":{\"type\":\"PLAIN_TEXT\",\"language\":\"en\",\"content\":\"";
    string AnalyzeSentimentstr2 = "\"},\"encodingType\":\"UTF8\"}";
    private const float RayCastLength = 10.0f;
    public YomeControler yomeControler;
    public Material MeshMaterial;
    public Material NonMeshMaterial;





    // Use this for initialization
    void Start () {

    }

    public void Initialize()
    {

        textToSpeech = Speeker.GetComponent<TextToSpeechManager>();
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
        request.SetRequestHeader("Authorization", "Bearer your agent ");


        yield return request.Send();

        Debug.Log("Status Code: " + request.responseCode);
        Debug.Log("Result: " + request.downloadHandler.text);
        ApiaiJson respose = JsonUtility.FromJson<ApiaiJson>(request.downloadHandler.text);

        answerTextField.text = respose.result.fulfillment.speech;
        textToSpeech.SpeakText(respose.result.fulfillment.speech.Replace("."," "));

        if (respose.result.metadata.intentName.Equals(FollowMe_intent))
        {
            yomeControler.ComeHere();
        }

        if (respose.result.metadata.intentName.Equals(MeshOn_intent))
        {
            SpatialMappingManager.Instance.SetSurfaceMaterial(MeshMaterial);
        }
        if (respose.result.metadata.intentName.Equals(MeshOff_intent))
        {
            SpatialMappingManager.Instance.SetSurfaceMaterial(NonMeshMaterial);
        }


    }



    // Update is called once per frame
    void Update () {

        if (m_DictationRecognizer.Status == SpeechSystemStatus.Stopped)
        {
            
              
                if (YomeFocusAction.isYomeGazed)
                {
                    m_DictationRecognizer.Start();
                    XPic.gameObject.SetActive(false);
                }
                else {

                    if (!XPic.IsActive())
                    {
                        XPic.gameObject.SetActive(true);
                    }
                    
                    MicPic.color = MicPicStopColor;
                }

            
        }
        else if (m_DictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            if (XPic.IsActive())
            {
                XPic.gameObject.SetActive(false);
            }
            MicPic.color = MicPicRunningColor;
        }
        else
        {
            MicPic.color = MicPicErrorColor;
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
