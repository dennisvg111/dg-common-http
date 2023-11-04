namespace DG.Common.Http.Testing
{
    public class SimpleMockHeader
    {
        private readonly string _name;
        private readonly string _value;

        public string Name => _name;
        public string Value => _value;

        public SimpleMockHeader(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public override string ToString()
        {
            return $"{_name}: {_value}";
        }
    }
}
