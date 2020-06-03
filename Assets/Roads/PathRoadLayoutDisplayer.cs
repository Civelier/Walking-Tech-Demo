using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
#endif

namespace Roads
{
#if UNITY_EDITOR
    [CustomEditor(typeof(PathRoadLayout))]
    [CanEditMultipleObjects]
    public class PathRoadLayoutDisplayer : Editor
    {
        Object selected;
        int stateHead = -1;
        int stateTail = -1;
        int stateSideRoads = -1;
        public override void OnInspectorGUI()
        {
            var t = (PathRoadLayout)target;
            base.OnInspectorGUI();
            //if (GUILayout.Button("Create road at start"))
            //{
            //    var pos = t[0];
            //    var obj = Instantiate(PathFactory.Instance.PathPrefab);
            //    var layout = obj.GetComponent<PathRoadLayout>();
            //    t.StartPaths.Add(layout);
            //    layout.ThisPath.bezierPath.AddSegmentToEnd(new Vector3(pos.x + 1, pos.y, pos.z));
            //    layout.ThisPath.bezierPath.AddSegmentToEnd(pos);
            //    layout.ThisPath.bezierPath.DeleteSegment(1);
            //    layout.ThisPath.bezierPath.DeleteSegment(0);
            //    layout.EndPaths.Add(t);
            //}
            //if (GUILayout.Button("Create road at end"))
            //{
            //    var pos = t[-1];
            //    var obj = Instantiate(PathFactory.Instance.PathPrefab);
            //    var layout = obj.GetComponent<PathRoadLayout>();
            //    t.EndPaths.Add(layout);
            //    layout.ThisPath.bezierPath.AddSegmentToEnd(pos);
            //    layout.ThisPath.bezierPath.AddSegmentToEnd(new Vector3(pos.x + 1, pos.y, pos.z));
            //    layout.ThisPath.bezierPath.DeleteSegment(1);
            //    layout.ThisPath.bezierPath.DeleteSegment(0);
            //    layout.StartPaths.Add(t);
            //}
            if (GUILayout.Button("Reset event"))
            {
                t.ResetHandle();
            }
            if (GUILayout.Button("Generate nodes"))
            {
                t.GenerateNodes();
            }
            if (GUILayout.Button("Update"))
            {
                t.EnableNotifications();
                t.OnUpdate();
            }
            if (GUILayout.Button("Set left road"))
            {
                EditorGUIUtility.ShowObjectPicker<Road>(null, true, "", EditorGUIUtility.GetObjectPickerControlID());
                stateSideRoads = 0;
            }
            if (GUILayout.Button("Remove left road"))
            {
                t.LeftRoad.RightRoad = null;
                t.LeftRoad = null;
            }
            if (GUILayout.Button("Set right road"))
            {
                EditorGUIUtility.ShowObjectPicker<Road>(null, true, "", EditorGUIUtility.GetObjectPickerControlID());
                stateSideRoads = 1;
            }
            if (GUILayout.Button("Remove right road"))
            {
                t.RightRoad.LeftRoad = null;
                t.RightRoad = null;
            }

            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
                selected = EditorGUIUtility.GetObjectPickerObject();
                switch (stateSideRoads)
                {
                    case 0:
                        t.LeftRoad = ((GameObject)selected).GetComponent<Road>();
                        t.LeftRoad.RightRoad = t;
                        break;
                    case 1:
                        t.RightRoad = ((GameObject)selected).GetComponent<Road>();
                        t.RightRoad.LeftRoad = t;
                        break;
                    default:
                        break;
                }
                stateSideRoads = -1;
            }

            GUILayout.Space(2);
            if (t.Head != null)
            {
                if (Event.current.commandName == "ObjectSelectorUpdated")
                    selected = EditorGUIUtility.GetObjectPickerObject();
                var head = t.Head;
                EditorGUILayout.PropertyField(new SerializedObject(t).FindProperty("_head"));
                //if (GUILayout.Button("Connect path as incomming"))
                //{
                //    EditorGUIUtility.ShowObjectPicker<PathRoadLayout>(null, true, "", EditorGUIUtility.GetObjectPickerControlID());

                //    stateHead = 0;
                //}
                //if (GUILayout.Button("Connect path as outgoing"))
                //{
                //    EditorGUIUtility.ShowObjectPicker<PathRoadLayout>(null, true, "", EditorGUIUtility.GetObjectPickerControlID());

                //    stateHead = 1;
                //}
                if (GUILayout.Button("Merge node to node"))
                {
                    var nodes = from o in FindObjectsOfType<GameObject>() where (o.GetComponent<PathNode>() != null && o != t.Head.gameObject) select o.GetComponent<PathNode>();
                    PathNode closest = null;
                    foreach (var node in nodes)
                    {
                        closest = closest ?? node;
                        closest = ((node.Position - t.Head.Position).magnitude < (closest.Position - t.Head.Position).magnitude) ? node : closest;
                    }
                    EditorGUIUtility.ShowObjectPicker<PathNode>(closest, true, "", EditorGUIUtility.GetObjectPickerControlID());

                    stateHead = 2;
                }

                if (GUILayout.Button("Connect to closest"))
                {
                    var nodes = from o in FindObjectsOfType<GameObject>() where (o.GetComponent<PathNode>() != null && o != t.Head.gameObject) select o.GetComponent<PathNode>();
                    PathNode closest = null;
                    foreach (var node in nodes)
                    {
                        closest = closest ?? node;
                        closest = ((node.Position - t.Head.Position).magnitude < (closest.Position - t.Head.Position).magnitude) ? node : closest;
                    }
                    if (closest != null)
                    {
                        head.MergeTo(closest);
                        Destroy(head.gameObject);
                    }
                }

                if (Event.current.commandName == "ObjectSelectorClosed")
                {
                    switch (stateHead)
                    {
                        case 0:
                            {
                                var o = ((GameObject)selected).GetComponent<PathRoadLayout>();
                                head.AddAsIncomming(o);
                            }
                            break;
                        case 1:
                            {
                                var o = ((GameObject)selected).GetComponent<PathRoadLayout>();
                                head.AddAsOutgoing(o);
                            }
                            break;
                        case 2:
                            {
                                var o = ((GameObject)selected).GetComponent<PathNode>();
                                head.MergeTo(o);
                                Destroy(head.gameObject);
                            }
                            break;
                        default:
                            break;
                    }
                    stateHead = -1;
                }

                if (GUILayout.Button("Create incomming road"))
                {
                    var pos = head.Position;
                    var obj = Instantiate(PathFactory.Instance.PathPrefab);
                    var layout = obj.GetComponent<PathRoadLayout>();
                    layout.Head = head;
                    layout.Path.bezierPath.AddSegmentToEnd(new Vector3(pos.x + 1, pos.y, pos.z));
                    layout.Path.bezierPath.AddSegmentToEnd(pos);
                    layout.Path.bezierPath.DeleteSegment(1);
                    layout.Path.bezierPath.DeleteSegment(0);
                    layout.MaxSpeed = t.MaxSpeed;
                }
                if (GUILayout.Button("Create outgoing road"))
                {
                    var pos = head.Position;
                    var obj = Instantiate(PathFactory.Instance.PathPrefab);
                    var layout = obj.GetComponent<PathRoadLayout>();
                    layout.Tail = head;
                    layout.Path.bezierPath.AddSegmentToEnd(pos);
                    layout.Path.bezierPath.AddSegmentToEnd(new Vector3(pos.x + 1, pos.y, pos.z));
                    layout.Path.bezierPath.DeleteSegment(1);
                    layout.Path.bezierPath.DeleteSegment(0);
                    layout.MaxSpeed = t.MaxSpeed;
                }
            }
            else
            {
                GUILayout.Label("No head");
            }
            GUILayout.Space(2);
            if (t.Tail != null)
            {
                if (Event.current.commandName == "ObjectSelectorUpdated")
                    selected = EditorGUIUtility.GetObjectPickerObject();
                var tail = t.Tail;
                EditorGUILayout.PropertyField(new SerializedObject(t).FindProperty("_tail"));
                //if (GUILayout.Button("Connect path as incomming"))
                //{
                //    EditorGUIUtility.ShowObjectPicker<PathRoadLayout>(null, true, "", EditorGUIUtility.GetObjectPickerControlID());

                //    stateTail = 0;
                //}
                //if (GUILayout.Button("Connect path as outgoing"))
                //{
                //    EditorGUIUtility.ShowObjectPicker<PathRoadLayout>(null, true, "", EditorGUIUtility.GetObjectPickerControlID());

                //    stateTail = 1;
                //}
                if (GUILayout.Button("Merge node to node"))
                {
                    var nodes = from o in FindObjectsOfType<GameObject>() where (o.GetComponent<PathNode>() != null && o != t.Tail.gameObject) select o.GetComponent<PathNode>();
                    PathNode closest = null; 
                    foreach (var node in nodes)
                    {
                        closest = closest ?? node;
                        closest = ((node.Position - t.Tail.Position).magnitude < (closest.Position - t.Tail.Position).magnitude) ? node : closest;
                    }
                    EditorGUIUtility.ShowObjectPicker<PathNode>(closest, true, "", EditorGUIUtility.GetObjectPickerControlID());

                    stateTail = 2;
                }
                if (GUILayout.Button("Connect to closest"))
                {
                    var nodes = from o in FindObjectsOfType<GameObject>() where (o.GetComponent<PathNode>() != null && o != t.Tail.gameObject) select o.GetComponent<PathNode>();
                    PathNode closest = null;
                    foreach (var node in nodes)
                    {
                        closest = closest ?? node;
                        closest = ((node.Position - t.Tail.Position).magnitude < (closest.Position - t.Tail.Position).magnitude) ? node : closest;
                    }
                    if (closest != null)
                    {
                        tail.MergeTo(closest);
                        Destroy(tail.gameObject);
                    }
                }

                if (Event.current.commandName == "ObjectSelectorClosed")
                {
                    switch (stateTail)
                    {
                        case 0:
                            {
                                var o = ((GameObject)selected).GetComponent<PathRoadLayout>();
                                tail.AddAsIncomming(o);
                            }
                            break;
                        case 1:
                            {
                                var o = ((GameObject)selected).GetComponent<PathRoadLayout>();
                                tail.AddAsOutgoing(o);
                            }
                            break;
                        case 2:
                            {
                                var o = ((GameObject)selected).GetComponent<PathNode>();
                                tail.MergeTo(o);
                                Destroy(tail.gameObject);
                            }
                            break;
                        default:
                            break;
                    }
                    stateTail = -1;
                }

                if (GUILayout.Button("Create incomming road"))
                {
                    var pos = tail.Position;
                    var obj = Instantiate(PathFactory.Instance.PathPrefab);
                    var layout = obj.GetComponent<PathRoadLayout>();
                    layout.Head = tail;
                    layout.Path.bezierPath.AddSegmentToEnd(new Vector3(pos.x + 1, pos.y, pos.z));
                    layout.Path.bezierPath.AddSegmentToEnd(pos);
                    layout.Path.bezierPath.DeleteSegment(1);
                    layout.Path.bezierPath.DeleteSegment(0);
                }
                if (GUILayout.Button("Create outgoing road"))
                {
                    var pos = tail.Position;
                    var obj = Instantiate(PathFactory.Instance.PathPrefab);
                    var layout = obj.GetComponent<PathRoadLayout>();
                    layout.Tail = tail;
                    layout.Path.bezierPath.AddSegmentToEnd(pos);
                    layout.Path.bezierPath.AddSegmentToEnd(new Vector3(pos.x + 1, pos.y, pos.z));
                    layout.Path.bezierPath.DeleteSegment(1);
                    layout.Path.bezierPath.DeleteSegment(0);
                }
            }
            else
            {
                GUILayout.Label("No tail");
            }
        }
    }
#endif
}
