namespace Entitas {

    public interface IAllOfMatcher : IAnyOfMatcher {

        IAnyOfMatcher AnyOf(params IMatcher[] matchers);
    }
}
