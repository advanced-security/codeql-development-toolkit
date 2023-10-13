namespace CodeQLToolkit.Features.Pack.Commands.Validate.Schemas {
    public interface IYmlSchema {
        public string Name { get; set; }
        public string Version { get; set; }
    }
}