using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Roads
{
#if UNITY_EDITOR
    [CustomEditor(typeof(PathNode))]
    [CanEditMultipleObjects]
    public class PathNodeEditor : Editor
    {
        Object selected;
        int state = -1;
        public override void OnInspectorGUI()
        {
            if (Event.current.commandName == "ObjectSelectorUpdated") 
                selected = EditorGUIUtility.GetObjectPickerObject();
            base.OnInspectorGUI();
            var t = (PathNode)target;
            if (GUILayout.Button("Connect path as incomming"))
            {
                EditorGUIUtility.ShowObjectPicker<PathRoadLayout>(null, true, "", EditorGUIUtility.GetObjectPickerControlID());

                state = 0;
            }
            if (GUILayout.Button("Connect path as outgoing"))
            {
                EditorGUIUtility.ShowObjectPicker<PathRoadLayout>(null, true, "", EditorGUIUtility.GetObjectPickerControlID());

                state = 1;
            }
            if (GUILayout.Button("Merge node to node"))
            {
                EditorGUIUtility.ShowObjectPicker<PathNode>(null, true, "", EditorGUIUtility.GetObjectPickerControlID());

                state = 2;
            }

            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                switch (state)
                {
                    case 0:
                        {
                            var o = ((GameObject)selected).GetComponent<PathRoadLayout>();
                            t.AddAsIncomming(o);
                        }
                        break;
                    case 1:
                        {
                            var o = ((GameObject)selected).GetComponent<PathRoadLayout>();
                            t.AddAsOutgoing(o);
                        }
                        break;
                    case 2:
                        {
                            var o = ((GameObject)selected).GetComponent<PathNode>();
                            t.MergeTo(o);
                            // Destroy(target);
                        }
                        break;
                    default:
                        break;
                }
                state = -1;
            }

            if (GUILayout.Button("Create incomming road"))
            {
                var pos = t.Position;
                var obj = Instantiate(PathFactory.Instance.PathPrefab);
                var layout = obj.GetComponent<PathRoadLayout>();
                layout.Head = t;
                layout.Path.bezierPath.AddSegmentToEnd(new Vector3(pos.x + 1, pos.y, pos.z));
                layout.Path.bezierPath.AddSegmentToEnd(pos);
                layout.Path.bezierPath.DeleteSegment(1);
                layout.Path.bezierPath.DeleteSegment(0);
            }
            if (GUILayout.Button("Create outgoing road"))
            {
                var pos = t.Position;
                var obj = Instantiate(PathFactory.Instance.PathPrefab);
                var layout = obj.GetComponent<PathRoadLayout>();
                layout.Tail = t;
                layout.Path.bezierPath.AddSegmentToEnd(pos);
                layout.Path.bezierPath.AddSegmentToEnd(new Vector3(pos.x + 1, pos.y, pos.z));
                layout.Path.bezierPath.DeleteSegment(1);
                layout.Path.bezierPath.DeleteSegment(0);
            }
        }
    }
#endif

    [ExecuteInEditMode]
    public class PathNode : MonoBehaviour
    {
        public List<PathRoadLayout> IncommingRoads = new List<PathRoadLayout>();
        public List<PathRoadLayout> OutgoingRoads = new List<PathRoadLayout>();

        public event System.Action<Vector3> NodeMoved;

        private Vector3 _lastPosition;

        public Vector3 Position
        {
            get => transform.position;
            set
            {
                transform.position = value;
                ForceUpdate();
            }
        }

        public void ForceUpdate()
        {
            //System.Action<Vector3> handler = NodeMoved;
            //handler?.Invoke(Position);
            foreach (var path in IncommingRoads)
            {
                path.SetGlobalPositionPoint(-1, Position);
            }
            foreach (var path in OutgoingRoads)
            {
                path.SetGlobalPositionPoint(0, Position);
            }
        }

        public void AddAsIncomming(PathRoadLayout path)
        {
            if (!IncommingRoads.Contains(path))
            {
                IncommingRoads.Add(path);
                path.Head = this;
            }
        }

        public void AddAsOutgoing(PathRoadLayout path)
        {
            if (!OutgoingRoads.Contains(path))
            {
                OutgoingRoads.Add(path);
                path.Tail = this;
            }
        }

        public void RemoveIncomming(PathRoadLayout path)
        {
            if (IncommingRoads.Contains(path))
            {
                IncommingRoads.Remove(path);
                if (IncommingRoads.Count == 0 && OutgoingRoads.Count == 0) MainThreadDispatcher.Schedule(() => Destroy(gameObject));
            }
        }

        public void RemoveOutgoing(PathRoadLayout path)
        {
            if (OutgoingRoads.Contains(path))
            {
                OutgoingRoads.Remove(path);
                
                if (IncommingRoads.Count == 0 && OutgoingRoads.Count == 0) MainThreadDispatcher.Schedule(() => Destroy(gameObject));
            }
        }

        public void MergeFrom(PathNode node)
        {
            foreach (var path in node.OutgoingRoads)
            {
                path.Tail = this;
            }
            foreach (var path in node.IncommingRoads)
            {
                path.Head = this;
            }
            ForceUpdate();
            Destroy(node.gameObject);
        }

        public void MergeTo(PathNode node)
        {
            node.MergeFrom(this);
            //MainThreadDispatcher.Schedule(() => DestroyImmediate(this));
        }

#if UNITY_EDITOR

        Vector3 GetNodePosition()
        {
            return IncommingRoads.FirstOrDefault()?.GetGlobalPositionPoint(-1) ?? (OutgoingRoads.FirstOrDefault()?.GetGlobalPositionPoint(0) ?? new Vector3());
        }

        void Update()
        {
            var pos = GetNodePosition();
            if (pos != Position)
            {
                transform.position = pos;
            }
        }
#endif
    }
}