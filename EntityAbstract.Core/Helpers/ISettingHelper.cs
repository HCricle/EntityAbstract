namespace EntityAbstract.Core.Helpers
{
    public interface ISettingHelper
    {
        object this[string key] { get; }

        bool Exist(string key);
        T GetValue<T>(string key);
        void SetValue(string key, object value);
        object[] GetAllValue();
    }
}