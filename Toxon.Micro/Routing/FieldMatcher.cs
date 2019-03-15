using System;
using System.Linq;
using System.Reflection;

namespace Toxon.Micro.Routing
{
    public class FieldMatcher : IRequestMatcher
    {
        public string FieldName { get; }
        public IValueMatcher FieldValue { get; }

        public FieldMatcher(string fieldName, IValueMatcher fieldValue)
        {
            FieldName = fieldName;
            FieldValue = fieldValue;
        }

        public MatchResult Matches(IRequest request)
        {
            if (request is null) return MatchResult.NoMatch;

            object requestFieldValue;

            var type = request.GetType();
            var property = GetProperty(type);
            if (property != null)
            {
                requestFieldValue = property.GetValue(request);
            }
            else
            {
                var field = GetField(type);
                if (field != null)
                {
                    requestFieldValue = field.GetValue(request);
                }
                else
                {
                    return MatchResult.NoMatch;
                }
            }

            var fieldMatch = FieldValue.Matches(requestFieldValue);
            if (!fieldMatch.Matched)
            {
                return MatchResult.NoMatch;
            }

            return new FieldMatchResult(FieldName, fieldMatch);
        }

        private FieldInfo GetField(Type type)
        {
            return type.GetFields().FirstOrDefault(x => FieldNameMatches(x.Name));
        }
        private PropertyInfo GetProperty(Type type)
        {
            return type.GetProperties().FirstOrDefault(x => FieldNameMatches(x.Name));
        }

        private bool FieldNameMatches(string fieldName)
        {
            return FieldName.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase);
        }

        private class FieldMatchResult : MatchResult
        {
            private readonly string _fieldName;
            private readonly MatchResult _fieldMatch;

            public FieldMatchResult(string fieldName, MatchResult fieldMatch)
            {
                _fieldName = fieldName;
                _fieldMatch = fieldMatch;
            }

            public override bool IsBetterMatchThan(MatchResult other)
            {
                if (other is FieldMatchResult otherField)
                {
                    var fieldNameComp = _fieldName.CompareTo(otherField._fieldName);

                    if (fieldNameComp < 0) return true;
                    if (fieldNameComp == 0) return _fieldMatch.IsBetterMatchThan(otherField._fieldMatch);
                    return false;
                }

                return !other.IsBetterMatchThan(this);
            }
        }
    }
}