using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Types
{
    public class SARIFResult
    {
        public Run[] runs { get; set; }
        public string schema { get; set; }
        public string version { get; set; }

        public string? RawSARIF { get; set; }
    }

    public class Run
    {
        public Artifact[] artifacts { get; set; }
        public Automationdetails automationDetails { get; set; }
        public Conversion conversion { get; set; }
        public Result[] results { get; set; }
        public Tool1 tool { get; set; }
        public Versioncontrolprovenance[] versionControlProvenance { get; set; }
    }

    public class Automationdetails
    {
        public string id { get; set; }
    }

    public class Conversion
    {
        public Tool tool { get; set; }
    }

    public class Tool
    {
        public Driver driver { get; set; }
    }

    public class Driver
    {
        public string name { get; set; }
    }

    public class Tool1
    {
        public Driver1 driver { get; set; }
        public Extension[] extensions { get; set; }
    }

    public class Driver1
    {
        public string name { get; set; }
        public string semanticVersion { get; set; }
    }

    public class Extension
    {
        public string name { get; set; }
        public string semanticVersion { get; set; }
        public Rule[] rules { get; set; }
    }

    public class Rule
    {
        public Defaultconfiguration defaultConfiguration { get; set; }
        public Fulldescription fullDescription { get; set; }
        public Help help { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public Properties properties { get; set; }
        public Shortdescription shortDescription { get; set; }
    }

    public class Defaultconfiguration
    {
        public string level { get; set; }
    }

    public class Fulldescription
    {
        public string text { get; set; }
    }

    public class Help
    {
        public string markdown { get; set; }
        public string text { get; set; }
    }

    public class Properties
    {
        public string precision { get; set; }
        public string queryURI { get; set; }
        public string securityseverity { get; set; }
        public string[] tags { get; set; }
    }

    public class Shortdescription
    {
        public string text { get; set; }
    }

    public class Artifact
    {
        public Location location { get; set; }
    }

    public class Location
    {
        public int index { get; set; }
        public string uri { get; set; }
    }

    public class Result
    {
        public string correlationGuid { get; set; }
        public string level { get; set; }

        public Location1[] locations { get; set; }

        public string LocationsString()
        {

            List<string> _locations = new List<string>();

            foreach (var location in locations)
            {
                _locations.Add(location.ToString());
            }

            return string.Join(", ", _locations);

        }
        public Message message { get; set; }
        public Partialfingerprints partialFingerprints { get; set; }
        public Properties1 properties { get; set; }
        public Rule1 rule { get; set; }
        public string ruleId { get; set; }
    }

    public class Message
    {
        public string text { get; set; }
    }

    public class Partialfingerprints
    {
        public string primaryLocationLineHash { get; set; }
    }

    public class Properties1
    {
        public int githubalertNumber { get; set; }
        public string githubalertUrl { get; set; }
    }

    public class Rule1
    {
        public string id { get; set; }
        public Toolcomponent toolComponent { get; set; }
        public int index { get; set; }
    }

    public class Toolcomponent
    {
        public int index { get; set; }
    }

    public class Location1
    {
        public Physicallocation physicalLocation { get; set; }

        public override string ToString()
        {
            return $"{physicalLocation.artifactLocation.uri}:{physicalLocation.region.startLine}:{physicalLocation.region.startColumn}-{physicalLocation.region.endLine}:{physicalLocation.region.endColumn}";
        }
    }

    public class Physicallocation
    {
        public Artifactlocation artifactLocation { get; set; }
        public Region region { get; set; }
    }

    public class Artifactlocation
    {
        public int index { get; set; }
        public string uri { get; set; }
    }

    public class Region
    {
        public int endColumn { get; set; }
        public int endLine { get; set; }
        public int startColumn { get; set; }
        public int startLine { get; set; }
    }

    public class Versioncontrolprovenance
    {
        public string branch { get; set; }
        public string repositoryUri { get; set; }
        public string revisionId { get; set; }
    }
}
