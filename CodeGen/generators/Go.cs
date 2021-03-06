﻿using System.Linq;

namespace CodeGen.generators
{
	/// <inheritdoc />
	/// <summary>
	/// Generator for Go
	/// </summary>
	public class GoGenerator : Generator
	{
		private const string ClassFormat = "type {0} struct {{{1}}}{2}{3}";
		private string Indent { get; } = GeneratorConf.GetIndent(true, 4);

		/// <inheritdoc />
		protected override string GenerateClass(Class @class)
		{
			string fields = "", methods = "", classes = "";
			fields = @class.Fields?.Aggregate($"\n{fields}", (current, field) => $"{current}{GenerateField(field)}\n");
			methods = @class.Methods?.Aggregate($"\n\n{methods}",
				(current, method) => current + $"func ({@class.Name}) {GenerateMethod(method)}\n\n");
			classes = @class.Classes?.Aggregate(classes, (current, cls) => current + GenerateClass(cls));
			return string.Format(ClassFormat, @class.Name, fields, methods, classes);
		}

		/// <inheritdoc />
		public override string GenerateField(Field field)
		{
			var result = Indent;

			if (field.Access == "public")
			{
				field.Name = field.Name?.First().ToString().ToUpper() + field.Name?.Substring(1);
			}

			result += $"{field.Name} {field.Type}";

			return result;
		}

		/// <inheritdoc />
		public override string GenerateMethod(Method method)
		{
			var result = "";
			if (method.Access == "public")
			{
				method.Name = method.Name?.First().ToString().ToUpper() + method.Name?.Substring(1);
			}
			else
			{
				method.Name = method.Name?.First().ToString().ToLower() + method.Name?.Substring(1);
			}

			result += method.Name + '(';

			for (var i = 0; i < method.Parameters?.Length; i++)
			{
				result += method.Parameters[i].Name + ' ' + method.Parameters[i].Type;
				if (i + 1 < method.Parameters.Length)
				{
					result += ", ";
				}
			}

			result += ')';

			if (method.Type != "")
			{
				result += " " + method.Type;
			}

			result += " {";

			if (method.Type != "")
			{
				result += $"\n{Indent}return nil\n";
			}

			result += '}';

			return result;
		}
	}
}
