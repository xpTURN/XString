using System;
using System.Runtime.CompilerServices;

using Cysharp.Text;
using xpTURN.Text.XInterpolatedStringHandler;

namespace xpTURN.Text;

/// <summary>ZString API wrapper. Use <c>XString.Format($"...")</c> for interpolated strings, <c>XString.Format("...", arg1, arg2)</c> for format strings.</summary>
public static class XString
{
    /// <summary>
    /// Returns the interpolated string as the final result. Use as <c>XString.Format($"Hello, {name}")</c>.
    /// The handler is always disposed in a finally block on method entry.
    /// </summary>
    public static string Format([InterpolatedStringHandlerArgument] ref _XS handler)
    {
        try
        {
            return handler.GetString();
        }
        finally
        {
            handler.Dispose();
        }
    }

    /// <summary>Creates Utf16 StringBuilder. Wrapper for ZString.CreateStringBuilder().</summary>
    public static Utf16ValueStringBuilder CreateStringBuilder() => ZString.CreateStringBuilder();

    /// <summary>Creates Utf16 StringBuilder. When notNested is true, uses thread-static buffer (no nesting). Wrapper for ZString.</summary>
    public static Utf16ValueStringBuilder CreateStringBuilder(bool notNested) => ZString.CreateStringBuilder(notNested);

    /// <summary>Creates Utf8 StringBuilder. Wrapper for ZString.CreateUtf8StringBuilder().</summary>
    public static Utf8ValueStringBuilder CreateUtf8StringBuilder() => ZString.CreateUtf8StringBuilder();

    /// <summary>Creates Utf8 StringBuilder. When notNested is true, uses thread-static buffer. Wrapper for ZString.</summary>
    public static Utf8ValueStringBuilder CreateUtf8StringBuilder(bool notNested) => ZString.CreateUtf8StringBuilder(notNested);

    /// <summary>Applies format string. Wrapper for ZString.Format(format, arg1).</summary>
    public static string Format<T1>(string format, T1 arg1) => ZString.Format(format, arg1);

    /// <summary>Applies format string. Wrapper for ZString.Format(format, arg1, arg2).</summary>
    public static string Format<T1, T2>(string format, T1 arg1, T2 arg2) => ZString.Format(format, arg1, arg2);

    /// <summary>Applies format string. Wrapper for ZString.Format(format, arg1, arg2, arg3).</summary>
    public static string Format<T1, T2, T3>(string format, T1 arg1, T2 arg2, T3 arg3) => ZString.Format(format, arg1, arg2, arg3);

    /// <summary>Applies format string. Wrapper for ZString.Format(format, arg1, arg2, arg3, arg4).</summary>
    public static string Format<T1, T2, T3, T4>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4) => ZString.Format(format, arg1, arg2, arg3, arg4);

    /// <summary>Joins sequence with separator. Wrapper for ZString.Join(separator, values).</summary>
    public static string Join<T>(char separator, params T[] values) => ZString.Join(separator, values);

    /// <summary>Joins sequence with separator. Wrapper for ZString.Join(separator, values).</summary>
    public static string Join<T>(string separator, params T[] values) => ZString.Join(separator, values);

    /// <summary>Concatenates arguments as strings. Wrapper for ZString.Concat(values).</summary>
    public static string Concat<T>(params T[] values) => ZString.Concat(values);
}