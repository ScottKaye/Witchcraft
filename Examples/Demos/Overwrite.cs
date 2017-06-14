using System;
using System.ComponentModel;
using Witchcraft;

namespace Examples.Demos
{
	[DisplayName("Overwrite Demo")]
	internal class Overwrite : IDemo
	{
		public void Run()
		{
			Console.WriteLine($"Do you like cats? (Hint: correct answer is \"absolutely\")");
			Console.Write("> ");

			var answer = Console.ReadLine();
			var len = answer.Length;
			answer.Overwrite("No.");

			Console.Write("You answered: ");
			Console.WriteLine(answer);  // No trickery here!
			Console.Write($"Wait...  My answer was {len} characters, how long is it now? ");
			Console.WriteLine(answer.Length);
			Console.WriteLine("Not fair?  Check the code!");
		}
	}
}
