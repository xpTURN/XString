using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

using Cysharp.Text;
using static Cysharp.Text.Utf16ValueStringBuilder;

namespace xpTURN.Text.XInterpolatedStringHandler;

/// <summary>
/// Interpolated string handler for XString.Format.
/// Uses Utf16ValueStringBuilder; Dispose must be called when done.
/// </summary>
[InterpolatedStringHandler]
public ref struct _XS
{
    private const int DefaultFormatBuffer = 1024;

    private Utf16ValueStringBuilder _sb; // Internal StringBuilder
    private bool _failed; // True if an exception occurred during Append; subsequent Appends are skipped and GetString will throw.
    private Exception _exception; // Exception from Append; rethrown when GetString() is called.

    /// <summary>
    /// Returns the formatted result string. Throws the exception that occurred during Append, if any.
    /// </summary>
    public string GetString()
    {
        if (_failed)
        {
            var ex = _exception ?? new InvalidOperationException("Format failed.");
            ExceptionDispatchInfo.Capture(ex).Throw();
            return null!; // unreachable
        }

        return _sb.ToString();
    }

    public Utf16ValueStringBuilder GetBuilder() => _sb;

    /// <summary>Releases the internal StringBuilder.</summary>
    public void Dispose()
    {
        _sb.Dispose();
    }

    /// <summary>Called by the compiler when parsing an interpolated string. literalLength/formattedCount are capacity hints.</summary>
    public _XS(int literalLength, int formattedCount)
    {
        // ZString's Utf16ValueStringBuilder does not use literalLength and formattedCount hint values.
        _sb = ZString.CreateStringBuilder();

        _failed = false;
        _exception = null;
    }

    /// <summary>Appends the literal string as-is.</summary>
    public void AppendLiteral(string value)
    {
        if (_failed) return;
        try
        {
            _sb.Append(value);
        }
        catch (Exception ex)
        {
            _failed = true;
            _exception = ex;
        }
    }

    /// <summary>Appends value of type T as string. null is treated as empty string.</summary>
    public void AppendFormatted<T>(T value)
    {
        if (_failed) return;
        try
        {
            _sb.Append<T>(value);
        }
        catch (Exception ex)
        {
            _failed = true;
            _exception = ex;
        }
    }

    /// <summary>Appends the string as-is.</summary>
    public void AppendFormatted(string value)
    {
        if (_failed) return;
        try
        {
            _sb.Append(value);
        }
        catch (Exception ex)
        {
            _failed = true;
            _exception = ex;
        }
    }

    /// <summary>Appends value with format string applied.</summary>
    public void AppendFormatted<T>(T value, string format)
    {
        if (_failed) return;
        try
        {
            if (string.IsNullOrEmpty(format))
            {
                _sb.Append<T>(value);
                return;
            }

            // Boxing-avoidance note: `default(T) == null` is true only when T is a reference type (or Nullable).
            // This branch skips the format call when T is a reference type and the value is null,
            // avoiding unnecessary boxing and formatting overhead for value types.
            if (default(T) == null && value == null)
            {
                return;
            }

            AppendFormatInternal(value, 0, format);
        }
        catch (Exception ex)
        {
            _failed = true;
            _exception = ex;
        }
    }

    /// <summary>Appends value with alignment applied.</summary>
    public void AppendFormatted<T>(T value, int alignment)
    {
        if (_failed) return;
        try
        {
            if (alignment == 0)
            {
                _sb.Append<T>(value);
                return;
            }

            // Boxing-avoidance note: `default(T) == null` is true only when T is a reference type (or Nullable).
            // This branch skips the format call when T is a reference type and the value is null,
            // avoiding unnecessary boxing and formatting overhead for value types.
            if (default(T) == null && value == null)
            {
                AppendNullWithPadding(alignment);
                return;
            }

            AppendFormatInternal(value, alignment, default);
        }
        catch (Exception ex)
        {
            _failed = true;
            _exception = ex;
        }
    }

    /// <summary>Appends value with alignment and format applied.</summary>
    public void AppendFormatted<T>(T value, int alignment, string format)
    {
        if (_failed) return;
        try
        {
            if (alignment == 0 && string.IsNullOrEmpty(format))
            {
                _sb.Append<T>(value);
                return;
            }

            // Boxing-avoidance note: `default(T) == null` is true only when T is a reference type (or Nullable).
            // This branch skips the format call when T is a reference type and the value is null,
            // avoiding unnecessary boxing and formatting overhead for value types.
            if (default(T) == null && value == null)
            {
                AppendNullWithPadding(alignment);
                return;
            }

            AppendFormatInternal(value, alignment, format);
        }
        catch (Exception ex)
        {
            _failed = true;
            _exception = ex;
        }
    }

    /// <summary>Appends string with alignment and/or format applied.</summary>
    public void AppendFormatted(string value, int alignment, string format)
    {
        if (_failed) return;
        try
        {
            if (alignment == 0 && string.IsNullOrEmpty(format))
            {
                _sb.Append(value);
                return;
            }

            if (value == null)
            {
                AppendNullWithPadding(alignment);
                return;
            }

            AppendFormatInternal(value, alignment, format);
        }
        catch (Exception ex)
        {
            _failed = true;
            _exception = ex;
        }
    }

    /// <summary>
    /// Formats value directly via FormatterCache&lt;T&gt;.TryFormatDelegate, bypassing composite format
    /// string construction and re-parsing. Handles alignment (padding) inline.
    /// </summary>
    private void AppendFormatInternal<T>(T value, int alignment, ReadOnlySpan<char> format)
    {
        if (alignment == 0)
        {
            // No alignment: format to stack buffer, then append.
            Span<char> buf = stackalloc char[typeof(T).IsValueType ? Unsafe.SizeOf<T>() * 8 : DefaultFormatBuffer];
            if (!TryFormatValue(value, buf, out var written, format))
            {
                // Retry with larger heap-free buffer.
                buf = stackalloc char[buf.Length * 2];
                if (!TryFormatValue(value, buf, out written, format))
                {
                    throw new FormatException($"Failed to format value. Type={typeof(T).Name}, alignment=0, format={format.ToString()}");
                }
            }
            _sb.Append(buf.Slice(0, written));
            return;
        }

        // With alignment: format first, then pad.
        {
            int width = alignment < 0 ? -alignment : alignment;
            bool leftJustify = alignment < 0;

            Span<char> buf = stackalloc char[typeof(T).IsValueType ? Unsafe.SizeOf<T>() * 8 : DefaultFormatBuffer];
            if (!TryFormatValue(value, buf, out var written, format))
            {
                buf = stackalloc char[buf.Length * 2];
                if (!TryFormatValue(value, buf, out written, format))
                {
                    throw new FormatException($"Failed to format value. Type={typeof(T).Name}, alignment={alignment}, format={format.ToString()}");
                }
            }

            int padding = width - written;
            if (leftJustify)
            {
                _sb.Append(buf.Slice(0, written));
                if (padding > 0) _sb.Append(' ', padding);
            }
            else
            {
                if (padding > 0) _sb.Append(' ', padding);
                _sb.Append(buf.Slice(0, written));
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TryFormatValue<T>(T value, Span<char> dest, out int written, ReadOnlySpan<char> format)
    {
        if (typeof(T).IsEnum && !format.IsEmpty)
            return TryFormatEnum(value, dest, out written, format);
        return FormatterCache<T>.TryFormatDelegate(value, dest, out written, format);
    }

    private static bool TryFormatEnum<T>(T value, Span<char> dest, out int written, ReadOnlySpan<char> format)
    {
        if (format.Length == 1)
        {
            char f = format[0];
            if (f == 'D' || f == 'd')
                return TryFormatEnumUnderlying(value, dest, out written, default);
            if (f == 'X' || f == 'x')
            {
                int hexDigits = Unsafe.SizeOf<T>() * 2;
                Span<char> hexFmt = stackalloc char[3];
                hexFmt[0] = f;
                if (hexDigits >= 10)
                {
                    hexFmt[1] = (char)('0' + hexDigits / 10);
                    hexFmt[2] = (char)('0' + hexDigits % 10);
                    return TryFormatEnumUnderlying(value, dest, out written, hexFmt);
                }
                hexFmt[1] = (char)('0' + hexDigits);
                return TryFormatEnumUnderlying(value, dest, out written, hexFmt.Slice(0, 2));
            }
        }

        // G, F, or other: fallback via IFormattable (boxes)
        var s = ((IFormattable)(object)value).ToString(format.ToString(), null);
        if (s.AsSpan().TryCopyTo(dest))
        {
            written = s.Length;
            return true;
        }
        written = 0;
        return false;
    }

    private static bool TryFormatEnumUnderlying<T>(T value, Span<char> dest, out int written, ReadOnlySpan<char> numericFormat)
    {
        switch (Type.GetTypeCode(typeof(T)))
        {
            case TypeCode.Int32:
                return Unsafe.As<T, int>(ref value).TryFormat(dest, out written, numericFormat, null);
            case TypeCode.UInt32:
                return Unsafe.As<T, uint>(ref value).TryFormat(dest, out written, numericFormat, null);
            case TypeCode.Int64:
                return Unsafe.As<T, long>(ref value).TryFormat(dest, out written, numericFormat, null);
            case TypeCode.UInt64:
                return Unsafe.As<T, ulong>(ref value).TryFormat(dest, out written, numericFormat, null);
            case TypeCode.Int16:
                return Unsafe.As<T, short>(ref value).TryFormat(dest, out written, numericFormat, null);
            case TypeCode.UInt16:
                return Unsafe.As<T, ushort>(ref value).TryFormat(dest, out written, numericFormat, null);
            case TypeCode.Byte:
                return Unsafe.As<T, byte>(ref value).TryFormat(dest, out written, numericFormat, null);
            case TypeCode.SByte:
                return Unsafe.As<T, sbyte>(ref value).TryFormat(dest, out written, numericFormat, null);
            default:
                written = 0;
                return false;
        }
    }

    /// <summary>
    /// Appends padding spaces for a null value. null is treated as empty string, so padding = |alignment|.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AppendNullWithPadding(int alignment)
    {
        int width = alignment < 0 ? -alignment : alignment;
        if (width > 0)
        {
            _sb.Append(' ', width);
        }
    }
}