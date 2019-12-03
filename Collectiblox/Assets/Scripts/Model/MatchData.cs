using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Collectiblox.Model
{
    public class MatchData
    {
        public event Action<FieldEntity<CardInstance<Monster>>> MonsterCreated;

        public Dictionary<PlayerKey, PlayerData> playerDatas;
        public Dictionary<ICardInstance, PlayerKey> cardInstances;
        public Dictionary<Vector2Int, IFieldEntity> fieldEntities;


        public PlayerKey player1Key;
        public PlayerKey player2Key;

        Vector2Int gridSize;

        public Crystal crystal => _crystal;
        public Vector2Int crystalPosition;
        Crystal _crystal;

        public MatchState currentState;

        public MatchData(
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

            playerDatas.Add(player1Key, new PlayerData(drawOrderP1, player1Key, 5, 5));
            playerDatas.Add(player2Key, new PlayerData(drawOrderP2, player2Key, 5, 5));

            _crystal = new Crystal(crystalStrength);

            fieldEntities.Add(
                crystalPosition,
                new FieldEntity<Crystal>(_crystal, crystalPosition));
            this.crystalPosition = crystalPosition;
            currentState = new MatchState(MatchStateType.Idle, player1Key, player2Key);
        }

        

        public FieldEntity<CardInstance<Monster>> SpawnMonster(
            CardInstance<Monster> monster, 
            Vector2Int position, 
            PlayerKey spawningPlayer = null)
        {
            FieldEntity<CardInstance<Monster>> entity = new FieldEntity<CardInstance<Monster>>(monster, position);
            if (spawningPlayer != null)
            {
                playerDatas[spawningPlayer].fieldEntities.Add(entity);
            }
            MonsterCreated?.Invoke(entity);
            return entity;
        }
        /// <summary>
        /// Get the strength coming in to one tile for one player!
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="requestingPlayer"></param>
        /// <returns></returns>
        public int GetStrengthForPlayerAtPosition(Vector2Int pos, PlayerKey requestingPlayer)
        {
            int val = GetStrengthForPlayerAtPositionRecursive(pos, new Vector2Int(-1, 0), requestingPlayer) 
                    + GetStrengthForPlayerAtPositionRecursive(pos, new Vector2Int(1, 0), requestingPlayer)
                    + GetStrengthForPlayerAtPositionRecursive(pos, new Vector2Int(0, 1), requestingPlayer)
                    + GetStrengthForPlayerAtPositionRecursive(pos, new Vector2Int(0, -1), requestingPlayer);

            return val;
        }

        private int GetStrengthForPlayerAtPositionRecursive(Vector2Int pos, Vector2Int dir, PlayerKey requestingPlayer)
        {
            pos += dir;
            if (!fieldEntities.ContainsKey(pos))
            {
                return 0;
            }
            IFieldEntity fieldEntity = fieldEntities[pos];
            if (fieldEntity.TryGetEntity<CardInstance<Monster>>(out CardInstance<Monster> monster))
            {
                if (cardInstances[monster] == requestingPlayer)
                    return GetStrengthForPlayerAtPositionRecursive(pos, dir, requestingPlayer) + monster.data.combatValue;
                else
                    return 0;
            }
            return 0;
        }
    }
}
