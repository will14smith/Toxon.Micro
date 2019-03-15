namespace Toxon.Micro.Routing
{
    public interface IRequestMatcher
    {
        MatchResult Matches(IRequest request);
    }
}