using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;


public class ShopDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text tokens;
    [SerializeField]
    private Animator summary_anim;

    [SerializeField]
    private GameStateSave states;

    private ShopSlot[] shopItems;

    private void Start()
    {
        shopItems = GetComponentsInChildren<ShopSlot>();
        GetComponent<Canvas>().enabled = false;

    }

    public void DissolveUnboughtItem()
    {
        foreach(ShopSlot item in shopItems)
        {
            //item.Dissolve();
        }
    }

    public void UpdateTokenDisplay()
    {
        tokens.text = states.Money.ToString();
    }

    public void WaitForSummaryPageAnim()
    {
        StartCoroutine( WaitAnimation());
    }

    IEnumerator WaitAnimation()
    {
        while (!summary_anim.GetCurrentAnimatorStateInfo(0).IsName("Black"))
        {
            yield return null;
        }
        GetComponent<Canvas>().enabled = true;
    }
}
