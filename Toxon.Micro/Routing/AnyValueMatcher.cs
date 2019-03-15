namespace Toxon.Micro.Routing
{
    public class AnyValueMatcher : IValueMatcher
    {
        public bool Matches(object value) => true;
    }
}