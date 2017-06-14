using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Witchcraft
{
	public static class AsyncContext
	{
		/// <summary>
		/// Returns a dictionary of local variables in the current async state machine.
		/// </summary>
		/// The meat of this was inspired/taken from Jon Skeet's "Abusing C#" talk!
		/// <see href="https://github.com/jskeet/DemoCode/blob/master/Abusing%20CSharp/Code/FunWithAwaiters/SaveState.cs" />
		/// Will not work with optimizations enabled!  (Release mode)
		public static Awaiter GetLocals() => new Awaiter((continuation, complete) =>
		{
			// Get currently executing async state machine
			var target = continuation.Target;
			var machineField = target.GetType().GetField("m_stateMachine", BindingFlags.NonPublic | BindingFlags.Instance);
			var source = (IAsyncStateMachine)machineField.GetValue(target);

			// The compiler generates a class with fields representing the state machine's locals
			// Read those fields, and perform some optional filtering
			var fields = source.GetType()
					.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
					.Where(f => !f.Name.StartsWith("<>")) // Locals look like "<varName>X__X", we want only those
					.Where(f => f.FieldType != typeof(IDictionary<string, Local>)); // Variables intended to store the result of this call are probably useless noise for the caller

			var locals = new Dictionary<string, Local>(fields.Count());

			foreach (var field in fields)
			{
				// Extract name and other details from runtime fieldinfo
				var type = field.FieldType;
				var value = field.GetValue(source);
				var name = Regex.Match(field.Name, @"^<(.+)>").Groups.Cast<Group>().ElementAtOrDefault(1)?.Value ?? field.Name;

				locals.Add(name, new Local
				{
					Field = field,
					Type = type,
					Value = value
				});
			}

			// Locals are now known, allow GetResult to complete
			complete.SetResult(locals);

			// Allow GetLocals itself to complete
			source.MoveNext();
		});

		public sealed class Awaiter : INotifyCompletion
		{
			/// <summary>Used for coordinating between the sync GetResult method and async caller.</summary>
			private TaskCompletionSource<IDictionary<string, Local>> _complete;

			/// <summary>Callback to forward the state machine to.</summary>
			private readonly Action<Action, TaskCompletionSource<IDictionary<string, Local>>> _handler;

			public bool IsCompleted { get => false; }

			public Awaiter(Action<Action, TaskCompletionSource<IDictionary<string, Local>>> handler) => _handler = handler;

			/// <summary>Reset the coordinator and get the state machine locals.</summary>
			public void OnCompleted(Action continuation)
			{
				// Sometimes, continuation is a ContinuationWrapper, and other times it is a MoveNextRunner -- we want the latter
				// Fortunately if it's a ContinuationWrapper, a MoveNextRunner is available at Target.m_continuation.Target!
				var targetType = continuation.Target.GetType();
				var continuationWrapperType = Assembly.Load("mscorlib").GetTypes().First(t => t.Name == "ContinuationWrapper");
				if (targetType == continuationWrapperType)
				{
					// Grab the m_continuation field from this ContinuationWrapper
					continuation = (Action)targetType
						.GetField("m_continuation", BindingFlags.Instance | BindingFlags.NonPublic)
						.GetValue(continuation.Target);
				}

				_complete = new TaskCompletionSource<IDictionary<string, Local>>();
				_handler(continuation, _complete);
			}

			/// <summary>Result given to the await caller.</summary>
			public IDictionary<string, Local> GetResult() => _complete.Task.Result;

			public Awaiter GetAwaiter() => this;
		}

		/// <summary>Object representing a runtime local variable.</summary>
		public struct Local
		{
			public FieldInfo Field;
			public object Value;
			public Type Type;
		}
	}
}
