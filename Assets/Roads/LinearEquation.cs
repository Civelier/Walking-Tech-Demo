using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    public class LinearEquation
    {
        public float A;
        public float B;

        /// <summary>
        /// Makes a linear equation
        /// </summary>
        /// <param name="a">Variation factor</param>
        /// <param name="b">Initial value</param>
        public LinearEquation(float a, float b)
        {
            A = a;
            B = b;
        }

        public float GetY(float x)
        {
            return A * x + B;
        }

        public float GetX(float y)
        {
            return (y - B) / A;
        }

        public LinearEquation GetInverse()
        {
            return new LinearEquation(1 / A, B / A);
        }

        public static Vector2? Resolve(LinearEquation e1, LinearEquation e2)
        {
            var a = e1.A - e2.A;
            var b = e1.B - e2.B;
            if (b == 0) return null;
            var x = a / b;
            return new Vector2(x, e1.GetY(x));
        }

        public static Vector2? Resolve(LinearEquation e1, LinearEquation e2, float max)
        {
            var a = e1.A - e2.A;
            var b = e1.B - e2.B;
            if (b == 0) return null;
            var x = a / b;
            var y = e2.GetY(x);
            if (y <= max) return new Vector2(x, e1.GetY(x));
            return new Vector2(e2.GetX(max), max);
        }

        public static LinearEquation FromAAndPoint(float a, Vector2 point)
        {
            return new LinearEquation(a, point.y - a * point.x);
        }
    }
}
