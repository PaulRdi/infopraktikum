using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Collectiblox
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] Decklist player1Decklist;
        [SerializeField] Decklist player2Decklist;

        Match match;

        private void Awake()
        {
            match = new Match(player1Decklist, player2Decklist);
            

        }
    }
}