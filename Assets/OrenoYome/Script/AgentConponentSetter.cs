using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(OVRLipSyncContextMorphTarget))]
[RequireComponent(typeof(OVRLipSyncContext))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(YomeFocusAction))]
[RequireComponent(typeof(TextToSpeechManager))]

public class AgentConponentSetter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Reset()
    {
        gameObject.GetComponent<NavMeshAgent>().speed = 0.75f;
        AudioSource source = gameObject.GetComponent<AudioSource>();
        gameObject.GetComponent<OVRLipSyncContext>().audioSource = source;
        gameObject.GetComponent<TextToSpeechManager>().AudioSource = source;
        gameObject.GetComponent<TextToSpeechManager>().Voice = TextToSpeechVoice.Zira;

        List <GameObject> allChildren = new List<GameObject>();
        GetChildren(gameObject, ref allChildren);

        foreach (GameObject chiid in allChildren)
        {
            if (chiid.name == "MTH_DEF")
            {
                gameObject.GetComponent<OVRLipSyncContextMorphTarget>().skinnedMeshRenderer = chiid.GetComponent<SkinnedMeshRenderer>();
            }
                
        }

    }

    public static void GetChildren(GameObject obj, ref List<GameObject> allChildren)
    {
        Transform children = obj.GetComponentInChildren<Transform>();
        //子要素がいなければ終了
        if (children.childCount == 0)
        {
            return;
        }
        foreach (Transform ob in children)
        {
            allChildren.Add(ob.gameObject);
            GetChildren(ob.gameObject, ref allChildren);
        }
    }
}
