using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Collectiblox
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] Decklist player1Decklist;
        [SerializeField] Decklist player2Decklist;


        [SerializeField] Transform playingGridParent;
        [SerializeField] GameObject playingGridPrefab;
        [SerializeField] GameObject crystalPrefab;


        [SerializeField] GameConfig config;

        Match match;

        private void Awake()
        {

            GameConfig.current = config;
            //Create Model
            match = new Match(
                player1Decklist, 
                player2Decklist,
                config.gridSize,
                config.crystalPosition,
                config.crystalStrength);

            //Create View
            GameObject go;
            for (int y = 0; y < config.gridSize.y; y++)
            {
                for (int x = 0; x < config.gridSize.x; x++)
                {
                    go = Instantiate(playingGridPrefab, playingGridParent);
                    go.transform.localPosition = new Vector3(x, 0, y);
                }
            }

            go = Instantiate(crystalPrefab);
            go.transform.position = playingGridParent.worldToLocalMatrix * new Vector3(config.crystalPosition.x, 0, config.crystalPosition.y);

        }
    }
}