﻿using System.Linq;
using CodeGen.utils;

namespace CodeGen.generators
{
	/// <inheritdoc />
	/// <summary>
	/// Visual Basic language generator
	/// </summary>
	public class VbGenerator : Generator
	{
		private const string ClassFormat = "{0}Class {1}\n{2}{3}{4}{5}";
		private string Indent { get; set; } = GeneratorConf.GetIndent(true, 4);

		/// <inheritdoc />
		protected override string GenerateClass(Class @class)
		{
			string fields = "", inherits = "", methods = "", classes = "";

			if (@class.Parent?.Length > 0)
			{
				inherits = Indent + "Inherits " + @class.Parent + "\n\n";
			}
			
			fields = @class.Fields?.Aggregate(fields,
				(current, field) => current + GenerateField(field) + '\n');
			
			methods = @class.Methods?.Aggregate(methods,
				(current, method) => current + '\n' + GeneratorConf.ShiftCode(GenerateMethod(method), 1, Indent) + '\n');
	
			classes = @class.Classes?.Aggregate('\n' + classes,
				(current, cls) => current + GeneratorConf.ShiftCode(GenerateClass(cls), 1, Indent) + '\n');
			
			var access = "";
			if (@class.Access?.Length > 0)
			{
				access = @class.Access + ' ';
			}
			
			return string.Format(ClassFormat, access, @class.Name, inherits, fields, methods, classes) + "\nEnd Class";
		}

		/// <inheritdoc />
		protected override string GenerateField(Field field)
		{
			var result = Indent + field.Access + ' ' + field.Name + " As " + field.Type;
			if (field.Default?.Length > 0)
			{
				result += " = " + field.Default;
			}
			return result;
		}

		/// <inheritdoc />
		protected override string GenerateMethod(Method method)
		{
			var result = method.Access + ' ';
			var type = method.Return?.Length > 0 ? "Function" : "Sub";
			result += type + ' ' + method.Name + '(';

			for (var i = 0; i < method.Parameters?.Length; i++)
			{
				var parameter = method.Parameters[i].Name + " As " + method.Parameters[i].Type;
				if (method.Parameters[i].Default?.Length > 0)
				{
					result += "Optional " + parameter + " = " + method.Parameters[i].Default;
				}
				else
				{
					result += "ByVal " + parameter;
				}

				if (i + 1 < method.Parameters.Length)
				{
					result += ", ";
				}
			}

			result += ')';
			if (type == "Function")
			{
				result += " As " + method.Return + '\n' + Indent + "Return 0";
			}
			else
			{
				result += '\n' + Indent + "Return";
			}

			result += "\nEnd " + type;
			return result;
		}
	}

	/// <inheritdoc />
	internal class VbNormalizer : Normalizer
	{
		/// <inheritdoc />
		public override Class NormalizeClass(ref Class @class)
		{
			base.NormalizeClass(ref @class);
			@class.Access = Utils.Title(@class.Access);
			return @class;
		}

		/// <inheritdoc />
		protected override Field NormalizeField(ref Field field)
		{
			base.NormalizeField(ref field);
			field.Access = Utils.Title(field.Access);
			return field;
		}

		/// <inheritdoc />
		protected override Method NormalizeMethod(ref Method method)
		{
			base.NormalizeMethod(ref method);
			method.Access = Utils.Title(method.Access);
			return method;
		}

		/// <inheritdoc />
		protected override string NormalizeType(string type)
		{
			switch (type)
			{
				case "int":
					type = "Integer";
					break;
				case "void":
					type = "";
					break;
				default:
					type = Utils.Title(type);
					break;
			}
			return type;
		}
	}
}
