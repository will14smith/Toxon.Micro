namespace Toxon.Micro.Routing
{
    public interface IValueMatcher
    {
        MatchResult Matches(object value);
    }
}