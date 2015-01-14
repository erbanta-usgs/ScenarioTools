using System;
using System.Collections.Generic;
using System.Text;

namespace ScenarioTools.Util
{
    public class Pair<T, S>
    {
        T element1;
        S element2;

        public Pair(T element1, S element2)
        {
            this.element1 = element1;
            this.element2 = element2;
        }
        public T getElement1()
        {
            return element1;
        }
        public S getElement2()
        {
            return element2;
        }
    }

}
