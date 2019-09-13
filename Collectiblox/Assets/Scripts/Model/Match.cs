using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Collectiblox
{
    public class Match
    {
        Dictionary<PlayerKey, PlayerData> playerDatas;
        Dictionary<ICardInstance, PlayerKey> cardInstances;
        Dictionary<Vector2Int, IFieldEntity> fieldEntities;


        public PlayerKey player1Key;
        public PlayerKey player2Key;

        Vector2Int gridSize;

        Crystal crystal;

        public Match(
            Decklist deckPlayer1, 
            Decklist deckPlayer2,
            Vector2Int gridSizeX,
            Vector2Int crystalPosition,
            int crystalStrength)
        {
            playerDatas = new Dictionary<PlayerKey, PlayerData>();
            cardInstances = new Dictionary<ICardInstance, PlayerKey>();
            fieldEntities = new Dictionary<Vector2Int, IFieldEntity>();

            player1Key = new PlayerKey();
            player2Key = new PlayerKey();

            List<ICardInstance> player1Instances = new List<ICardInstance>();
            List<ICardInstance> player2Instances = new List<ICardInstance>();

            foreach (DecklistEntry entry in deckPlayer1.cards)
            {
                ICardData cardData = Cards.DB[entry.cardName];
                ICardInstance cardInstance = Cards.CreateCardInstance(cardData, player1Key);

                cardInstances.Add(cardInstance, player1Key);
                player1Instances.Add(cardInstance);
            }
            foreach (DecklistEntry entry in deckPlayer2.cards)
            {
                ICardData cardData = Cards.DB[entry.cardName];
                ICardInstance cardInstance = Cards.CreateCardInstance(cardData, player2Key);

                cardInstances.Add(cardInstance, player1Key);
                player2Instances.Add(cardInstance);
            }
            DrawOrder drawOrderP1 = new DrawOrder(player1Instances);
            DrawOrder drawOrderP2 = new DrawOrder(player2Instances);

            playerDatas.Add(player1Key, new PlayerData(drawOrderP1));
            playerDatas.Add(player2Key, new PlayerData(drawOrderP2));

            crystal = new Crystal(crystalStrength);

            fieldEntities.Add(
                crystalPosition,
                new FieldEntity<Crystal>(crystal, crystalPosition));
        }

        public FieldEntity<CardInstance<Monster>> SpawnMonster(CardInstance<Monster> monster, Vector2Int position)
        {
            return new FieldEntity<CardInstance<Monster>>(monster, position);
        }
    }
}
