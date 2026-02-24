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

    /// <summary>Appends object as string. alignment/format are currently unused; signature for compatibility.</summary>
    public void AppendFormatted(object value, int alignment = 0, string format = null)
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

    /// <summary>
    /// Appends with format string applied.
    /// _sb.AppendFormat(value, format) is not supported; uses _sb.AppendFormat("{0:format}", value) instead.
    /// </summary>
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

            int maxLen = "{0:}".Length + format.Length * sizeof(char);
            Span<char> buf = stackalloc char[maxLen];

            int p = 0;
            buf[p++] = '{';
            buf[p++] = '0';
            buf[p++] = ':';

            for (int i = 0; i < format.Length && p < buf.Length - 1; i++)
            {
                char c = format[i];
                // In composite format, '}' closes the placeholder; literal '}' in format must be escaped as "}}".
                // Examples:
                //   format "custom}"  → "{0:custom}}"  (output "}}" so the last '}' is treated as literal)
                //   format "a}b}c"    → "{0:a}}b}}c}"  (each '}' in format replaced with "}}")
                if (c == '}')
                {
                    if (p + 2 <= buf.Length)
                    {
                        buf[p++] = '}';
                        buf[p++] = '}';
                    }
                }
                else
                {
                    buf[p++] = c;
                }
            }

            if (p < buf.Length) buf[p++] = '}';

            _sb.AppendFormat(buf.Slice(0, p), value);
        }
        catch (Exception ex)
        {
            _failed = true;
            _exception = ex;
        }
    }
}