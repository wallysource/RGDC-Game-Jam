using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public enum GameState
    {
        Explore3D,
        Battle2D,
        Dash,
        Death,
    }
    public GameObject[] enemies;
    public CharacterController rb;
    public GameState currentState = GameState.Explore3D;
    float closestdist;
    public GameObject closestEnemy = null;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        findclosestenemy();
    }
    void findclosestenemy()
    {
        closestdist = 9223372036854775807;
        closestEnemy=null;
        foreach(GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(rb.transform.position,enemy.transform.position);
            if(distance<closestdist)
            {
                closestdist = distance;
                closestEnemy = enemy;
            }
        }
    }
}
