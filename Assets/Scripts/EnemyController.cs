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

    public void DrawCard()
    {

    }

    public void PlaceBase(CardController card)
    {

    }
}
