using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClaire : MonoBehaviour
{
    public GameObject player;

    [SerializeField] private int rotationSpeed = 3;
    [SerializeField] private bool rotationCam = true;
    private Vector3 _offset;
    // Start is called before the first frame update
    void Start()
    {
        if (player != null)
        {
            _offset = transform.position - player.transform.position;    
        }
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.transform.position + _offset;
        if (rotationCam)
        {
            Quaternion angle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up);
            _offset = angle * _offset;
            transform.LookAt(player.transform);
        }
    }
}
