using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Witchcraft;

namespace Examples
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var demos = Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(t => t.IsClass)
				.Where(t => typeof(IDemo).IsAssignableFrom(t))
				.Select((t, i) =>
				{
					var name = t.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
					var instance = (IDemo)Activator.CreateInstance(t);

					return new
					{
						Id = ((char)('a' + i)).ToString(),
						Name = $"{name} - {t.Name}",
						Instance = instance
					};
				});

			Console.WriteLine("Choose a demo:");
			foreach (var demo in demos)
			{
				Console.WriteLine($"{demo.Id}: {demo.Name}");
			}

			var choice = Console.ReadLine();

			Console.WriteLine();
			demos.FirstOrDefault(d => d.Id == choice)?.Instance.Run();

			Console.WriteLine();
			Console.WriteLine("Waiting for input...");
			Console.ReadKey();
		}
	}
}
