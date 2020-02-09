using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float speed = 0.007f;
    private float angleOffset = 45;
    Vector3 lastMousePos;
    private float minSize = 1f;
    private float maxSize = 4.5f;
    private readonly float _overTime = 0.5f;
    private bool _running;
    private Transform player;
    private Vector3 targetDir;
    private float rotateSpeed = 420;


    private void Start()
    {
        targetDir = transform.forward;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.position + Vector3.up * 4f;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            targetDir = Quaternion.Euler(0, -45, 0) * targetDir;
            rotateSpeed = -Mathf.Abs(rotateSpeed);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            targetDir = Quaternion.Euler(0, 45, 0) * targetDir;
            rotateSpeed = Mathf.Abs(rotateSpeed);
        }
 
        if (Vector3.Angle(targetDir, transform.forward) > 10)
        {
            transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);
        }
        else { transform.forward = targetDir; }
        if(Vector3.SignedAngle(targetDir, transform.forward, Vector3.up) > 5 && rotateSpeed > 0)
        {
            transform.forward = targetDir;
        }else if (Vector3.SignedAngle(targetDir, transform.forward, Vector3.up) < 5 && rotateSpeed < 0)
        {
            transform.forward = targetDir;
        }
        //if (_gm._count == 0) return;
        //if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.orthographicSize < maxSize)
        //{
        //    Camera.main.orthographicSize += 0.5f;
        //}
        //if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.orthographicSize > minSize)
        //{
        //    Camera.main.orthographicSize -= 0.5f;
        //}
        //if (Input.GetMouseButton(2) || Input.GetMouseButton(0))
        //{
        //    if(lastMousePos != Vector3.zero)
        //    {
        //        Vector3 offset = (lastMousePos - Input.mousePosition) * speed;
        //        offset = Quaternion.AngleAxis(angleOffset, Vector3.forward) * offset;
        //        transform.position += new Vector3(offset.x, 0, offset.y);
        //    }
        //}
        //if (Input.GetMouseButtonUp(2) || Input.GetMouseButton(0))
        //{
        //    lastMousePos = Vector3.zero;
        //}
        //lastMousePos = Input.mousePosition;
    }
}