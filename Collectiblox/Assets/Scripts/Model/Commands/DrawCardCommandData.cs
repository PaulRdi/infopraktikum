using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Collectiblox.Model.Commands
{
    public class DrawCardCommandData : CommandData
    {
        public override PlayerKey sender => _sender;
        PlayerKey _sender;


        public DrawCardCommandData(
            PlayerKey sender)
        {
            this._sender = sender;
        }
    }
}
