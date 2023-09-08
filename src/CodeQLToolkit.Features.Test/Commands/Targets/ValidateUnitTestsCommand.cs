using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Test.Commands.Targets
{

    
    public class UnitTestResult
    {
        public string test { get; set; }
        public bool pass { get; set; }
        public string failureStage { get; set; }
        public string failureDescription { get; set; }
        public object[] messages { get; set; }
        public int compilationMs { get; set; }
        public int evaluationMs { get; set; }
        public string expected { get; set; }
        public string actual { get; set; }
        public string[] diff { get; set; }
    }

    public class ValidateUnitTestsCommand : CommandTarget
    {
        public string ResultsDirectory { get; set; }

        public bool PrettyPrint { get; set; }

        public override void Run()
        {
            Log<ValidateUnitTestsCommand>.G().LogInformation($"Validating unit tests in {ResultsDirectory}");

            string[] results = Directory.GetFiles(ResultsDirectory, "test_report_*", SearchOption.AllDirectories);

            List<UnitTestResult> unitTestResults = new List<UnitTestResult>();

            foreach (string result in results)
            {
                using (StreamReader r = new StreamReader(result))
                {
                    string json = r.ReadToEnd();
                    List<UnitTestResult> items = JsonConvert.DeserializeObject<List<UnitTestResult>>(json);

                    foreach(var item in items)
                    {                        
                        unitTestResults.Add(item);                        
                    }
                }
            }


            int totalCases = unitTestResults.Count;
            int currentCase = 0;

            foreach (var item in unitTestResults)
            {
                currentCase++;

                if (item.pass)
                {
                    if(PrettyPrint)
                    {
                        Console.WriteLine($" ✅ [PASS] ({currentCase} of {totalCases}) {item.test}");
                    }
                    else
                    {
                        Log<ValidateUnitTestsCommand>.G().LogError($"+----------------------------------------------+");
                        Log<ValidateUnitTestsCommand>.G().LogError($"| TEST CASE ({currentCase} of {totalCases})    : {Path.GetFileName(item.test)}");
                        Log<ValidateUnitTestsCommand>.G().LogError($"+----------------------------------------------+");
                        Log<ValidateUnitTestsCommand>.G().LogError($"| STATUS                : PASSED ");
                        Log<ValidateUnitTestsCommand>.G().LogError($"+----------------------------------------------+");
                    }
                }
                else
                {
                    if (PrettyPrint)
                    {
                        Console.WriteLine($"❌ [FAIL] ({currentCase} of {totalCases}) {item.test}");
                    }
                    else
                    {
                        if (item.failureStage == "RESULT")
                        {
                            Log<ValidateUnitTestsCommand>.G().LogError($"+----------------------------------------------+");
                            Log<ValidateUnitTestsCommand>.G().LogError($"| TEST CASE ({currentCase} of {totalCases})    : {Path.GetFileName(item.test)}");
                            Log<ValidateUnitTestsCommand>.G().LogError($"+----------------------------------------------+");
                            Log<ValidateUnitTestsCommand>.G().LogError($"| STATUS                : FAILED ");
                            Log<ValidateUnitTestsCommand>.G().LogError($"| FAILURE TYPE          : RESULT");
                            Log<ValidateUnitTestsCommand>.G().LogError($"+----------------------------------------------+");
                            Log<ValidateUnitTestsCommand>.G().LogError($"| TEST DIFFERENCES                             |");
                            Log<ValidateUnitTestsCommand>.G().LogError($"+----------------------------------------------+");
                            Log<ValidateUnitTestsCommand>.G().LogError($"| FULL PATH             : {item.test}");


                            foreach (var diff in item.diff)
                            {
                                Log<ValidateUnitTestsCommand>.G().LogError($"| {diff}");
                            }
                            Log<ValidateUnitTestsCommand>.G().LogError($"+----------------------------------------------+");

                            Log<ValidateUnitTestsCommand>.G().LogError($"\n\n\n\n");

                        }
                        else
                        {
                            Log<ValidateUnitTestsCommand>.G().LogError($"+----------------------------------------------+");
                            Log<ValidateUnitTestsCommand>.G().LogError($"| TEST CASE ({currentCase} of {totalCases})    : {Path.GetFileName(item.test)}");
                            Log<ValidateUnitTestsCommand>.G().LogError($"+----------------------------------------------+");
                            Log<ValidateUnitTestsCommand>.G().LogError($"| STATUS                : FAILED ");
                            Log<ValidateUnitTestsCommand>.G().LogError($"| FAILURE TYPE          : {item.failureStage}");
                            Log<ValidateUnitTestsCommand>.G().LogError($"+----------------------------------------------+");
                            Log<ValidateUnitTestsCommand>.G().LogError($"| FAILURE DESCRIPTION                          |");
                            Log<ValidateUnitTestsCommand>.G().LogError($"+----------------------------------------------+");
                            Log<ValidateUnitTestsCommand>.G().LogError($"| FULL PATH             : {item.test}");
                            Log<ValidateUnitTestsCommand>.G().LogError($"| {item.failureDescription}");


                            Log<ValidateUnitTestsCommand>.G().LogError($"+----------------------------------------------+");

                            Log<ValidateUnitTestsCommand>.G().LogError($"\n\n\n\n");

                        }
                    }
                }
            }

            var failedTests = unitTestResults.Select(x=>x).Where(x=>x.pass==false).ToArray();

            if (failedTests.Length > 0 && !PrettyPrint)
            {
                DieWithError("One or more failures during run unit tests.");
            }
        }        
    }
}
