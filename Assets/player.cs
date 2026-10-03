using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    public CharacterController rb;
    Quaternion turndir;
    public GameManager gameManager;
    public Camera cam;

    Vector3 movedir;
    float jumpvel;
    public float speed;
    public float sprintspeed;
    public float jumpforce;
    public float gravity;
    public float dashspeed;
    Vector3 forward;
    Vector3 right;
    Vector3 camdir;
    public float moveright;
    public float moveforward;
    public float turnspeed = 0.5f;
    float finalspeed;
    bool dashoncooldown = false;
    public int state = 1;
    public int flowstate;
    public Image flow1;
    public Image flow2;
    public Image flow3;
    public Image flow4;

    public Image healthbar;
    public Image flowbar;
    public Image healthlag;
    public Image flowlag;

    float health = 100;
    public float healthmax = 100;
    float flow = 100;
    public float flowmax = 100;

    public AudioSource one;
    public AudioSource two;
    public AudioSource three;
    public AudioSource four;
    public AudioSource battlechime;
    bool ismoving = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthbar.fillAmount = 0;
        healthlag.fillAmount = 0;
        flowbar.fillAmount = 0;
        flowlag.fillAmount = 0;
        flow = 0;
        flow4.canvasRenderer.SetAlpha(0);
        flow3.canvasRenderer.SetAlpha(0);
        flow2.canvasRenderer.SetAlpha(0);
        flow1.canvasRenderer.SetAlpha(0);
    }

    // Update is called once per frame
    void Update()
    {
        float healthpercent = health / healthmax;
        float flowpercent = flow / flowmax;
        flowpercent = flowpercent - ((int)(flowpercent));
        flowstate = (int)(flow / 100);
        if (flowstate == 0)
        {
            flow1.canvasRenderer.SetAlpha(0);
            flow2.canvasRenderer.SetAlpha(0);
            flow3.canvasRenderer.SetAlpha(0);
            flow4.canvasRenderer.SetAlpha(0);
        }
        else if (flowstate == 1)
        {
            flow1.canvasRenderer.SetAlpha(Mathf.Lerp(flow1.canvasRenderer.GetAlpha(), 1, 0.5f));
            flow2.canvasRenderer.SetAlpha(0);
            flow3.canvasRenderer.SetAlpha(0);
            flow4.canvasRenderer.SetAlpha(0);
        }
        else if (flowstate == 2)
        {
            flow1.canvasRenderer.SetAlpha(1);
            flow2.canvasRenderer.SetAlpha(Mathf.Lerp(flow2.canvasRenderer.GetAlpha(), 1, 0.5f));
            flow3.canvasRenderer.SetAlpha(0);
            flow4.canvasRenderer.SetAlpha(0);
        }
        else if (flowstate == 3)
        {
            flow1.canvasRenderer.SetAlpha(1);
            flow2.canvasRenderer.SetAlpha(1);
            flow3.canvasRenderer.SetAlpha(Mathf.Lerp(flow3.canvasRenderer.GetAlpha(), 1, 0.5f));
            flow4.canvasRenderer.SetAlpha(0);
        }
        else if (flowstate == 4)
        {
            flow1.canvasRenderer.SetAlpha(1);
            flow2.canvasRenderer.SetAlpha(1);
            flow3.canvasRenderer.SetAlpha(1);
            flow4.canvasRenderer.SetAlpha(Mathf.Lerp(flow4.canvasRenderer.GetAlpha(), 1, 0.5f));
        }
        healthbar.fillAmount = Mathf.Lerp(healthbar.fillAmount, healthpercent, 0.5f);
        healthlag.fillAmount = Mathf.Lerp(healthlag.fillAmount, healthpercent, 0.1f);
        flowbar.fillAmount = Mathf.Lerp(flowbar.fillAmount, flowpercent, 0.5f);
        flowlag.fillAmount = Mathf.Lerp(flowlag.fillAmount, flowpercent, 0.2f);
        if (Input.GetKeyDown(KeyCode.H))
        {
            health = health - 10;
        }
        if(Input.GetKeyDown(KeyCode.E)&&gameManager.currentState==GameManager.GameState.Explore3D)
        {
            battlechime.Play();
            //find closest enemy and set state to battle
            gameManager.currentState = GameManager.GameState.Battle2D;
        }
        else if(Input.GetKeyDown(KeyCode.E)&&gameManager.currentState==GameManager.GameState.Battle2D)
        {
            gameManager.currentState = GameManager.GameState.Explore3D;
        }
        if (Input.GetKeyDown(KeyCode.F) && flow <= 400)
        {
            float preflow = flow;
            flow = flow + 10;
            if (preflow < 100 && flow >= 100)
            {
                one.Play();
            }
            else if (preflow < 200 && flow >= 200)
            {
                two.Play();
            }
            else if (preflow < 300 && flow >= 300)
            {
                three.Play();
            }
            else if (preflow < 400 && flow == 400)
            {
                four.Play();
            }
        }
        // case 0 dead case 1 regular movement case 2 dash case 3 2d battle
        switch (gameManager.currentState)
        {
            case GameManager.GameState.Death:
                break;
            case GameManager.GameState.Explore3D:
                forward = cam.transform.forward;
                right = cam.transform.right;
                forward.y = 0;
                right.y = 0;
                forward = forward.normalized;
                right = right.normalized;
                moveright = Input.GetAxis("Horizontal");
                moveforward = Input.GetAxis("Vertical");
                Vector3 movedir = rb.transform.forward + forward * moveforward + right * moveright;
                turndir = Quaternion.LookRotation(movedir);
                jumpvel = rb.isGrounded && Input.GetKey(KeyCode.Space) ? jumpforce : (!rb.isGrounded ? jumpvel - gravity * Time.deltaTime : 0);
                finalspeed = Input.GetKey(KeyCode.LeftControl) ? sprintspeed : speed;
                if(Input.GetKey(KeyCode.LeftShift)&&!dashoncooldown)
                {
                    StartCoroutine(dashtimer(0.1f));
                    StartCoroutine(cooldowndash(1f));
                }
                rb.Move(forward * moveforward * finalspeed + right * moveright * finalspeed + Vector3.up * jumpvel);
                rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, turndir, turnspeed);
                break;
            case GameManager.GameState.Dash:
                rb.Move(rb.transform.forward * dashspeed);
                break;
            case GameManager.GameState.Battle2D:
                if(Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.D))
                {
                    ismoving = true;
                    forward = gameManager.closestEnemy.transform.position - rb.transform.position;
                    forward.y = 0;
                    forward = forward.normalized;
                }
                else if(Input.GetKeyUp(KeyCode.A)||Input.GetKeyUp(KeyCode.D))
                {
                    forward = gameManager.closestEnemy.transform.position - rb.transform.position;
                    forward.y = 0;
                    forward = forward.normalized;
                    ismoving = false;
                }
                jumpvel = rb.isGrounded && Input.GetKey(KeyCode.W) ? jumpforce : (!rb.isGrounded ? jumpvel - gravity * Time.deltaTime : 0);
                moveforward = Input.GetAxisRaw("Horizontal");
                bool parellel = Vector3.Dot(cam.transform.right,forward.normalized)>0;
                if (!parellel)
                {
                    moveforward = -moveforward;
                }
                Vector3 lookdir = gameManager.closestEnemy.transform.position - rb.transform.position;
                lookdir.y = 0;
                lookdir = lookdir.normalized;
                turndir = Quaternion.LookRotation(lookdir);
                rb.transform.rotation = Quaternion.Slerp(rb.transform.rotation, turndir, turnspeed);
                finalspeed = Input.GetKey(KeyCode.LeftControl) ? sprintspeed : speed;
                rb.Move(forward * moveforward * finalspeed + Vector3.up * jumpvel);
                break;

        }
    }
    IEnumerator dashtimer(float i)
    {
        gameManager.currentState = GameManager.GameState.Dash;
        yield return new WaitForSeconds(i);
        gameManager.currentState = GameManager.GameState.Explore3D;
    }
    IEnumerator cooldowndash(float i)
    {
        dashoncooldown = true;
        yield return new WaitForSeconds(i);
        dashoncooldown = false;
    }
    IEnumerator attackcooldown(int ID)
    {
        yield return new WaitForSeconds(2);
    }
}
