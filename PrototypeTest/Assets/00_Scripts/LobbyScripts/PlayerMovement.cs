using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PhotonView photonView;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    public float Health;


    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            CheckInput();    
        }
    }

    private void CheckInput()
    {
        float moveSpeed = 10f;
        float rotateSpeed = 50f;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        transform.position += transform.forward * (z * moveSpeed * Time.deltaTime);
        transform.Rotate(new Vector3(0, x * rotateSpeed * Time.deltaTime, 0));
    }
}