using System;
using System.Diagnostics.Contracts;

namespace Witchcraft
{
	public static class OverwriteExtension
	{
		/// <summary>
		/// Writes a new value to the interned string.  Be aware that you are writing to unallocated memory if <paramref name="newStr"/> is longer than <paramref name="oldStr"/>.
		/// </summary>
		/// <param name="oldStr">Existing string that will be interned and overwritten with <paramref name="newStr"/>.</param>
		/// <param name="newStr">New string that will replace all literal values of <paramref name="oldStr"/>.</param>
		// Useful info
		// Implementation: https://github.com/dotnet/coreclr/blob/d4a6cf7bc9d1b5ccbf671f1c8fae1b4f83027cbd/src/mscorlib/src/System/String.cs
		// Layout:         https://github.com/dotnet/coreclr/blob/118f88dc17c75800ca249330ea41a963d3bae306/src/vm/object.h#L1093
		// TLDR: Strings in .NET are like P-Strings as far as I can tell...  I am most likely very wrong but hey
		public unsafe static void Overwrite(this string oldStr, string newStr)
		{
			if (oldStr == null) throw new ArgumentNullException(nameof(oldStr));
			if (newStr == null) throw new ArgumentNullException(nameof(newStr));
			if (newStr.Length > 255) throw new ArgumentOutOfRangeException(nameof(newStr), "New string must be less than 256 characters.");
			Contract.EndContractBlock();

			var oldLen = oldStr.Length;
			var newChars = newStr.ToCharArray(); // Get the new string content out of the intern pool

			fixed (char* c = string.Intern(oldStr))
			{
				// String length is stored 2 chars before the string content
				byte* b = (byte*)(c - 2);

				// Clear existing memory (write zeroes)
				for (var i = 0; i < oldLen; ++i)
				{
					c[i] = '\0';
				}

				// Write new string
				for (var i = 0; i < newChars.Length; ++i)
				{
					c[i] = newChars[i];
				}

				// Write new string length
				b[0] = (byte)newChars.Length;

#if WITCHER
				for (var i = 0; i < oldStr.Length * 2 + 4; ++i)
				{
					Console.WriteLine($"{i}: {b[i]}");
				}
#endif
			}
		}
	}
}
