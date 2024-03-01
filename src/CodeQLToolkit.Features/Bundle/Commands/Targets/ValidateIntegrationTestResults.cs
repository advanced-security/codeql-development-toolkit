using CodeQLToolkit.Features.CodeQL.Commands.Targets;
using CodeQLToolkit.Features.Validation;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Bundle.Commands.Targets
{
    public class ValidateIntegrationTestResults : CommandTarget
    {
        public string Expected {  get; set; }
        public string Actual { get; set; }

        public override void Run()
        {
            Log<ValidateIntegrationTestResults>.G().LogInformation($"Running validate-integration-tests command");

            // First load up the expected and the actuals.
            if (!File.Exists(Expected))
            {
                DieWithError($"Expected file {Expected} does not exist.");
            }

            if(!File.Exists(Actual))
            {
                DieWithError($"Actual file {Actual} does not exist.");
            }

            try
            {
                var expectedSARIF = JsonConvert.DeserializeObject<SARIFResult>(File.ReadAllText(Expected));
                var actualSARIF = JsonConvert.DeserializeObject<SARIFResult>(File.ReadAllText(Actual));


                // Check 1: Ensure they contain the same number of results. This is a pretty signifigant error so we don't report 
                // past this point. 
                if (expectedSARIF.runs.Length != actualSARIF.runs.Length)
                {
                    DieWithError($"Number of runs does not match. Expected: {expectedSARIF.runs.Length}, Actual: {actualSARIF.runs.Length}");
                }

                // Check 2: For each run, check if the number of results match.
                for (var i = 0; i < expectedSARIF.runs.Length; i++)
                {
                    if (expectedSARIF.runs[i].results.Length != actualSARIF.runs[i].results.Length)
                    {
                        Log<ValidateIntegrationTestResults>.G().LogWarning($"Number of results does not match. Expected: {expectedSARIF.runs[i].results.Length}, Actual: {actualSARIF.runs[i].results.Length}");
                    }
                }

                // Check 3: Build up some mappings to compare exactly what is missing. 
                var actualDict = ExtractResultMap(actualSARIF);
                var expectedDict = ExtractResultMap(expectedSARIF);

               
                // Populate the differences.
                var resultsInExpectedNotInActual = FindMissingResults(expectedSARIF, actualDict);
                var resultsInActualNotInExpected = FindMissingResults(actualSARIF, expectedDict);

                // Report results.
                if(resultsInExpectedNotInActual.Count == 0 && resultsInActualNotInExpected.Count == 0)
                {
                    Log<ValidateIntegrationTestResults>.G().LogInformation($"SARIF results identical.");
                }
                else
                {
                    Log<ValidateIntegrationTestResults>.G().LogInformation($"SARIF results not identical. Please see below for full report.");

                    Log<ValidateIntegrationTestResults>.G().LogInformation($"SARIF Results in EXPECTED not contained in ACTUAL");
                    Log<ValidateIntegrationTestResults>.G().LogInformation($"------------------------------------------------------");

                    foreach (var r in resultsInExpectedNotInActual)
                    {

                        Log<ValidateIntegrationTestResults>.G().LogInformation($"Rule: {r.ruleId} @ Location {r.LocationsString()} with Message \"{r.message.text}\" exists in EXPECTED but is missing from ACTUAL.");

                    }

                    Log<ValidateIntegrationTestResults>.G().LogInformation($"SARIF Results in ACTUAL not contained in EXPECTED");
                    Log<ValidateIntegrationTestResults>.G().LogInformation($"------------------------------------------------------");

                    foreach (var r in resultsInActualNotInExpected)
                    {
                        Log<ValidateIntegrationTestResults>.G().LogInformation($"Rule: {r.ruleId} @ Location {r.LocationsString()} with Message \"{r.message.text}\" exists in ACTUAL but is missing from EXPECTED.");
                    }

                    DieWithError("SARIF results NOT identical. Results reported, above.");
                }

            }
            catch (Exception e)
            {
                Log<ValidateIntegrationTestResults>.G().LogError(e.Message);
                DieWithError("Error decoding SARIF files. Please check the format and try again.");
            }

            List<Result> FindMissingResults(SARIFResult? expectedSARIF, Dictionary<string, List<Result>> actualDict)
            {
                var resultsNotFound = new List<Result>();

                for (var i = 0; i < expectedSARIF.runs.Length; i++)
                {
                    for (var j = 0; j < expectedSARIF.runs[i].results.Length; j++)
                    {
                        var result = expectedSARIF.runs[i].results[j];

                        if (!actualDict.ContainsKey(result.ruleId))
                        {
                            resultsNotFound.Add(result);
                        }
                        else
                        {
                            if (!ContainsResult(result, actualDict[result.ruleId]))
                            {
                                resultsNotFound.Add(result);
                            }
                        }
                    }

                }

                return resultsNotFound;
            }
        }

        private bool ContainsResult(Result searchFor, List<Result> inResults)
        {
            foreach(var result in inResults)
            {
                if(ResultMatches(result, searchFor))
                {
                    return true;
                }
            }
            
            return false;
        }

        private bool ResultMatches(Result a, Result b)
        {
            if(a.ruleId == b.ruleId && a.message.text == b.message.text && a.LocationsString() == b.LocationsString())
            {
                return true;
            }

            return false;
        }

        private Dictionary<string, List<Result>> ExtractResultMap(SARIFResult? sarif)
        {
            Dictionary<string, List<Result>> dict = new Dictionary<string, List<Result>>();

            for (var i = 0; i < sarif.runs.Length; i++)
            {
                for (var j = 0; j < sarif.runs[i].results.Length; j++)
                {
                    var result = sarif.runs[i].results[j];

                    if (dict.ContainsKey(result.ruleId))
                    {
                        dict[result.ruleId].Add(result);
                    }
                    else
                    {
                        dict[result.ruleId] = new List<Result>()
                            {
                                result
                            };
                    }
                }
            }
            return dict;
        }
    }
}
