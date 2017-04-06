using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Witchcraft
{
	public static class AutoCloneExtension
	{
		/// <summary>
		/// Deeply clone an object.  This means that changes made to the original object will not be reflected in the new one.
		/// </summary>
		/// <typeparam name="T">The object type.</typeparam>
		/// <param name="obj">The object to clone.</param>
		/// <returns>A new, (hopefully) identical object with a entirely different pointers.</returns>
		public static T AutoClone<T>(this T obj)
		{
			if (obj.Equals(default(T)))
			{
				return default(T);
			}

			var type = obj.GetType();

			if (type.IsValueType || type == typeof(string))
			{
				return obj;
			}

			if (type.IsArray)
			{
				var arr = obj as Array;
				var newArr = Array.CreateInstance(type.GetElementType(), arr.Length);
				arr.CopyTo(newArr, 0);
				return (T)(object)newArr;
			}

			// Create object in memory, regardless of constructors
			// https://github.com/dotnet/coreclr/blob/5957f2dde0f90c2bdaa4f0a83339c87140cc657b/src/vm/reflectioninvocation.cpp#L2751
			var inst = (T)FormatterServices.GetUninitializedObject(type);
			var fields = type.GetRuntimeFields().Where(f => !f.IsLiteral);

			foreach (var field in fields)
			{
				field.SetValue(inst, field.GetValue(obj).AutoClone());
			}

			return inst;
		}
	}
}
