using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Roads
{
    public static class BoundsExtensions
    {
        public static float GetForward(this Bounds bounds)
        {
            return bounds.center.z + bounds.extents.z;
        }

        public static Bounds SetForward(this Bounds bounds, float forward)
        {
            float delta = forward - bounds.GetForward();
            bounds.center += Vector3.forward * delta / 2;
            bounds.size += Vector3.forward * delta;
            return bounds;
        }

        public static float GetBack(this Bounds bounds)
        {
            return bounds.center.z - bounds.extents.z;
        }

        public static Bounds SetBack(this Bounds bounds, float back)
        {
            float delta = back - bounds.GetBack();
            bounds.center += Vector3.back * delta / 2;
            bounds.size += Vector3.back * delta;
            return bounds;
        }

        public static float GetRight(this Bounds bounds)
        {
            return bounds.center.x + bounds.extents.x;
        }

        public static Bounds SetRight(this Bounds bounds, float right)
        {
            float delta = right - bounds.GetRight();
            bounds.center += Vector3.right * delta / 2;
            bounds.size += Vector3.right * delta;
            return bounds;
        }

        public static float GetLeft(this Bounds bounds)
        {
            return bounds.center.x - bounds.extents.x;
        }

        public static Bounds SetLeft(this Bounds bounds, float left)
        {
            float delta = left - bounds.GetLeft();
            bounds.center += Vector3.left * delta / 2;
            bounds.size += Vector3.left * delta;
            return bounds;
        }

        public static float GetUp(this Bounds bounds)
        {
            return bounds.center.y + bounds.extents.y;
        }

        public static Bounds SetUp(this Bounds bounds, float up)
        {
            float delta = up - bounds.GetUp();
            bounds.center += Vector3.up * delta / 2;
            bounds.size += Vector3.up * delta;
            return bounds;
        }

        public static float GetDown(this Bounds bounds)
        {
            return bounds.center.y - bounds.extents.y;
        }

        public static Bounds SetDown(this Bounds bounds, float down)
        {
            float delta = down - bounds.GetDown();
            bounds.center += Vector3.down * delta / 2;
            bounds.size += Vector3.down * delta;
            return bounds;
        }

        public static Bounds FromSides(float forward, float back, float left, float right, float up, float down)
        {
            Vector3 size = new Vector3(left - right, forward - back, up - down);
            Vector3 center = new Vector3(left, back, down) + (size / 2);
            return new Bounds(center, size);
        }

    }
}
