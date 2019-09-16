using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Collectiblox.Model;
using Collectiblox.Model.Commands;
using Collectiblox.Controller;
using Collectiblox.Model.DB;
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
        [SerializeField] GameObject playingGridPrefab;
        [SerializeField] GameObject crystalPrefab;

        [SerializeField] GameConfig config;

        public MatchData match;
        public Dictionary<IFieldEntity, MonoBehaviour> entityToBehaviour;
        public List<ICommandExecutionListener> commandExecutionListeners;
        public List<MatchState> stateOrder;
        public int currentStateOrderIndex;
        public CommandManager commandManager;
        public Validator validator;

        public bool TrySendCommand(ICommand command)
        {
            return commandManager.TrySendCommand(command);
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
            commandManager = new CommandManager();
            validator = new Validator();

            //Create Model
            match = new MatchData(
                player1Decklist,
                player2Decklist,
                config.gridSize,
                config.crystalPosition,
                config.crystalStrength);

            SetupMatchStates();

            //Create View
            GameObject go;
            for (int y = 0; y < config.gridSize.y; y++)
            {
                for (int x = 0; x < config.gridSize.x; x++)
                {
                    go = Instantiate(playingGridPrefab, playingGridParent);
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

            instance.transform.position = Convert.GridToWorld(playingGridParent.localToWorldMatrix, obj.gridPos);
        }





        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                TrySendCommand(new Command<PlayCardCommandData>(
                    CommandType.PlayCard, 
                    new PlayCardCommandData(
                        match.player1Key,
                        match.cardInstances.ElementAt(0).Key,
                        new Vector2Int(
                            UnityEngine.Random.Range(0,5), 
                            UnityEngine.Random.Range(0,5)))));
            }


            if (validator.isValidated)
            {
                ICommand cmd;
                if (commandManager.TryExecuteNextCommand(this, out cmd))
                {
                    commandExecutionListeners.ForEach(e => e.CommandExecuted(cmd, this));
                }
            }
        }
        private void Awake()
        {
            Init();
        }
    }
}