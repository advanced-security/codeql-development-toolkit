using CodeQLToolkit.Features.Test.Lifecycle.Models;
using CodeQLToolkit.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Test.Commands.Targets.Actions
{

    [AutomationType(AutomationType.ACTIONS)]
    public class ExecuteUnitTestsCommandTarget : BaseExecuteUnitTestsCommandTarget
    {
        
        public override void Run()
        {
            Log<ExecuteUnitTestsCommandTarget>.G().LogInformation($"Preparing to execute unit tests found in {Base} for Language {Language}...");

            // get a directory to work in 
            var tmpDirectory = WorkDirectory;

            var languageRoot = Path.Combine(Base, Language);            

            // check if the language root exists
            if (!Directory.Exists(languageRoot)){
                DieWithError($"Language root {languageRoot} does not exist so unit tests cannnot be run.");
            }

            // Identify the test directories. 
            string[] dirs = Directory.GetDirectories(languageRoot, "test", SearchOption.AllDirectories);

            Log<ExecuteUnitTestsCommandTarget>.G().LogInformation($"Test Directory Inventory {Language}");
            Log<ExecuteUnitTestsCommandTarget>.G().LogInformation($"-----------------------------------");

            foreach ( string dir in dirs)
            {
                Log<ExecuteUnitTestsCommandTarget>.G().LogInformation($"Found test directory: {dir}");
            }

            var transformedDirs = dirs.Select(dir => Path.GetRelativePath(Base, dir));
          
            Parallel.For(0, NumThreads,
                 slice => {

                     TestReport report = new TestReport()
                     {
                         RunnerOS = RunnerOS,
                         CLIVersion = CLIVersion,
                         STDLibIdent = STDLibIdent,
                         Language = Language,
                         Slice = slice,
                         NumSlices = NumThreads
                     };

                     var workingDirectory = Path.GetFullPath(Base);
                     var testPathString = string.Join(" ", transformedDirs);
                     var outFileReport = Path.Combine(tmpDirectory, report.FileName);

                     Log<ExecuteUnitTestsCommandTarget>.G().LogInformation($"Executing tests in working directory {workingDirectory}.");
                     Log<ExecuteUnitTestsCommandTarget>.G().LogInformation($"Test Paths: {testPathString}");
                     Log<ExecuteUnitTestsCommandTarget>.G().LogInformation($"Slice: {slice} of {NumThreads}");
                     Log<ExecuteUnitTestsCommandTarget>.G().LogInformation($"Report File: {outFileReport}...");

                     using (Process process = new Process())
                     {
                         process.StartInfo.FileName = "codeql";
                         process.StartInfo.WorkingDirectory = workingDirectory;
                         process.StartInfo.UseShellExecute = false;
                         process.StartInfo.RedirectStandardOutput = true;
                         process.StartInfo.RedirectStandardError = false;
                         process.StartInfo.Arguments = $"test run --failing-exitcode=122 --slice={slice+1}/{NumThreads} --ram=2048 --format=json --search-path={Language} {testPathString}";
                         
                         process.Start();

                         // needed for STDOUT redirection
                         var output = process.StandardOutput.ReadToEnd();

                         File.WriteAllText(outFileReport, output);

                         process.WaitForExit();

                         if (process.ExitCode != 0)
                         {
                             // This fine
                             if(process.ExitCode == 122)
                             {
                                 Log<ExecuteUnitTestsCommandTarget>.G().LogError($"One more more unit tests failed. Please see the output of the validation step for more information about failed tests cases.");
                             }
                             // this is not fine
                             else
                             {
                                 DieWithError($"Non-test related error while running unit tests. Please check debug output for more infomation.");
                             }                             
                         }
                     }
                 }
            );



        }
    }
}
