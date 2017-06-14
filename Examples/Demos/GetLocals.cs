using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witchcraft;

namespace Examples.Demos
{
	[DisplayName("GetLocals Demo")]
	internal class GetLocals : IDemo
	{
		public void Run()
		{
			Task.Run(async () =>
			{
				// Define some locals
				int specialNumber = 42;
				string description = "This is in a regular async method";

				// This will get all locals, including entry, name, and local defined "below"
				var locals = await AsyncContext.GetLocals();
				
				foreach(var entry in locals)
				{
					var name = entry.Key;
					var local = entry.Value;

					Console.WriteLine($"{name}: {local.Value} ({local.Type.Name})");
				}
			}).Wait();
		}
	}
}
