using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Assets.Scripts
{
    public static class PropertyDrawerUtilities
    {
        public static Rect GetRect(Rect position, int line)
        {
            float y = EditorGUIUtility.singleLineHeight + 2 * line;
            return new Rect(position.x, position.y + y, position.width, position.height);
        }

        public static float GetHeight(int numberOfFields)
        {
            return (EditorGUIUtility.singleLineHeight + 2) * numberOfFields;
        }

        public static Rect GetRectSize(Rect position, int numberOfLines)
        {
            return new Rect(position.x, position.y, position.width, GetHeight(numberOfLines));
        }
    }
}
#endif
