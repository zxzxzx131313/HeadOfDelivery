using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ShopSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator anim;
    private Dissolve dissolve;
    private bool bought = false;

    public GameStateSave states;

    [Header("slot setting")]
    [SerializeField]
    private int cost;
    [SerializeField]
    private FaceAbilityCode ability;
    [SerializeField]
    private GameEvent OnBoughtAbility;
    [SerializeField]
    private GameEvent OnNSF;
    [SerializeField]
    private Canvas description;
    [SerializeField]
    private TMP_Text cost_text;
    [SerializeField]
    private CanvasGroup detail;
    [SerializeField]
    private Canvas boughtMessage;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        description.enabled = false;
        cost_text.text = cost.ToString();
        dissolve = GetComponent<Dissolve>();
        boughtMessage.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!bought)
        {
            anim.SetTrigger("MouseOver");
            description.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        anim.SetTrigger("MouseExit");
        description.enabled = false;
        
    }

    public void BuyAbility()
    {
        if (!states.HasAbility(ability))
        {
            if (states.TransactionsCount(ExpenseType.Token) >= cost)
            {
                states.AddTransaction(ExpenseType.Token, -cost);
                states.AddNewAbility(ability);
                bought = true;
                ShowBoughtMessage();
                OnBoughtAbility.Raise();
            }
            else
            {
                OnNSF.Raise();
            }
        }
    }

    public void Dissolve()
    {
        if (!bought)
        {
            dissolve.Vanish();
            dissolve.VanishCanvas(detail);
        }
    }

    public void ShowBoughtMessage()
    {
        boughtMessage.enabled = true;
        boughtMessage.GetComponentInChildren<Animator>().SetTrigger("Sold");
    }
}
