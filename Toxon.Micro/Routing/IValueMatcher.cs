namespace Toxon.Micro.Routing
{
    public interface IValueMatcher
    {
        bool Matches(object value);
    }
}