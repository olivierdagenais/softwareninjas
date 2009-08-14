using System;
using System.Collections.Generic;

namespace SoftwareNinjas.Core
{
    public class PairList<F, S> : IEnumerable<Pair<F, S>>
    {
        private IList<Pair<F, S>> _list = new List<Pair<F, S>>();

        public void Add(F f, S s)
        {
            _list.Add(new Pair<F, S>(f, s));
        }

        public void Add(Pair<F, S> pair)
        {
            _list.Add(pair);
        }

        #region IEnumerable<Pair<F,S>> Members

        public IEnumerator<Pair<F, S>> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }
}
