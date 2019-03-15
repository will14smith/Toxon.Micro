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

        public bool Matches(IRequest request)
        {
            if (request is null) return false;

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
                    return false;
                }
            }

            return FieldValue.Matches(requestFieldValue);
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
    }
}