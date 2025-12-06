using System;

namespace NeoAI
{
	public static class Tools
	{
		public static T[,] ResizeArray<T>(this T[,] matrix, int newRows, int newColumns)
		{
			var result = new T[newRows, newColumns];
			int oldRows = matrix.GetLength(0);
			int oldColumns = matrix.GetLength(1);
			for (int i = 0; i < Math.Min(oldRows, newRows); i++)
			{
				Array.Copy(matrix, i * oldColumns, result, i * newColumns, Math.Min(oldColumns, newColumns));
			}
			return result;
		}
		public static byte[] ToBytes<T>(this T[,] array) where T : struct
		{
			var buffer = new byte[array.GetLength(0) * array.GetLength(1) * System.Runtime.InteropServices.Marshal.SizeOf(typeof(T))];
			Buffer.BlockCopy(array, 0, buffer, 0, buffer.Length);
			return buffer;
		}
		public static void FromBytes<T>(this T[,] array, byte[] buffer) where T : struct
		{
			var len = Math.Min(array.GetLength(0) * array.GetLength(1) * System.Runtime.InteropServices.Marshal.SizeOf(typeof(T)), buffer.Length);
			Buffer.BlockCopy(buffer, 0, array, 0, len);
		}
		public static byte? SafeByte(string _val)
		{
			if (_val == "") { return 0; }
			byte.TryParse(_val, out byte result);
			return result;
		}
		public static int? SafeInt32(string _val)
		{
			if (_val == "") { return 0; }
			int.TryParse(_val, out int result);
			return result;
		}
		public static decimal? SafeDecimal(string _val)
		{
			if (_val == "") { return 0; }
			decimal.TryParse(_val, out decimal result);
			return result;
		}
		public static DateTime? SafeDateTime(string _val)
		{
			if (_val == "" || _val.Length < 10) { return null; }
			DateTime.TryParse(_val, out DateTime result);
			return result;
		}
		public static dynamic ControledCast<T>(object _val)
		{
			if (typeof(T) == typeof(int))
			{
				return SafeInt32(_val.ToString());
			}
			else if (typeof(T) == typeof(byte))
			{
				return SafeByte(_val.ToString());
			}
			else if (typeof(T) == typeof(decimal))
			{
				return SafeDecimal(_val.ToString());
			}
			else if (typeof(T) == typeof(string))
			{
				return _val.ToString();
			}
			else if (typeof(T) == typeof(DateTime))
			{
				return SafeDateTime(_val.ToString());
			}
			else if (typeof(T) == typeof(Byte[]))
			{
				return (byte[])_val;
			}
			else
			{
				return false;
			}
		}
	}
}
