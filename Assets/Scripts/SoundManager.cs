using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance{get; set;}
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    [Header("Sound FX")]
    public AudioSource acceptQuestSound;
    public AudioSource enterNPCSound;
    public AudioSource walkingOnGrassSound;
    public AudioSource pauseSound;
    public AudioSource unpauseSound;
    public AudioSource victorySound;
    public AudioSource declineQuestSound;
    public AudioSource gettingHitSound;
    public AudioSource walkingOnWaterSound;
    [Header("Music")]
    public AudioSource firstBGMusic;

    // Start is called before the first frame update

    public void PlaySound(AudioSource soundToPlay)
    {
        if(!soundToPlay.isPlaying)
        {
            soundToPlay.Play();
        }
    }
}
