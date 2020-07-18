namespace Entitas {

    public interface IAnyOfMatcher : INoneOfMatcher {

        INoneOfMatcher NoneOf(params IMatcher[] matchers);
    }
}
