# Changelog

---

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
