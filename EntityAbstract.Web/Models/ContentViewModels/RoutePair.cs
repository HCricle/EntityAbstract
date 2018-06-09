namespace EntityAbstract.Web.Models.ContentViewModels
{
    public class RoutePair
    {
        public RoutePair(object key, object value)
        {
            Key = key.ToString();
            Value = value.ToString();
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
