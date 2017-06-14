using System;
using System.ComponentModel;
using Witchcraft;

namespace Examples.Demos
{
	[DisplayName("AutoClone Demo")]
	internal class AutoClone : IDemo
	{
		public void Run()
		{
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
		}

		private class TestThing
		{
			public GenericThing<int> GenericInt { get; set; } = new GenericThing<int>(5);
			public double Double { get; private set; } = 0.56;

			public void SetDouble(double d)
			{
				Double = d;
			}
		}

		private class GenericThing<T>
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
}
