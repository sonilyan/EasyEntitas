namespace Entitas {

    public partial class Matcher : IAllOfMatcher {

        public int[] indices {
            get {
                if (_indices == null) {
                    _indices = mergeIndices(_allOfIndices, _anyOfIndices, _noneOfIndices);
                }
                return _indices;
            }
        }

        public int[] allOfIndices { get { return _allOfIndices; } }
        public int[] anyOfIndices { get { return _anyOfIndices; } }
        public int[] noneOfIndices { get { return _noneOfIndices; } }

        public string[] componentNames { get; set; }

        int[] _indices;
        int[] _allOfIndices;
        int[] _anyOfIndices;
        int[] _noneOfIndices;

        Matcher() {
        }

        IAnyOfMatcher IAllOfMatcher.AnyOf(params IMatcher[] matchers) {
	        var indices = mergeIndices(matchers);
			_anyOfIndices = distinctIndices(indices);
			_indices = null;
			_isHashCached = false;
	        return this;
        }

        public INoneOfMatcher NoneOf(params IMatcher[] matchers) {
	        var indices = mergeIndices(matchers);
			_noneOfIndices = distinctIndices(indices);
			_indices = null;
			_isHashCached = false;
            return this;
        }

        public bool Matches(Entity entity) {
            return (_allOfIndices == null || entity.HasComponents(_allOfIndices))
                && (_anyOfIndices == null || entity.HasAnyComponent(_anyOfIndices))
                && (_noneOfIndices == null || !entity.HasAnyComponent(_noneOfIndices));
        }
    }
}
