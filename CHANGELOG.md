# Changelog

---

## [0.1.3] - 2026-02-26

### Bug Fixes

#### Fix retry buffer size calculation in AppendFormatInternal

- Changed retry buffer from `written > buf.Length ? written : buf.Length * 2` to `buf.Length * 2`
- `written` is unreliable on TryFormat failure (may be 0 or partial), so always double the buffer

### Improvements

#### Add diagnostic info to FormatException

- FormatException message now includes `Type`, `alignment`, `format` for easier debugging

## [0.1.2] - 2026-02-26

### Performance Improvements

#### Replace Composite Format with Direct TryFormatDelegate

- Removed `AppendFormattedViaComposite`, replaced with `AppendFormatInternal`
- Before: built composite format string (`"{0,alignment:format}"`) on the stack per call, then re-parsed via `_sb.AppendFormat`
- After: calls `FormatterCache<T>.TryFormatDelegate` directly and handles alignment padding inline
- Eliminates composite string construction and re-parsing overhead

#### Avoid Boxing on Null Check (Debug build only)

- Changed null check from `value == null` to `default(T) == null && value == null`
- Prevents unnecessary boxing for value types (null branch entered only for reference types / Nullable)

### Tests

#### Benchmark Test Improvements

- `StringBuilder_FourArgs`: changed from reusing StringBuilder to creating a new instance per iteration (fair comparison)
- `StringBuilder128_FourArgs`: added benchmark that creates a new StringBuilder with initial capacity 128 per iteration
- `StringBuilderShare_FourArgs`: separated the existing shared-StringBuilder approach into its own benchmark

## [0.1.1] - 2026-02-24

### New Features

#### Format Parameter Support

Alignment and format specifiers are now properly supported.

- Fixed: Alignment and format parameters were previously ignored
- Example: `XString.Format($"Score: {score,10:F2}")` aligns to 10 characters with 2 decimal places

#### Support for Up to 16 Format Arguments

Format overloads extended from T1~T4 to T1~T16.

- Example: `XString.Format("{0} {1} ... {15}", arg1, arg2, ..., arg16)`

### Tests

Expanded from 34 to 93 (+174%)

Added tests:

- 4 concurrency tests (multithreading safety)
- 11 edge case tests (boundary conditions, large strings)
- 33 functional tests (various scenarios)

---

## [0.1.0] - 2026-02-24

- First release
