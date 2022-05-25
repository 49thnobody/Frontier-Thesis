using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;

    private void Awake()
    {
        instance = this;
    }

    public Transform BaseLayout;
    public List<CardController> Bases;

    public OutputController Authority;

    public void DrawCard()
    {

    }

    public void PlaceBase(CardController card)
    {

    }

    public void DiscardBase(CardController basement)
    {

    }

    public void TakeDamage(int damage)
    {
        Authority.UpdateValue(Authority.Value - damage);
        if (Authority.Value <= 0)
            ;//lose
    }
}
