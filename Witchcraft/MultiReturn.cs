using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

/*
 * Note:
 * 
 * This really isn't "Witchcraft" yet - I'm looking into ways to overload explicit operators so that casting to different types can be handled separately.
 * The self-awaitable pattern is neat, though.
 */

namespace Witchcraft
{
	/// <summary>
	/// Store several different values to be retrieved in several different ways.
	/// </summary>
	/// <example>
	/// var mult = new MultiReturn<int>
	/// {
	///		Implicit = 50,
	///		Awaited = 25,
	///		Indexer = 5 // Infinite sequence of 5 (pass a collection to use it non-infinitely)
	/// };
	/// 
	/// int regular = mult;        // 20
	/// int awaited = await mult;  // 25
	/// int indexer = mult[3];     // 5
	/// </example>
	/// <typeparam name="T">Type of value to store.</typeparam>
	public sealed class MultiReturn<T> : INotifyCompletion
	{
		public T Implicit { get; set; }
		public T Awaited { get; set; }
		public MultiCollection Indexer { get; set; }

		// Await
		public bool IsCompleted { get => true; }
		public void OnCompleted(Action @void) { }
		public T GetResult() => Awaited;
		public MultiReturn<T> GetAwaiter() => this;

		// Implicit
		public static implicit operator T(MultiReturn<T> mult) => mult.Implicit;

		// Indexer
		public T this[int i] { get => Indexer[i]; }
		public struct MultiCollection
		{
			private IEnumerable<T> _values;
			public T this[int i] => _values.ElementAtOrDefault(i);

			private MultiCollection(IEnumerable<T> values) => _values = values;

			private static IEnumerable<T> InfiniteSequence(T val)
			{
				while (true) yield return val;
			}

			public static implicit operator MultiCollection(T val) => new MultiCollection(InfiniteSequence(val));
			public static implicit operator MultiCollection(T[] vals) => new MultiCollection(vals);
		}
	}
}
