using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Witchcraft;

namespace Examples.Demos
{
	[DisplayName("GetOffset Demo")]
	internal class GetOffset : IDemo
	{
		public unsafe void Run()
		{
			// Make a predictable object
			var predictable = new PredictableStruct(3, 6);

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

		[StructLayout(LayoutKind.Sequential)]
		private struct PredictableStruct
		{
			public int A;
			public int B;

			public PredictableStruct(int a, int b)
			{
				A = a;
				B = b;
			}
		}
	}
}
