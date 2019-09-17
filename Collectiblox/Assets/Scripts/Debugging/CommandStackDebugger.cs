using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Collectiblox.Model.Commands;

namespace Collectiblox.Debugging
{
    public class CommandStackDebugger : MonoBehaviour
    {
        [SerializeField] GameObject commandStackText;
        GameManager gm;

        Dictionary<ICommand, TextMeshProUGUI> commandToText;

        private void Awake()
        {
            gm = FindObjectOfType<GameManager>();
            commandToText = new Dictionary<ICommand, TextMeshProUGUI>();
            if (gm == null)
            {
                Debug.LogWarning("Could not find GameManager in " + this.ToString());
                Destroy(this);
            }
        }

        private void Update()
        {
            List<ICommand> toRemove = new List<ICommand>();
            foreach(ICommand cmd in commandToText.Keys)
            {
                if (!gm.commandManager.commandStack.Contains(cmd))
                    toRemove.Add(cmd);
            }

            foreach(ICommand cmd in gm.commandManager.commandStack)
            {
                if (!commandToText.ContainsKey(cmd))
                {
                    commandToText.Add(cmd, Instantiate(commandStackText, this.transform).GetComponent<TextMeshProUGUI>());
                    commandToText[cmd].text = cmd.ToString();
                }
            }

            foreach(ICommand cmd in toRemove)
            {
                Destroy(commandToText[cmd].gameObject);
                commandToText.Remove(cmd);
            }            
        }
    }
}