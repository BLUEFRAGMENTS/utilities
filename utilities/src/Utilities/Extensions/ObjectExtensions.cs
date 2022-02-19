using System;

namespace Utilities.Extensions;

public static class ObjectExtensions
{
    /// <summary>
    /// Throws NullReferenceException if object is null.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="objectName"></param>
    /// <param name="message"></param>
    public static void ThrowIfObjectIsNull(this object obj, string objectName, string? message = null)
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
