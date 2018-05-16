﻿using Xunit;
using CodeGen.generators;
using System.Collections.Generic;

namespace CodeGen.Test.generators
{
	public class GroovyGeneratorTest
	{
		private static Generator Gen { get; } = new GroovyGenerator();
		private static string Indent { get; } = Gen.GetIndent();

		[Theory]
		[MemberData(nameof(GroovyGeneratorTestData.FieldData), MemberType = typeof(GroovyGeneratorTestData))]
		public void TestGenerateField(Field field, string result)
		{
			Assert.Equal(Gen.GenerateField(field), result);
		}

		[Theory]
		[MemberData(nameof(GroovyGeneratorTestData.MethodData), MemberType = typeof(GroovyGeneratorTestData))]
		public void TestGenerateMethod(Method method, string result)
		{
			Assert.Equal(Gen.GenerateMethod(method), result);
		}

		private class GroovyGeneratorTestData
		{
			public static IEnumerable<object[]> FieldData => new List<object[]>
			{
				new object[]
				{
					new Field {Name = "name", Type = "int", Default = "1"},
					$"{Indent} int name = 1"
				},
				new object[]
				{
					new Field {Name = "fieldName", Type = "string"},
					$"{Indent} String fieldName"
				}
			};

			public static IEnumerable<object[]> MethodData => new List<object[]>
			{
				new object[]
				{
					new Method {Name = "methodName", Access = "public", Type = "int"},
					$"public int methodName() {{\n{Indent}return 0\n}}"
				},
				new object[]
				{
					new Method {Name = "MethodName", Access = "private", Type = "string"},
					$"private String MethodName() {{\n{Indent}return 0\n}}"
				},
				new object[]
				{
					new Method
					{
						Name = "MethodName",
						Access = "private",
						Type = "string",
						Parameters = new[]
						{
							new Parameter {Name = "param1", Type = "int", Default = "100"},
							new Parameter {Name = "param2", Type = "string"},
						}
					},
					$"private String MethodName(int param1, String param2) {{\n{Indent}return 0\n}}"
				}
			};
		}
	}
}