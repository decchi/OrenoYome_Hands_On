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


public class YomeControler : Singleton<YomeControler>, IInputClickHandler
{
    float eyeSightangle = 80.0f;
    public GameObject Yome;

    // Consts
    const float RayCastLength = 10.0f;


    NavMeshAgent agent;
    Animator animator;
    // Use this for initialization
    void Start () {

        Yome.SetActive(false);
        StartCoroutine(loop());
        if (Yome.GetComponent<NavMeshAgent>() != null )
        {
            agent = Yome.GetComponent<NavMeshAgent>();
        }
        animator = Yome.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update () {
        animator.SetFloat("Speed", agent.velocity.sqrMagnitude);

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
            if (!Yome.activeSelf)
            {
                Yome.SetActive(true);
                
                hitPos = hitInfo.point;
                hitNormal = hitInfo.normal;
                Yome.transform.position = hitPos;
                Vector3 heading = Yome.transform.position - Camera.main.transform.position;
                heading.y = 0;
                Yome.transform.rotation = Quaternion.LookRotation(heading);
                Yome.transform.rotation = Yome.transform.rotation * Quaternion.Euler(0, 180, 0);
                ConversationManager.Instance.Initialize();
            }
                 //agent.destination = hitInfo.point;
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

        }
    }

    public void ComeHere()
    {
        agent.destination = Camera.main.transform.TransformPoint(0f,0f,1f);
    }


}
