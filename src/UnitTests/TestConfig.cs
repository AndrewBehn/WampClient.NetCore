using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using WampClient.Core;
using Xunit;

namespace UnitTests
{
    public class TestConfig<T>
    {
        private readonly Dictionary<Type, object> _dependencies;

        public TestConfig()
        {
            var constructors = typeof(T).GetConstructors();
            if (constructors.Length != 1)
                throw new InvalidOperationException();

            var con = constructors.First();
            var paramInfos = con.GetParameters();

            _dependencies = paramInfos.ToDictionary(pi => pi.ParameterType, pi => DynamicMock(pi.ParameterType));

            var parameters = _dependencies.Select(
                keyValuePair =>
                {
                    var paramType = keyValuePair.Key;
                    var moq = keyValuePair.Value;
                    return moq.GetType().GetProperties().Single(f => f.Name == "Object" && f.PropertyType == paramType).GetValue(moq, new object[] { });
                });
            Subject = (T)con.Invoke(parameters.ToArray());
        }

        public T Subject { get; }

        public Mock<TDependency> GetDependency<TDependency>() where TDependency : class
        {
            return (Mock<TDependency>)_dependencies[typeof(TDependency)];
        }

        private static object DynamicMock(Type type)
        {
            var mock = typeof(Mock<>).MakeGenericType(type).GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            return mock;
        }

    }
}
