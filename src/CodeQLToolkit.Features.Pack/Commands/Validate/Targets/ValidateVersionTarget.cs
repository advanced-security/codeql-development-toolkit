using CodeQLToolkit.Shared.Logging;
using CodeQLToolkit.Shared.Utils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Semver;
using CodeQLToolkit.Features.Pack.Commands.Validate.Schemas;
using System.Diagnostics;

namespace CodeQLToolkit.Features.Pack.Commands.Validate.Targets
{
    public class YamlParseException : Exception
    {
        public string message { get; set; }
        public YamlParseException(string failMessage)
        {
            message = failMessage;
        }
    }
    public class ValidateVersionTarget : CommandTarget
    {
        public string[] QlpackYmlFiles { get; set; }
        private static Deserializer YamlDeserializer = (Deserializer)new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

        public override void Run()
        {
            Console.WriteLine("Hello world! I got: ");
            foreach (string qlpackYmlFile in QlpackYmlFiles)
        private static UnbrokenSemVersionRange FindUnbrokenSemverRangeOverlap(
            UnbrokenSemVersionRange range1,
            UnbrokenSemVersionRange range2
        )
        { 
            if (range1.End.ComparePrecedenceTo(range2.Start) == -1 || range2.End.ComparePrecedenceTo(range1.Start) == -1) {
                return UnbrokenSemVersionRange.Empty;
            } else {
                var start = range1.Start.ComparePrecedenceTo(range2.Start) == -1 ? range2.Start : range1.Start;
                var end = range1.End.ComparePrecedenceTo(range2.End) == -1 ? range1.End : range2.End;
                return UnbrokenSemVersionRange.Inclusive(start, end);
            }
        }

        private static SemVersionRange FindSemverRangeOverlap(
            SemVersionRange range1,
            SemVersionRange range2
        )
        {
            var acc = new List<UnbrokenSemVersionRange>();
            foreach (UnbrokenSemVersionRange unbrokenRange1 in range1)
                foreach (UnbrokenSemVersionRange unbrokenRange2 in range2)
                    acc.Add(FindUnbrokenSemverRangeOverlap(unbrokenRange1, unbrokenRange2));

            return SemVersionRange.Create(acc);
        }

        private static bool thereIsSemVersionRangeOverlap(
            SemVersionRange range1,
            SemVersionRange range2
        )
        {
            var intersection = FindSemverRangeOverlap(range1, range2);
            bool foundNonemptyUnbrokenSemVersionRange = false;

            foreach (UnbrokenSemVersionRange unbrokenRange in intersection)
            {
                Console.WriteLine($"Inspecting [{unbrokenRange.Start}, {unbrokenRange.End})...");
                if (!unbrokenRange.Equals(UnbrokenSemVersionRange.Empty))
                    foundNonemptyUnbrokenSemVersionRange = true;
                break;
            }

            return foundNonemptyUnbrokenSemVersionRange;
        }

        private static void testRangeOverlap()
        {
            var semver21 = SemVersionRange.Parse("1.2.1");
            var semver22 = SemVersionRange.Parse("^1.2.0");

            Debug.Assert(thereIsSemVersionRangeOverlap(semver21, semver22));

            var semver31 = SemVersionRange.Parse("^1.2.0");
            var semver32 = SemVersionRange.Parse("1.2.1");

            Debug.Assert(thereIsSemVersionRangeOverlap(semver31, semver32));

            var semver41 = UnbrokenSemVersionRange.AtLeast(SemVersion.Parse("1.2.0"));
            var semver42 = UnbrokenSemVersionRange.Equals(SemVersion.Parse("1.2.1"));

            Console.WriteLine($"semver41: {semver41.Start}, {semver41.End}");
            Console.WriteLine($"semver42: {semver42.Start}, {semver42.End}");

            var intersection = FindUnbrokenSemverRangeOverlap(semver41, semver42);            
            Console.WriteLine($"intersection: {intersection.Start}, {intersection.End}");
        }
    }
}
