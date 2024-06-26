﻿using CodeQLToolkit.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Pack.Commands.Targets
{
    public class HelloJeongsooCommandTarget : CommandTarget
    {

        public int Times { get; set; }


        public override void Run()
        {
            for (int i = 0; i < Times; i++)
            {
                Console.WriteLine($"Hello! My Base Target is: {Base}");
            }


            var c = new QLTConfig()
            {
                Base = Base
            };

            if (!File.Exists(c.QLTConfigFilePath))
            {
                ProcessUtils.DieWithError($"Cannot read values from missing file {c.QLTConfigFilePath}");
            }

            var config = c.FromFile();


            Console.WriteLine($"---------current settings---------");
            Console.WriteLine($"CodeQL CLI Version: {config.CodeQLCLI}");
            Console.WriteLine($"CodeQL Standard Library Version: {config.CodeQLStandardLibrary}");
            Console.WriteLine($"CodeQL CLI Bundle Version: {config.CodeQLCLIBundle}");
            Console.WriteLine($"----------------------------------");
            Console.WriteLine("(hint: use `qlt codeql set` to modify these values.)");

        }
    }
}
