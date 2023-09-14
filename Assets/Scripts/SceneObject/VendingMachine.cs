using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    [SerializeField] private GameObject SpawnedToken;
    [SerializeField] private GameObject SpawnedBigToken;
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private LevelStats stats;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip Deny;
    [SerializeField] private AudioClip Coin;
    [SerializeField] private AudioClip Insert;
    [SerializeField] private AudioSource EffectPlayer;

    Animator anim;
    bool AnimationTrigger = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void GetRandomDrop()
    {
        EffectPlayer.clip = Insert;
        EffectPlayer.Play();
        StartCoroutine(WaitAnimation());

    }

    void RandomToken()
    {
        if (stats.ExtraStepsLeft > 0)
        {
            int seed = Random.Range(0, 100);
            if (seed % 2 == 0)
            {
                SpawnToken(true);
            }
            else
            {
                SpawnToken(false);
            }
        }
        else
        {
            EffectPlayer.clip = Deny;
            EffectPlayer.Play();
        }
        AnimationTrigger = false;
    }

    IEnumerator WaitAnimation()
    {
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("VendingActive") || !AnimationTrigger)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("VendingActive"))
                AnimationTrigger = true;
            yield return null;
        }
        RandomToken();
    }

    public void SpawnToken(bool spawnExtra)
    {
        EffectPlayer.clip = Coin;
        EffectPlayer.Play();
        GameObject spawned;
        if (!spawnExtra)
            spawned = Instantiate(SpawnedToken);
        else
            spawned = Instantiate(SpawnedBigToken);
        spawned.transform.position = SpawnPoint.position;
        spawned.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f, 1f), 1f));
    }
}
