using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Collectiblox.Model
{
    public class DrawOrder
    {
        int seed;
        int currRandom;
        public List<ICardInstance> drawOrder;
        /// <summary>
        /// The order of cards the drawOrder originally had after the constructor was called.
        /// </summary>
        public List<ICardInstance> ogOrder;

        public DrawOrder (List<ICardInstance> cards)
        {
            this.drawOrder = new List<ICardInstance>(cards);
            seed = UnityEngine.Random.Range(0, 100000000);
            currRandom = seed;
            Shuffle<ICardInstance>(ref drawOrder);
            ogOrder = new List<ICardInstance>(drawOrder);
        }

        public bool TryDraw(out ICardInstance card)
        {
            card = null;

            if (drawOrder.Count > 0)
            {
                card = drawOrder[0];
                drawOrder.RemoveAt(0);
                return true;
            }
            return false;
        }

        //fisher yates shuffle: https://pdfs.semanticscholar.org/d9f6/65a955a2706c7dd3d4fe47e959f0d118932b.pdf
        private void Shuffle<T>(ref List<T> cards)
        {
            for (int n = cards.Count - 1;  n > 0; n--)
            {
                int rnd = NextRandom();
                int mod = Math.Abs(rnd % n);
                int k = n - mod;
                T tmp = cards[k];
                cards[k] = cards[n];
                cards[n] = tmp;
            }
        }
        //simple lcrg: https://wiki.freepascal.org/Marsaglia's_pseudo_random_number_generators
        int NextRandom()
        {
            currRandom = currRandom * 69069 + 1234567;
            return currRandom;
        }

        public override string ToString()
        {
            string output = "Draw order: \n";
            foreach(ICardInstance instance in drawOrder)
            {
                output += instance.baseData.cardName + "\n";
            }

            return output;
        }
    }
}
