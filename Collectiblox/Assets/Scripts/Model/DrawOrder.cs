using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Collectiblox
{
    public class DrawOrder
    {
        int seed;
        int currRandom;
        public List<CardInstance> drawOrder;

        public DrawOrder (List<CardInstance> cards)
        {
            this.drawOrder = new List<CardInstance>(cards);
            seed = Time.renderedFrameCount;
            currRandom = seed;
            Shuffle<CardInstance>(ref drawOrder);
        }

        //fisher yates shuffle: https://pdfs.semanticscholar.org/d9f6/65a955a2706c7dd3d4fe47e959f0d118932b.pdf
        private void Shuffle<T>(ref List<T> cards)
        {
            for (int n = cards.Count - 1;  n >= 0; --n)
            {
                int k = n - NextRandom() % n;
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
    }
}
