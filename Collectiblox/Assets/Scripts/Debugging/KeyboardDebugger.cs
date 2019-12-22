using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Collectiblox.Model;
using Collectiblox.Model.Commands;
using Collectiblox.Model.Rules;
namespace Collectiblox.Debugging
{
    public class KeyboardDebugger : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            DebugStuff();

        }

        private void DebugStuff()
        {
            RuleEvaluationInfo ri = default(RuleEvaluationInfo);
            if (Input.GetKeyDown(KeyCode.F))
            {
                new HandRequest(GameManager.instance.match.player1Key).Start(PlayFirstCardFromHand);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                ri = GameManager.instance.TrySendCommand(new Command<DrawCardCommandData>(
                    CommandType.DrawCardFromDeck,
                    new DrawCardCommandData(
                        GameManager.instance.match.player1Key)));

                HandRequest handRequest = new HandRequest(GameManager.instance.match.player1Key);
                handRequest.Start(DebugHand);
            }
            if (ri != null &&
                !ri.actionAllowed)
            {
                if (ri.cardInstance == null)
                    Debug.Log(ri.ruleDeniedMessage);
                else
                    Debug.Log(ri.ruleDeniedMessage + "\n" + ri.cardInstance.ToString());
            }
        }

        private void PlayFirstCardFromHand(List<ICardInstance> obj)
        {
            RuleEvaluationInfo ri = default(RuleEvaluationInfo);
            if (obj.Count == 0)
            {
                Debug.Log("request returned no cards in hand");
                return;
            }

            Vector2Int position = new Vector2Int(
                            UnityEngine.Random.Range(0, 5),
                            UnityEngine.Random.Range(0, 5));
            ri = GameManager.instance.TrySendCommand(new Command<PlayCardCommandData>(
                CommandType.PlayCard,
                new PlayCardCommandData(
                    GameManager.instance.match.player1Key,
                    obj[0],
                    position)));

            if (ri != null &&
                !ri.actionAllowed)
            {
                if (ri.cardInstance == null)
                    Debug.Log(ri.ruleDeniedMessage);
                else
                    Debug.Log(ri.ruleDeniedMessage + "\n" + ri.cardInstance.ToString());
            }
        }

        private void DebugHand(List<ICardInstance> obj)
        {
            foreach (ICardInstance i in obj)
            {
                Debug.Log(i.ToString());
            }
        }
    }
}