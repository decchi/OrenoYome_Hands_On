using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity;
using UnityEngine.AI;
using System;
using System.Text;
using UnityEngine.UI;


public class YomeControler : MonoBehaviour,IInputClickHandler
{
    float eyeSightangle = 80.0f;
    public GameObject Yome;

    // Consts
    public const float RayCastLength = 10.0f;

    // Config
    public LayerMask UILayerMask;

    //2017.4.6 add mori
       NavMeshAgent agent;

    private String YomeTag = "Yome";
    public Text inputTextField;
    // Use this for initialization
    void Start () {
        Yome.SetActive(false);

        agent = Yome.GetComponent<NavMeshAgent>(); //2017.4.6 add mori
        StartCoroutine(loop());
    }

    // Update is called once per frame
    void Update () {




    }

    //AirTapされたときに呼び出される関数
    public void OnInputClicked(InputClickedEventData eventData)
    {
        Vector3 hitPos, hitNormal;
                RaycastHit hitInfo;
        Vector3 uiRayCastOrigin = Camera.main.transform.position;
        Vector3 uiRayCastDirection = Camera.main.transform.forward;
        if (Physics.Raycast(uiRayCastOrigin, uiRayCastDirection, out hitInfo, RayCastLength, SpatialMappingManager.Instance.LayerMask))
        {
            Debug.Log("hIT!");
            if (!Yome.activeSelf)
            {
                Yome.SetActive(true);
                DictationManager.Instance.Initialize();
                hitPos = hitInfo.point;
                hitNormal = hitInfo.normal;
                Debug.Log("hitPos2 !" + hitPos.ToString());
                Yome.transform.position = hitPos;
                Vector3 heading = Yome.transform.position - Camera.main.transform.position;
                heading.y = 0;
                Yome.transform.rotation = Quaternion.LookRotation(heading);
                Yome.transform.rotation = Yome.transform.rotation * Quaternion.Euler(0, 180, 0);

            }
                 agent.destination = hitInfo.point; //2017.4.6 add mori

            //     Debug.Log("agent.destination  !" + agent.destination.ToString());
        }

    }

    private IEnumerator loop()
    {
        // ループ
        while (true)
        {
            // 1秒毎にループします
            yield return new WaitForSeconds(1f);
            float angle = Vector3.Angle((Camera.main.transform.position - Yome.transform.position).normalized, Yome.transform.forward);

            if (angle > eyeSightangle)
            {
                //lookAtIK.enabled = false;
            }
            else
            {
                //lookAtIK.enabled = true;
            }



             if (agent.velocity.sqrMagnitude == 0 && YomeFocusAction.isYomeGazed)
             {
                Vector3 heading = Yome.transform.position - Camera.main.transform.position;
                heading.y = 0;
                Yome.transform.rotation = Quaternion.LookRotation(heading);
                Yome.transform.rotation = Yome.transform.rotation * Quaternion.Euler(0, 180, 0);
                //lookAtIK.enabled = true;
              }
        }
    }

    public void ComeHere()
    {
        agent.destination = Camera.main.transform.TransformPoint(0f,0f,1f);
    }


}
