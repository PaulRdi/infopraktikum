using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Collectiblox.Model;
using Collectiblox.Model.Commands;
using Collectiblox.Controller;
using Collectiblox.Model.DB;
using Collectiblox.Model.Rules;
using System;

namespace Collectiblox
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<GameManager>();
                return _instance;
            }
        }
        static GameManager _instance;

        [SerializeField] Decklist player1Decklist;
        [SerializeField] Decklist player2Decklist;

        [SerializeField] public Transform playingGridParent;
        [SerializeField] GameObject gridCellPrefab;
        [SerializeField] GameObject crystalPrefab;

        [SerializeField] GameConfig config;

        public MatchData match;
        public Dictionary<IFieldEntity, MonoBehaviour> entityToBehaviour;
        public List<ICommandExecutionListener> commandExecutionListeners;
        public List<MatchState> stateOrder;        

        public int currentStateOrderIndex;
        public CommandManager commandManager;
        public Validator validator;
        public RequestManager requestManager;

        public RuleEvaluationInfo TrySendCommand(ICommand command)
        {
            return commandManager.TrySendCommand(this, command);
        }

        private void OnDestroy()
        {
            match.MonsterCreated -= Match_MonsterCreated;
        }

        #region Initialization
        private void Init()
        {
            _instance = this;
            InitGame();
        }

        private void InitGame()
        {
            GameConfig.current = config;
            entityToBehaviour = new Dictionary<IFieldEntity, MonoBehaviour>();
            commandExecutionListeners = new List<ICommandExecutionListener>();
            stateOrder = new List<MatchState>();
            commandManager = new CommandManager(config.rules);
            validator = new Validator();

            //Create Model
            match = new MatchData(
                player1Decklist,
                player2Decklist,
                config.gridSize,
                config.crystalPosition,
                config.crystalStrength);

#if NETWORKING_SERVER
#else
            requestManager = new RequestManager(match);
#endif
            SetupMatchStates();

            //Create View
            GameObject go;
            for (int y = 0; y < config.gridSize.y; y++)
            {
                for (int x = 0; x < config.gridSize.x; x++)
                {
                    go = Instantiate(gridCellPrefab, playingGridParent);
                    go.transform.localPosition = new Vector3(x, 0, y);
                    go.GetComponentInChildren<GridCellController>().Init(new Vector2Int(x, y));
                }
            }

            foreach (MonoBehaviour mb in FindObjectsOfType<MonoBehaviour>())
            {
                if (mb is ICommandExecutionListener)
                {
                    commandExecutionListeners.Add((ICommandExecutionListener)mb);
                }
            }

            go = Instantiate(crystalPrefab);
            go.transform.position = playingGridParent.worldToLocalMatrix * new Vector3(config.crystalPosition.x, 0, config.crystalPosition.y);

            match.MonsterCreated += Match_MonsterCreated;
        }

        private void SetupMatchStates()
        {
            stateOrder.Add(
                new MatchState(
                    MatchStateType.Draw,
                    match.player1Key,
                    match.player2Key));
            stateOrder.Add(
                new MatchState(
                    MatchStateType.Main,
                    match.player1Key,
                    match.player2Key));
            stateOrder.Add(
                new MatchState(
                    MatchStateType.Evaluate,
                    match.player1Key,
                    match.player2Key));
            stateOrder.Add(
                new MatchState(
                    MatchStateType.Draw,
                    match.player2Key,
                    match.player1Key));
            stateOrder.Add(
                new MatchState(
                    MatchStateType.Main,
                    match.player2Key,
                    match.player1Key));
            stateOrder.Add(
                new MatchState(
                    MatchStateType.Evaluate,
                    match.player2Key,
                    match.player1Key));
            stateOrder.Add(
                new MatchState(
                    MatchStateType.IncreaseMana,
                    match.player2Key,
                    match.player1Key));
            stateOrder.Add(
                new MatchState(
                    MatchStateType.Draw,
                    match.player2Key,
                    match.player1Key));
            stateOrder.Add(
                new MatchState(
                    MatchStateType.Main,
                    match.player2Key,
                    match.player1Key));
            stateOrder.Add(
                new MatchState(
                    MatchStateType.Evaluate,
                    match.player2Key,
                    match.player1Key));
            stateOrder.Add(
                new MatchState(
                    MatchStateType.Draw,
                    match.player1Key,
                    match.player2Key));
            stateOrder.Add(
                new MatchState(
                    MatchStateType.Main,
                    match.player1Key,
                    match.player2Key));
            stateOrder.Add(
                new MatchState(
                    MatchStateType.Evaluate,
                    match.player1Key,
                    match.player2Key));
            stateOrder.Add(
                new MatchState(
                    MatchStateType.IncreaseMana,
                    match.player1Key,
                    match.player2Key));
        }
#endregion

        private void Match_MonsterCreated(FieldEntity<CardInstance<Monster>> obj)
        {
            GameObject prefab = obj.entity.data.prefab;
            GameObject instance = Instantiate(prefab);
            instance.GetComponent<BoardEntity>().Init(obj);
            instance.transform.position = Convert.GridToWorld(playingGridParent, obj.gridPos);
        }

        void Update()
        {

            if (validator.isValidated)
            {
                if (commandManager.TryExecuteNextCommand(this, out ICommand cmd))
                {
                    commandExecutionListeners.ForEach(e => e.CommandExecuted(cmd, this));
                }
            }
        }     

        private void Awake()
        {
            Init();
        }

        public static Vector3 QuantizeToGrid(Vector3 position)
        {
            Transform og = instance.playingGridParent;
            Vector3 relativePos = position - og.position;
            Vector3 gridCellSize = instance.gridCellPrefab.transform.lossyScale;


            relativePos = new Vector3(
                gridCellSize.x == 0 ? 1.0f : Mathf.Floor((relativePos.x + gridCellSize.x * .5f) / gridCellSize.x),
                gridCellSize.y == 0 ? 1.0f : Mathf.Floor((relativePos.y + gridCellSize.y * .5f) / gridCellSize.y),
                gridCellSize.z == 0 ? 1.0f : Mathf.Floor((relativePos.z + gridCellSize.z * .5f) / gridCellSize.z));
           // relativePos *= 2;
            relativePos.y = position.y;

            return relativePos;
        }

        public static Vector3Int WorldToGrid(Vector3 position)
        {
            Transform og = instance.playingGridParent;
            Vector3 relativePos = position - og.position;
            Vector3 gridCellSize = instance.gridCellPrefab.transform.lossyScale;

            return new Vector3Int(
                gridCellSize.x == 0 ? 1 : (int)Math.Floor((relativePos.x + gridCellSize.x * .5f) / gridCellSize.x),
                gridCellSize.y == 0 ? 1 : (int)Math.Floor((relativePos.y + gridCellSize.y * .5f) / gridCellSize.y),
                gridCellSize.z == 0 ? 1 : (int)Math.Floor((relativePos.z + gridCellSize.z * .5f) / gridCellSize.z));
        }
    }
}