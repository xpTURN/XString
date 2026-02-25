using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

using Cysharp.Text;

namespace xpTURN.Text.XInterpolatedStringHandler;

/// <summary>
/// Interpolated string handler for XString.Format.
/// Uses Utf16ValueStringBuilder; Dispose must be called when done.
/// </summary>
[InterpolatedStringHandler]
public ref struct _XS
{
    private const int CompositeFormatPrefix = 4; // "{0," length
    private const int MaxAlignmentDigits = 11; // "-1000000000" (int.MinValue formatted)
    private const int CompositeFormatSuffix = 1; // "}" closing brace
    private const int PerCharFormatEscapeFactor = 2; // Each '}' becomes '}}'

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
            if (format == null || format.Length == 0)
            {
                _sb.Append<T>(value);
                return;
            }

            if (value == null)
            {
                AppendNullWithPadding(0);
                return;
            }

            AppendFormattedViaComposite(value, 0, format);
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

            if (value == null)
            {
                AppendNullWithPadding(alignment);
                return;
            }

            AppendFormattedViaComposite(value, alignment, null);
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
            if (alignment == 0 && (format == null || format.Length == 0))
            {
                _sb.Append<T>(value);
                return;
            }

            if (value == null)
            {
                AppendNullWithPadding(alignment);
                return;
            }

            AppendFormattedViaComposite(value, alignment, format);
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
            if (alignment == 0 && (format == null || format.Length == 0))
            {
                _sb.Append(value);
                return;
            }

            if (value == null)
            {
                AppendNullWithPadding(alignment);
                return;
            }

            AppendFormattedViaComposite(value, alignment, format);
        }
        catch (Exception ex)
        {
            _failed = true;
            _exception = ex;
        }
    }

    /// <summary>
    /// Builds a composite format string "{0[,alignment][:format]}" on the stack and delegates to _sb.AppendFormat.
    ///
    /// Design Note: ZString's AppendFormatInternal is internal, so we construct the composite format string
    /// as a workaround to access alignment and format specifier processing via the public AppendFormat(format, value).
    /// This approach avoids reflection/performance penalties while maintaining API compatibility.
    ///
    /// Stack-allocated format string: Avoids GC allocation for most common formatting scenarios.
    /// The format pattern "{0[,alignment][:format]}" is then parsed by ZString's public AppendFormat method.
    /// </summary>
    private void AppendFormattedViaComposite<T>(T value, int alignment, string format)
    {
        // Max alignment digits: "-1000000000" = 11 chars. Construct composite: "{0[,alignment][:format]}"
        // Format part: each '}' is escaped to "}}" so worst case = format.Length * 2, plus ':' separator.
        int formatLen = format != null ? format.Length : 0;
        int maxLen = CompositeFormatPrefix + MaxAlignmentDigits + CompositeFormatSuffix + formatLen * PerCharFormatEscapeFactor;
        Span<char> buf = stackalloc char[maxLen];

        int p = 0;
        buf[p++] = '{';
        buf[p++] = '0';

        // Alignment part: ,alignment
        if (alignment != 0)
        {
            buf[p++] = ',';
            bool written = alignment.TryFormat(buf[p..], out int alignChars);
            if (!written) throw new InvalidOperationException("Alignment value too large for buffer.");
            p += alignChars;
        }

        // Format part: :format (with '}' escaped as "}}")
        // Defensive code for format string construction: C# interpolation syntax prohibits '}' in the format specifier,
        // and ZString's FormatParser doesn't support '}' escaping in format regions, so this path is unreachable in practice.
        // However, we include it for defensive programming and potential future ZString compatibility.
        if (formatLen > 0)
        {
            buf[p++] = ':';
            for (int i = 0; i < format.Length; i++)
            {
                char c = format[i];
                if (c == '}')
                {
                    buf[p++] = '}';
                    buf[p++] = '}';
                }
                else
                {
                    buf[p++] = c;
                }
            }
        }

        buf[p++] = '}';
        _sb.AppendFormat(buf.Slice(0, p), value);
    }

    /// <summary>
    /// Appends padding spaces for a null value. null is treated as empty string, so padding = |alignment|.
    /// </summary>
    private void AppendNullWithPadding(int alignment)
    {
        int width = alignment < 0 ? -alignment : alignment;
        if (width > 0)
            _sb.Append(' ', width);
    }
}