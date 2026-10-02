using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;

public class camera : MonoBehaviour
{
    public camera cam;
    public CharacterController rb;
    public GameManager gameManager;
    public float d23transitionspeed = 0.1f;

    public float sensitivity = 0.1f;
    public float offset = 5;
    float pitch;
    public float maxpitch=1;
    public float minpitch=-1;
    float yaw;
    public float mousex;
    public float mousey;
    public float lerpspeed;
    Vector3 playerpos;
    Vector3 camoffset;
    Vector3 enemypos;
    void Start()
    {
        
    }

    void LateUpdate()
    {
        switch (gameManager.currentState)
        {
            case GameManager.GameState.Explore3D:
            case GameManager.GameState.Dash:
            case GameManager.GameState.Death:
                mousex = Input.GetAxis("Mouse X");
                mousey = Input.GetAxis("Mouse Y");
                playerpos = rb.transform.position;
                pitch -= mousey * sensitivity;
                yaw += mousex * sensitivity; ;
                pitch = Mathf.Clamp(pitch, minpitch, maxpitch);
                Quaternion rotation = quaternion.Euler(pitch, yaw, 0);
                camoffset = rotation * new Vector3(0, 0, -offset);
                if (Input.GetKeyDown(KeyCode.UpArrow) && offset <= 20)
                {
                    offset++;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) && offset >= 2)
                {
                    offset--;
                }
                cam.transform.position = Vector3.Slerp(cam.transform.position, rb.transform.position + camoffset, lerpspeed);
                cam.transform.LookAt(rb.transform.position);
                break;
            case GameManager.GameState.Battle2D:
                playerpos = rb.transform.position;
                enemypos = gameManager.closestEnemy.transform.position;
                Vector3 midpoint = (playerpos + enemypos) / 2;
                camoffset = new Vector3(0, 0, -offset);
                cam.transform.position = Vector3.Slerp(cam.transform.position, midpoint + camoffset, d23transitionspeed);
                cam.transform.LookAt(midpoint);
                break;
        }
    }
}
