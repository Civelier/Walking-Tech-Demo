using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityTests
{
    public enum TestResultStatus
    {
        Passed,
        Failed,
        Crashed
    }
    public struct UnityTestResult
    {
        public readonly Exception CrashException;
        public readonly TestResultStatus ResultStatus;
        public readonly string TestName;

        public UnityTestResult(TestResultStatus status, string testName)
        {
            ResultStatus = status;
            CrashException = null;
            TestName = testName;
        }

        public UnityTestResult(Exception exception, string testName)
        {
            CrashException = exception;
            ResultStatus = TestResultStatus.Crashed;
            TestName = testName;
        }

        public void DebugResult()
        {
            switch (ResultStatus)
            {
                case TestResultStatus.Passed:
                    Debug.Log($"{TestName} - Success");
                    break;
                case TestResultStatus.Failed:
                    Debug.LogError($"{TestName} - Failed");
                    break;
                case TestResultStatus.Crashed:
                    Debug.LogError($"{TestName} - Failed");
                    Debug.LogException(CrashException);
                    break;
                default:
                    break;
            }
        }
    }
}
