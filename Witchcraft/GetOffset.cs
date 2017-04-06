using System;

namespace Witchcraft
{
	public static class GetOffsetExtension
	{
		/// <summary>
		/// Get the memory offset of a managed object.
		/// </summary>
		// There's likely a better way to get to this result without the crazy casts...
		public static unsafe byte* GetOffset(this object obj)
		{
			TypedReference tr = __makeref(obj);
			return (byte*)**(IntPtr**)(&tr);
		}
	}
}
