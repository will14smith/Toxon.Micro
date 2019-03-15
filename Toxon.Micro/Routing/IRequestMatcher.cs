namespace Toxon.Micro.Routing
{
    public interface IRequestMatcher
    {
        bool Matches(IRequest request);
    }
}