using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Collectiblox.Model;
using Collectiblox.Model.Commands;
using Collectiblox;
using System;

namespace Collectiblox.Debugging
{
    public class ParticleSystemTest : MonoBehaviour, IValidator, ICommandExecutionListener
    {
        [SerializeField] ParticleSystem particles;
        List<GameObject> systems;

        private void Awake()
        {
            systems = new List<GameObject>();
        }
        public event Action<IValidator> Validated;

        public void CommandExecuted(ICommand command, GameManager gm)
        {
            if (command.TryGetData<PlayCardCommandData>(out PlayCardCommandData cmdData))
            {
                GameManager.instance.validator.AddValidationTarget(this);
                Vector3 pos = Convert.GridToWorld(gm.playingGridParent.localToWorldMatrix, cmdData.targetTile);
                ParticleSystem ps = Instantiate(particles, pos, Quaternion.identity);
                systems.Add(ps.gameObject);
                ps.GetComponent<DestroyNotifier>().Destroyed += ParticleSystemTest_Destroyed;
            }
        }

        private void ParticleSystemTest_Destroyed(GameObject go)
        {
            systems.Remove(go);

            if (systems.Count == 0)
                Validated?.Invoke(this);
        }
    }
}