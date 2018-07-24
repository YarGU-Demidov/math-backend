using MathSite.Api.Common.Entities;

namespace MathSite.Api.Entities
{
    public class SiteSetting : Entity
    {
        public SiteSetting()
        {
        }

        public SiteSetting(string key, byte[] value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public byte[] Value { get; set; }
    }
}