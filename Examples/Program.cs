using System;
using System.Runtime.InteropServices;
using Witchcraft;

namespace Examples
{
	public class Program
	{
		private static void Main(string[] args)
		{
			// Do you like cats? No.
			Console.WriteLine($"Do you like cats? (Hint: correct answer is \"absolutely\")");
			Console.Write("> ");
			var answer = Console.ReadLine();
			var len = answer.Length;
			answer.Overwrite("No.");
			Console.Write("You answered: ");
			Console.WriteLine(answer);
			Console.Write($"Wait...  My answer was {len} characters, how long is it now? ");
			Console.WriteLine(answer.Length);
			Console.WriteLine("Not fair?  Check the code!");

			Console.WriteLine("\n----------- AutoClone ----------- \n");

			// Make an object, clone it, then change some values on the old object
			// Compare this by setting clone = testThing
			var testThing = new TestThing();
			var clone = testThing.AutoClone();
			testThing.SetDouble(0.1);
			testThing.GenericInt.SettableValue = -1;

			// Original values: 0.1, 5, -1
			// Cloned values: 0.56, 5, 5
			Console.WriteLine($"Original values: {testThing.Double}, {testThing.GenericInt.Value}, {testThing.GenericInt.SettableValue}");
			Console.WriteLine($"Cloned values: {clone.Double}, {clone.GenericInt.Value}, {clone.GenericInt.SettableValue}");

			// Make a predictable object
			var predictable = new PredictableStruct(3, 6);
			unsafe
			{
				// Get offset of predictable struct (just to prove it is the actual memory address)
				var offset = predictable.GetOffset();
				Console.WriteLine($"Offset: {*offset}");

				// The PredictableStruct is 8 bytes, but has 4 bytes of "header" (?)
				// Print out 12 bytes, starting at the offset
				for (var i = 0; i < 12; ++i)
				{
					Console.WriteLine($"{i}: {*(offset + i)}");
				}
			}

			Console.ReadKey();
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct PredictableStruct
	{
		public int A;
		public int B;

		public PredictableStruct(int a, int b)
		{
			A = a;
			B = b;
		}
	}

	internal class TestThing
	{
		public GenericThing<int> GenericInt { get; set; } = new GenericThing<int>(5);
		public double Double { get; private set; } = 0.56;

		public void SetDouble(double d)
		{
			Double = d;
		}
	}

	internal class GenericThing<T>
	{
		public T Value { get; }
		public T SettableValue { get; set; }

		public GenericThing(T value)
		{
			Value = value;
			SettableValue = value;
		}
	}
}
