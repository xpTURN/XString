# xpTURN.XString

**ZString** API wrapper package for Unity. ZString does not provide a dedicated API for interpolated or format strings, so this package aims to provide `XString.Format($"...")` and format-string overloads.

- `XString.Format($"...")`: Interpolated string API.
  - Uses ZString's Utf16ValueStringBuilder at compile time so that code is expanded into efficient calls, reducing runtime cost and heap usage.
  - Helps avoid argument mismatches (e.g. arg1, arg2) and related runtime errors; a preferred style when using format-style APIs.

## Requirements

- **Unity** 2022.3 (12f1)
- **ZString** 2.6+
- **xpTURN.Polyfill** 0.3.0+ (required for C# 10/11 features, or configure manually)

### xpTURN.Polyfill Installation

<details>
<summary>How to add xpTURN.Polyfill to your project (when not installed or configured)</summary>

To use XString (with interpolated strings / C# 10+), you need to modify the project settings to use the C# preview language version (e.g. `-langversion:preview`, adding Polyfill code).

- ⚠️ There can be various ways to set this up. You can skip this section if you have already added it to your project.

### 1 Install xpTURN.Polyfill

📦 [xpTURN.Polyfill](https://github.com/xpTURN/Polyfill)

1. Open **Window > Package Manager**
2. Click **+** > **Add package from git URL...**

```text
https://github.com/xpTURN/Polyfill.git?path=src/Polyfill/Assets/Polyfill
```

1. ⚙️ Run Edit > Polyfill > Player Settings > `Apply Additional Compiler Arguments -langversion (All Installed Platforms)`
</details>

### ZString Installation

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
<summary>StringBuilder</summary>

```csharp
var sb = new StringBuilder();
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


| Benchmark                    | GC Sum  | Note |
| ---------------------------- | ------- | ---- |
| Utf16ValueStringBuilder      | 80      | 👍   |
| XString (interpolated)       | 80      | 👍   |
| ZString (format)             | 80      | 👍   |
| StringBuilder (capacity:128) | 140–180 |      |
| String (format)              | 200     |      |
| String (interpolated)        | 200     |      |
| StringBuilder                | 140–340 |      |


- Utf16ValueStringBuilder, XString, and ZString all show similar GC pressure. Only the final `new string()` for the return value is allocated.
- For StringBuilder, setting capacity in advance helps. Each `AppendFormat` still creates temporary strings.
- When a single StringBuilder instance is reused across iterations (Clear then reuse), GC Sum drops to around 140.

---

#### Time (ms) — lower is better


| Benchmark                  | Min    | Median | Max    | Avg    | StdDev |
| -------------------------- | ------ | ------ | ------ | ------ | ------ |
| Utf16ValueStringBuilder    | 171.12 | 172.75 | 178.69 | 173.26 | 1.84   |
| ZString (format)           | 171.42 | 173.18 | 178.45 | 173.95 | 2.16   |
| StringBuilder (shared)     | 166.43 | 167.77 | 173.96 | 168.67 | 2.11   |
| StringBuilder(128, shared) | 169.06 | 170.86 | 174.62 | 171.40 | 1.60   |
| XString (interpolated)     | 182.89 | 184.19 | 188.63 | 184.57 | 1.60   |
| String (format)            | 191.48 | 193.78 | 197.95 | 194.30 | 1.62   |
| String (interpolated)      | 193.24 | 194.32 | 200.48 | 195.13 | 1.85   |


- Order by CPU load (Avg): StringBuilder (shared), StringBuilder(128) < Utf16ValueStringBuilder, ZString < XString < String.Format, string interpolation.
- XString is slightly slower than ZString because it converts e.g. `(value, "D1")` into `AppendFormat("{0:D1}", value)`; that conversion uses stackalloc, so no extra heap allocation.

---

#### Summary

- Using StringBuilder alone gives limited GC savings (especially considering coding cost).
- Utf16ValueStringBuilder’s AppendFormat using stackalloc is key to low GC.
- ZString and Utf16ValueStringBuilder perform very well. 👍
- With XString interpolated strings, the compiler generates code similar to using Utf16ValueStringBuilder directly.
  - Consider XString when you want to reduce mistakes (e.g. argument order) while keeping good performance.

## License

See [LICENSE](https://github.com/xpTURN/XString/blob/main/LICENSE.md).

## Links

- [Changelog](https://github.com/xpTURN/XString/blob/main/CHANGELOG.md)
- [xpTURN](https://github.com/xpTURN)

