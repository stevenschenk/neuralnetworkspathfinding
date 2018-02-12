using System;
using UnityEngine;

namespace DefaultNamespace
{
    public static class Extensions
    {
        public static float[] Minus(this float[] a, float[] b)
        {
            if(a.Length != b.Length)
                throw new Exception("Can't substract two unequal vectors");

            var newVector = new float[a.Length];
            
            for (var i = 0; i < a.Length; i++)
            {
                newVector[i] = a[i] - b[i];
            }

            return newVector;
        }
        
        public static float[] Plus(this float[] a, float[] b)
        {
            if(a.Length != b.Length)
                throw new Exception("Can't substract two unequal vectors");

            var newVector = new float[a.Length];
            
            for (var i = 0; i < a.Length; i++)
            {
                newVector[i] = a[i] + b[i];
            }

            return newVector;
        }

        public static float VectorLength(this float[] a)
        {
            var length = 0f;
            for (var i = 0; i < a.Length; i++)
            {
                length += Mathf.Pow(a[i], 2);
            }

            return Mathf.Sqrt(length);
        }
    }
}