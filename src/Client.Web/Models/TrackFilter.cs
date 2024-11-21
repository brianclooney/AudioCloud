
namespace AudioCloud.Client.Web.Models
{
    public class TrackFilter
    {
        public string Value { get; set; } = string.Empty;
        public TrackFilterType Type { get; set; } = TrackFilterType.None;

        public override bool Equals(Object? o)
        {
            if (o == null || o is not TrackFilter)
            {
                return false;
            }
            else {
                var f = (TrackFilter) o;
                return Value == f.Value && Type == f.Type;
            }
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() + Type.GetHashCode();
        }
    }
}
