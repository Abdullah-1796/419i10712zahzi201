using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RecordAndReplay : MonoBehaviour
{
    [SerializeField] private GameObject[] avatarParts;
    private List<PlayerTransform[]> snapshots;
    private bool isRecording = false;
    private bool isReplaying = false;
    private float interval = 0.016f;
    private float time = 0;
    private int index = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        snapshots = new List<PlayerTransform[]>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time >= interval && isRecording)
        {
            time = 0;
            Record();
        }

        if(/*time >= interval &&*/ isReplaying)
        {
            time = 0;
            Replay();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            isRecording = true;
            isReplaying = false;
            snapshots.Clear();
            index = 0;
        }
        else if(Input.GetKeyDown(KeyCode.P))
        {
            isReplaying = true;
            isRecording = false;

            GetComponent<Avatar>().useCalibrationData = false;

            foreach (Animator anim in GetComponentsInChildren<Animator>())
            {
                anim.enabled = false;
            }

            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = true;
            }


            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            isRecording = false;
        }
    }

    void Record()
    {
        PlayerTransform[] playerTransforms = new PlayerTransform[avatarParts.Length];
        playerTransforms[0] = new PlayerTransform
        {
            position = avatarParts[0].transform.position,
            rotation = avatarParts[0].transform.rotation
        };
        for (int i = 1; i < avatarParts.Length; i++)
        {
            playerTransforms[i] = new PlayerTransform
            {
                position = avatarParts[i].transform.localPosition,
                rotation = avatarParts[i].transform.localRotation
            };
        }
        snapshots.Add(playerTransforms);
        Debug.Log("Recording");
    }

    void Replay()
    {
        if (GetComponent<Animator>().enabled)
            GetComponent<Animator>().enabled = false;
        if (snapshots.Count > index)
        {
            avatarParts[0].transform.position = snapshots[index][0].position;
            avatarParts[0].transform.rotation = snapshots[index][0].rotation;
            for (int i = 1; i < avatarParts.Length; i++)
            {
                avatarParts[i].transform.localPosition = snapshots[index][i].position;
                avatarParts[i].transform.localRotation = snapshots[index][i].rotation;
            }
            index++;
        }
        else
        {
            index = 0;
            isReplaying = false;
            GetComponent<Avatar>().useCalibrationData = true;
        }
        Debug.Log("Replaying");
    }

    struct PlayerTransform
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}
