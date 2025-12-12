using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MeshKernelNET.Api;
using NUnit.Framework;
using ProtoBuf;

namespace MeshKernelNETTest.Api
{
    [TestFixture]
    public class MeshKernelSerializationTest
    {
        [TestCaseSource(nameof(GetProtoContractTypes))]
        public void TypeWithProtoContract_HasDefaultConstructor(Type type)
        {
            ConstructorInfo defaultConstructor = type.GetConstructor(Type.EmptyTypes);

            Assert.That(defaultConstructor, Is.Not.Null,
                        $"Type '{type.Name}' with proto contract must have a public parameterless constructor for protobuf serialization.");
        }

        [TestCaseSource(nameof(GetProtoContractTypes))]
        public void TypeWithProtoContract_CanBeInstantiated(Type type)
        {
            object instance = Activator.CreateInstance(type);

            Assert.That(instance, Is.Not.Null,
                        $"Should be able to create instance of '{type.Name}' for protobuf serialization.");
        }

        [TestCaseSource(nameof(GetProtoContractTypes))]
        public void TypeWithProtoContract_CanBeSerializedAndDeserialized(Type type)
        {
            using (var stream = new MemoryStream())
            {
                object instance = Activator.CreateInstance(type);

                Serializer.Serialize(stream, instance);
                stream.Position = 0;
                object deserialized = Serializer.Deserialize(type, stream);

                Assert.That(deserialized, Is.Not.Null, $"Should be able to deserialize '{type.Name}'.");
                Assert.That(deserialized, Is.TypeOf(type), $"Deserialized object should be of type '{type.Name}'.");
            }
        }

        private static IEnumerable<TestCaseData> GetProtoContractTypes()
        {
            Assembly assembly = typeof(MeshKernelApi).Assembly;

            List<Type> protoContractTypes = assembly.GetTypes()
                                                    .Where(t => t.GetCustomAttribute<ProtoContractAttribute>() != null)
                                                    .OrderBy(t => t.Name)
                                                    .ToList();

            foreach (Type type in protoContractTypes)
            {
                yield return new TestCaseData(type).SetName($"{type.Name}");
            }
        }
    }
}