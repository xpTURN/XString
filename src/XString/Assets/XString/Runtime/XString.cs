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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf16ValueStringBuilder CreateStringBuilder() =>
        ZString.CreateStringBuilder();

    /// <summary>Creates Utf16 StringBuilder. When notNested is true, uses thread-static buffer (no nesting). Wrapper for ZString.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf16ValueStringBuilder CreateStringBuilder(bool notNested) =>
        ZString.CreateStringBuilder(notNested);

    /// <summary>Creates Utf8 StringBuilder. Wrapper for ZString.CreateUtf8StringBuilder().</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8ValueStringBuilder CreateUtf8StringBuilder() =>
        ZString.CreateUtf8StringBuilder();

    /// <summary>Creates Utf8 StringBuilder. When notNested is true, uses thread-static buffer. Wrapper for ZString.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Utf8ValueStringBuilder CreateUtf8StringBuilder(bool notNested) =>
        ZString.CreateUtf8StringBuilder(notNested);

    /// <summary>Applies format string. Wrapper for ZString.Format(format, arg1).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1>(string format, T1 arg1) =>
        ZString.Format(format, arg1);

    /// <summary>Applies format string. Wrapper for ZString.Format(format, arg1, arg2).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2>(string format, T1 arg1, T2 arg2) =>
        ZString.Format(format, arg1, arg2);

    /// <summary>Applies format string. Wrapper for ZString.Format(format, arg1, arg2, arg3).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2, T3>(string format, T1 arg1, T2 arg2, T3 arg3) =>
        ZString.Format(format, arg1, arg2, arg3);

    /// <summary>Applies format string. Wrapper for ZString.Format(format, arg1, arg2, arg3, arg4).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2, T3, T4>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4) =>
        ZString.Format(format, arg1, arg2, arg3, arg4);

    /// <summary>Applies format string. Wrapper for ZString.Format.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2, T3, T4, T5>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) =>
        ZString.Format(format, arg1, arg2, arg3, arg4, arg5);

    /// <summary>Applies format string. Wrapper for ZString.Format.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2, T3, T4, T5, T6>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) =>
        ZString.Format(format, arg1, arg2, arg3, arg4, arg5, arg6);

    /// <summary>Applies format string. Wrapper for ZString.Format.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2, T3, T4, T5, T6, T7>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) =>
        ZString.Format(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

    /// <summary>Applies format string. Wrapper for ZString.Format.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2, T3, T4, T5, T6, T7, T8>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) =>
        ZString.Format(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);

    /// <summary>Applies format string. Wrapper for ZString.Format.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) =>
        ZString.Format(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);

    /// <summary>Applies format string. Wrapper for ZString.Format.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) =>
        ZString.Format(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);

    /// <summary>Applies format string. Wrapper for ZString.Format.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) =>
        ZString.Format(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);

    /// <summary>Applies format string. Wrapper for ZString.Format.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) =>
        ZString.Format(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);

    /// <summary>Applies format string. Wrapper for ZString.Format.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) =>
        ZString.Format(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);

    /// <summary>Applies format string. Wrapper for ZString.Format.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) =>
        ZString.Format(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);

    /// <summary>Applies format string. Wrapper for ZString.Format.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) =>
        ZString.Format(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);

    /// <summary>Applies format string. Wrapper for ZString.Format.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string format, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) =>
        ZString.Format(format, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);

    /// <summary>Joins sequence with separator. Wrapper for ZString.Join(separator, values).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join<T>(char separator, params T[] values) =>
        ZString.Join(separator, values);

    /// <summary>Joins sequence with separator. Wrapper for ZString.Join(separator, values).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join<T>(string separator, params T[] values) =>
        ZString.Join(separator, values);

    /// <summary>Concatenates arguments as strings. Wrapper for ZString.Concat(values).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Concat<T>(params T[] values) =>
        ZString.Concat(values);
}