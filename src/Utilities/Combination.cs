using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace HuntTheWumpus.Utilities
{

    public struct Pair<T>
    {
        public T A;
        public T B;
    }

    public class Pairings<T> : IEnumerable<Pair<T>>
    {


        T[] vals;
        PairEnumerator penum;
        public Pairings(T[] vals)
        {
            this.vals = vals;
            penum = new PairEnumerator(this);
        }
        public IEnumerator<Pair<T>> GetEnumerator()
        {
            return penum;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        private class PairEnumerator : IEnumerator<Pair<T>>
        {
            int i, j;
            Pairings<T> coll;

            public Pair<T> Current
            {
                get
                {
                    var p = new Pair<T>();
                    p.A = coll.vals[i];
                    p.B = coll.vals[j];
                    return p;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
                Reset();
            }

            public PairEnumerator(Pairings<T> pairings)
            {
                coll = pairings;
                i = 0;
                j = 0; //technically 1, but MoveNext gets called first
            }


            public bool MoveNext()
            {
                j++;
                if (j == coll.vals.Length)
                {
                    i++;
                    j = i + 1;
                }
                if (i == coll.vals.Length || j == coll.vals.Length)
                {
                    return false;
                }
                return true;
            }

            public void Reset()
            {
                i = 0;
                j = 0;
            }
        }
    }
}
