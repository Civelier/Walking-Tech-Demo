using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityTests
{
    public delegate IEnumerator<bool?> Test();

    public class UnityTest : MonoBehaviour
    {
        private Dictionary<string, GameObject> _instanciatedObjects = new Dictionary<string, GameObject>();

        private Dictionary<string, Test> _tests = new Dictionary<string, Test>();
        public IReadOnlyDictionary<string, Test> Tests => _tests;

        public void RunTests()
        {
            StartCoroutine(RunCoroutine());
        }

        IEnumerator<object> RunCoroutine()
        {
            foreach (var pair in _tests)
            {
                using (var en = pair.Value())
                {
                    bool passed = true;
                    bool exit = false;
                    bool first = true;
                    while (!exit && en.MoveNext())
                    {
                        if (!first) yield return null;
                        else first = false;
                        try
                        {
                            if (en.Current != null)
                            {
                                if (!en.Current.Value) passed = false;
                            }
                        }
                        catch (System.Exception e)
                        {
                            new UnityTestResult(e, name).DebugResult();
                            exit = true;
                            break;
                        }
                    }
                    if (!exit) new UnityTestResult(passed ? TestResultStatus.Passed : TestResultStatus.Failed, pair.Key).DebugResult();
                }
                ReleaseMemory();
            }
        }

        protected void AddTest(Test test, string name)
        {
            _tests.Add(name, test);
        }

        public void ReleaseMemory()
        {
            foreach (var gameObject in _instanciatedObjects)
            {
                Debug.LogWarning($"Destroying: {gameObject.Key}");
                DestroyImmediate(gameObject.Value);
            }
            _instanciatedObjects.Clear();
        }

        public GameObject SafeInstantiate(GameObject obj, string name)
        {
            var g = Instantiate(obj);
            g.name = name;
            _instanciatedObjects.Add(name, g);
            return g;
        }

        public T SafeInstantiate<T>(GameObject obj, string name) where T : Object
        {
            return SafeInstantiate(obj, name).GetComponent<T>();
        }
    }

    public static class UnityAssertions
    {
        public static void ShouldBeEqual<T>(this T value, T expectedResult)
        {
            if (!value.Equals(expectedResult)) throw new System.Exception($"Expected value to be equal to {expectedResult}, but found {value}.");
        }

        public static void ShouldNotBeEqual<T>(this T value, T expectedResult)
        {
            if (value.Equals(expectedResult)) throw new System.Exception($"Expected value not to be equal to {expectedResult}, but found {value}.");
        }

        public static void ShouldBeApproximately(this float value, float expectedResult)
        {
            if (!Mathf.Approximately(value, expectedResult)) throw new System.Exception($"Expected value to be approximately {expectedResult}, but found {value}.");
        }

        public static void ShouldBeApproximately(this Vector3 value, Vector3 expectedResult)
        {
            if (!Mathf.Approximately(value.x, expectedResult.x) || !Mathf.Approximately(value.y, expectedResult.y) || !Mathf.Approximately(value.z, expectedResult.z)) throw new System.Exception($"Expected value to be approximately {expectedResult}, but found {value}.");
        }

        public static void ShouldNotBeApproximately(this float value, float expectedResult)
        {
            if (Mathf.Approximately(value, expectedResult)) throw new System.Exception($"Expected value not to be approximately {expectedResult}, but found {value}.");
        }

        public static void ShouldNotBeApproximately(this Vector3 value, Vector3 expectedResult)
        {
            if (Mathf.Approximately(value.x, expectedResult.x) && Mathf.Approximately(value.y, expectedResult.y) && Mathf.Approximately(value.z, expectedResult.z)) throw new System.Exception($"Expected value to be approximately {expectedResult}, but found {value}.");
        }

        public static void ShouldBeGreaterThan(this int value, int other)
        {
            if (value <= other) throw new System.Exception($"Expected value to be greater than {other}, but found {value}.");
        }
        public static void ShouldBeGreaterThan(this float value, float other)
        {
            if (value <= other) throw new System.Exception($"Expected value to be greater than {other}, but found {value}.");
        }

        public static void ShouldBeTrue(this bool value)
        {
            if (!value) throw new System.Exception($"Expected value to be true, but found {value}.");
        }

        public static void ShouldBeFalse(this bool value)
        {
            if (value) throw new System.Exception($"Expected value to be false, but found {value}.");
        }
    }
}