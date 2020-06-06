using System;

namespace Bluefragments.Utilities.Extensions
{
	public static class ObjectExtensions
	{
		/// <summary>
		/// Throws ArgumentNullException if parameter is null.
		/// </summary>
		/// <param name="parameter"></param>
		/// <param name="parameterName">Parameter name used for exception message.</param>
		public static void ThrowIfParameterIsNull(this object parameter, string parameterName)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException(parameterName);
			}
		}

		/// <summary>
		/// Throws NullReferenceException if object is null.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objectName"></param>
		/// <param name="message"></param>
		public static void ThrowIfObjectIsNull(this object obj, string objectName, string message = null)
		{
			if (obj == null)
			{
				string msg = string.Empty;

				if (!string.IsNullOrWhiteSpace(message))
				{
					msg = " " + message;
				}

				throw new NullReferenceException($"{objectName} is null.{msg}");
			}
		}
	}
}
