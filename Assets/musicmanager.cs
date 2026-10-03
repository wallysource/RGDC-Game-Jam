using UnityEngine;

public class musicmanager : MonoBehaviour
{
    public GameManager gameManager;
    public AudioSource musinstrumental;
    public AudioSource musbattle;
    public bool isPlaying=false;
    void Start()
    {
        musinstrumental.volume = 0;
        musbattle.volume = 0;
        musinstrumental.Play();
        musbattle.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlaying)
        {
            if(musinstrumental.volume == 0 && musbattle.volume == 0)
            {
                musbattle.Play();
                musinstrumental.Play();
            }
            if(gameManager.currentState == GameManager.GameState.Battle2D)
            {
                musinstrumental.volume = Mathf.Lerp(musinstrumental.volume, 0, 0.01f);
                musbattle.volume = Mathf.Lerp(musbattle.volume, 1, 0.01f);
            }
            else
            {
                musinstrumental.volume = Mathf.Lerp(musinstrumental.volume, 1, 0.01f);
                musbattle.volume = Mathf.Lerp(musbattle.volume, 0, 0.01f);
            }
        }
        else
        {
            musinstrumental.volume = Mathf.Lerp(musinstrumental.volume,0,0.01f);
            musbattle.volume = Mathf.Lerp(musbattle.volume,0,0.01f);
            if (musbattle.volume < 0.001f)
                musbattle.volume = 0f;
            if (musinstrumental.volume < 0.001f)
                musinstrumental.volume = 0f;
            if(musinstrumental.volume == 0 && musbattle.volume == 0)
            {
                musbattle.Stop();
                musinstrumental.Stop();
            }
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            isPlaying = !isPlaying;
        }
    }
}
