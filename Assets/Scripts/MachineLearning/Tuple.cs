using System;

namespace MachineLearning
{
    [Serializable]
    public class Tuple<T, U>
    {
        public T Item1;
        public U Item2;

        public Tuple(T item1, U item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }
}