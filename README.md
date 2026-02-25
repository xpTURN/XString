# xpTURN.XString

**ZString** API wrapper package for Unity. ZString does not provide a dedicated API for interpolated strings, so this package aims to provide `XString.Format($"...")` and format-string overloads.

- `XString.Format($"...")`: Interpolated string API.
  - Uses ZString's Utf16ValueStringBuilder at compile time so that code is expanded into efficient calls, reducing runtime cost and heap usage.
  - Helps avoid argument mismatches (e.g. arg1, arg2) and related runtime errors; a preferred style when using format-style APIs.

## Requirements

- **Unity** 2022.3 (12f1)
- **ZString** 2.6+
- **xpTURN.Polyfill** 0.3.0+ (required for C# 10/11 features, or configure manually)

### xpTURN.Polyfill Installation

📦 [xpTURN.Polyfill](https://github.com/xpTURN/Polyfill)

<details>
<summary>How to add xpTURN.Polyfill to your project (when not installed or configured)</summary>

To use XString (with interpolated strings / C# 10+), you need to modify the project settings to use the C# preview language version (e.g. `-langversion:preview`, adding Polyfill code).

- ⚠️ There can be various ways to set this up. You can skip this section if you have already added it to your project.

### 1 Install xpTURN.Polyfill

1. Open **Window > Package Manager**
2. Click **+** > **Add package from git URL...**

```text
https://github.com/xpTURN/Polyfill.git?path=src/Polyfill/Assets/Polyfill
```

1. ⚙️ Run Edit > Polyfill > Player Settings > `Apply Additional Compiler Arguments -langversion (All Installed Platforms)`

</details>

### ZString Installation

📦 [Cysharp/ZString](https://github.com/Cysharp/ZString)

<details>
<summary>How to add ZString to your project (when not installed)</summary>

### Install ZString

1. Open **Window > Package Manager**
2. Click **+** > **Add package from git URL...**
3. Enter:

```text
https://github.com/Cysharp/ZString.git?path=src/ZString.Unity/Assets/Scripts/ZString
```

</details>

## xpTURN.XString – Installation

1. Open **Window > Package Manager**
2. Click **+** > **Add package from git URL...**

```text
https://github.com/xpTURN/XString.git?path=src/XString/Assets/XString
```

## Usage

### Interpolated strings (`$"..."`)

Use interpolated strings only in the form `XString.Format($"...")`. Internally, ZString's Utf16ValueStringBuilder is used to process the interpolated string.

```csharp
using xpTURN.Text;

string name = "Alice";
int score = 100;
string result = XString.Format($"Hello, {name}! Score: {score}");
// result == "Hello, Alice! Score: 100"
```

### Format strings (`"{0}"`)

Use format strings as `XString.Format(format, arg1, arg2, ...)`. (Thin wrapper over `ZString.Format(format, arg1, arg2, ...)`.)

```csharp
XString.Format("{0}", 42);
XString.Format("{0} + {1}", "a", "b");
XString.Format("{0}-{1}-{2}-{3}", "A","B","C","D");
```

### Other APIs

| Method                                        | Description                                                         |
| --------------------------------------------- | ------------------------------------------------------------------- |
| `XString.CreateStringBuilder()`               | Create Utf16ValueStringBuilder (ZString wrapper)                    |
| `XString.CreateStringBuilder(bool notNested)` | When `notNested: true`, uses thread-static buffer (ZString wrapper) |
| `XString.CreateUtf8StringBuilder()`           | Create Utf8 StringBuilder (ZString wrapper)                         |
| `XString.Join(separator, values)`             | Join sequence with separator (ZString wrapper)                      |
| `XString.Concat(values)`                      | Concatenate arguments in order (ZString wrapper)                    |

## Namespace

- Runtime: `xpTURN.Text`

## Performance benchmarks

### Benchmark scenarios

Each iteration creates a **new instance** (no StringBuilder reuse across iterations). This reflects real-world usage where callers typically create and discard builders per operation.

### Verification source code

<details>
<summary>XString (interpolated)</summary>

```csharp
_ = XString.Format($"User: {Label} | Score: {Score:N2} | At: {At:yyyy-MM-dd HH:mm} | Rate: {Rate:P1}");
```

</details>

<details>
<summary>ZString (format)</summary>

```csharp
_ = ZString.Format("User: {0} | Score: {1:N2} | At: {2:yyyy-MM-dd HH:mm} | Rate: {3:P1}", Label, Score, At, Rate);
```

</details>

<details>
<summary>String (interpolated)</summary>

```csharp
_ = $"User: {Label} | Score: {Score:N2} | At: {At:yyyy-MM-dd HH:mm} | Rate: {Rate:P1}";
```

</details>

<details>
<summary>String.Format</summary>

```csharp
_ = string.Format("User: {0} | Score: {1:N2} | At: {2:yyyy-MM-dd HH:mm} | Rate: {3:P1}", Label, Score, At, Rate);
```

</details>

<details>
<summary>StringBuilder(128)</summary>

```csharp
var sb = new StringBuilder(128);
sb.Append("User: ");
sb.Append(Label);
sb.Append(" | Score: ");
sb.AppendFormat("{0:N2}", Score);
sb.Append(" | At: ");
sb.AppendFormat("{0:yyyy-MM-dd HH:mm}", At);
sb.Append(" | Rate: ");
sb.AppendFormat("{0:P1}", Rate);
_ = sb.ToString();
```

</details>

<details>
<summary>Utf16ValueStringBuilder</summary>

```csharp
using var sb = ZString.CreateStringBuilder();
sb.Append("User: ");
sb.Append(Label);
sb.Append(" | Score: ");
sb.AppendFormat("{0:N2}", Score);
sb.Append(" | At: ");
sb.AppendFormat("{0:yyyy-MM-dd HH:mm}", At);
sb.Append(" | Rate: ");
sb.AppendFormat("{0:P1}", Rate);
_ = sb.ToString();
```

</details>

### Benchmark results

#### GC (Time.GC) — lower is better

| Benchmark                    | GC Sum  |
| ---------------------------- | ------- |
| Utf16ValueStringBuilder      | 80      |
| ZString (format)             | 80      |
| XString (interpolated)       | 80      |
| StringBuilder (shared, 128)  | 140     |
| StringBuilder (new, 128)     | 180     |
| String (format)              | 200     |
| String (interpolated)        | 200     |
| StringBuilder (new, default) | 340     |

- Utf16ValueStringBuilder, XString, and ZString all show similar GC pressure. Only the final `new string()` for the return value is allocated.
- StringBuilder usage suppresses GC allocation somewhat but still generates temporary strings during `AppendFormat` operations.
  - This only applies when instances are reused and optimizations are carefully considered.

#### Time (ms) — lower is better

| Benchmark                    | Avg      |
| ---------------------------- | -------- |
| Utf16ValueStringBuilder      | 196.93   |
| ZString (format)             | 198.02   |
| StringBuilder (shared, 128)  | 198.52   |
| XString (interpolated)       | 198.96   |
| StringBuilder (new, 128)     | 211.03   |
| String (interpolated)        | 221.63   |
| String (format)              | 224.10   |
| StringBuilder (new, default) | 331.41   |

- Order by CPU load (Avg): Utf16ValueStringBuilder ≈ ZString ≈ StringBuilder (shared) ≈ XString < StringBuilder (new, 128) < String.Format ≈ String (interpolated) << StringBuilder (new, default)

---

#### Summary

- 🔴 StringBuilder without initial capacity (`new StringBuilder()`) is by far the worst in both GC (340) and time (331 ms) — buffer resizing causes heavy allocation and slowdown.
- 🔴 Even with capacity hint (`new StringBuilder(128)`), per-call allocation still adds overhead (GC 180, 211 ms).
- ⚠️ StringBuilder with instance reuse (shared, 128) matches ZString-tier performance but requires careful lifecycle management.
- 👍 ZString, Utf16ValueStringBuilder, and XString all achieve the lowest GC (80) and similar time (~197–199 ms).
- With XString interpolated strings, the compiler generates code similar to using Utf16ValueStringBuilder directly.
  - Consider XString when you want to reduce mistakes (e.g. argument order) while keeping good performance.

## License

See [LICENSE](https://github.com/xpTURN/XString/blob/main/LICENSE.md).

## Links

- [Changelog](https://github.com/xpTURN/XString/blob/main/CHANGELOG.md)
- [xpTURN](https://github.com/xpTURN)

