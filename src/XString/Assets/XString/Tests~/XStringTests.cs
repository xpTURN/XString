using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Cysharp.Text;
using TMPro;
using UnityEngine;
using xpTURN.Text;
using xpTURN.Text.XInterpolatedStringHandler;

namespace xpTURN.Text.Tests
{
    [TestFixture]
    public class XStringTests
    {
        /// <summary>Runs the action under the given culture and restores the previous culture when done.</summary>
        private static void RunUnderCulture(string cultureName, Action action)
        {
            var prev = Thread.CurrentThread.CurrentCulture;
            var prevUi = Thread.CurrentThread.CurrentUICulture;
            try
            {
                var ci = CultureInfo.GetCultureInfo(cultureName);
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
                action();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = prev;
                Thread.CurrentThread.CurrentUICulture = prevUi;
            }
        }
        [Test]
        public void Format_Interpolated_LiteralOnly_ReturnsLiteral()
        {
            var result = XString.Format($"Hello");
            Assert.That(result, Is.EqualTo("Hello"));
        }

        [Test]
        public void Format_Interpolated_EmptyLiteral_ReturnsEmpty()
        {
            var result = XString.Format($"");
            Assert.That(result, Is.EqualTo(""));
        }

        [Test]
        public void Format_Interpolated_SingleInt_EmbedsValue()
        {
            var result = XString.Format($"value={42}");
            Assert.That(result, Is.EqualTo("value=42"));
        }

        [Test]
        public void Format_Interpolated_SingleString_EmbedsValue()
        {
            var result = XString.Format($"name={"Alice"}");
            Assert.That(result, Is.EqualTo("name=Alice"));
        }

        [Test]
        public void Format_Interpolated_NullReference_EmbedsEmpty()
        {
            string nil = null;
            var result = XString.Format($"x={nil}");
            Assert.That(result, Is.EqualTo("x="));
        }

        // --- Interpolated string: null handling by variable type ---

        [Test]
        public void Format_Interpolated_Null_String_EmbedsEmpty()
        {
            string a = null;
            var result = XString.Format($"prefix_{a}_suffix");
            Assert.That(result, Is.EqualTo("prefix__suffix"));
        }

        [Test]
        public void Format_Interpolated_Null_Object_EmbedsEmpty()
        {
            object a = null;
            var result = XString.Format($"[{a}]");
            Assert.That(result, Is.EqualTo("[]"));
        }

        [Test]
        public void Format_Interpolated_Null_NullableInt_EmbedsEmpty()
        {
            int? a = null;
            var result = XString.Format($"n={a}");
            Assert.That(result, Is.EqualTo("n="));
        }

        [Test]
        public void Format_Interpolated_Null_NullableDouble_EmbedsEmpty()
        {
            double? a = null;
            var result = XString.Format($"x={a}");
            Assert.That(result, Is.EqualTo("x="));
        }

        [Test]
        public void Format_Interpolated_Null_NullableWithFormat_EmbedsEmpty()
        {
            double? a = null;
            var result = XString.Format($"{a:N2}");
            Assert.That(result, Is.EqualTo(""));
        }

        [Test]
        public void Format_Interpolated_Null_ReferenceTypeWithFormat_EmbedsEmpty()
        {
            string a = null;
            var result = XString.Format($"{a:X}");
            Assert.That(result, Is.EqualTo(""));
        }

        [Test]
        public void Format_Interpolated_Null_MixedWithValues_EmbedsEmptyForNullsOnly()
        {
            string s = null;
            int? n = null;
            int ok = 1;
            var result = XString.Format($"s=[{s}] n=[{n}] ok=[{ok}]");
            Assert.That(result, Is.EqualTo("s=[] n=[] ok=[1]"));
        }

        [Test]
        public void Format_Interpolated_MultipleValues_Concatenates()
        {
            var result = XString.Format($"a={1} b={2} c={3}");
            Assert.That(result, Is.EqualTo("a=1 b=2 c=3"));
        }

        [Test]
        public void Format_Interpolated_Unicode_PreservesCharacters()
        {
            var result = XString.Format($"한글={"테스트"}");
            Assert.That(result, Is.EqualTo("한글=테스트"));
        }

        [Test]
        public void Format_Interpolated_WithFormat_IFormattable_UsesFormat()
        {
            var result = XString.Format($"{1234:N0}");
            Assert.That(result, Is.EqualTo(1234.ToString("N0")));
        }

        [Test]
        public void Format_Interpolated_NumberFormat_RespectsCurrentCulture_enUS()
        {
            RunUnderCulture("en-US", () =>
            {
                var result = XString.Format($"{1234.56:N2}");
                Assert.That(result, Is.EqualTo("1,234.56"));
            });
        }

        [Test]
        public void Format_Interpolated_NumberFormat_RespectsCurrentCulture_deDE()
        {
            RunUnderCulture("de-DE", () =>
            {
                var result = XString.Format($"{1234.56:N2}");
                Assert.That(result, Is.EqualTo("1.234,56"));
            });
        }

        [Test]
        public void Format_Interpolated_NumberFormat_RespectsCurrentCulture_koKR()
        {
            RunUnderCulture("ko-KR", () =>
            {
                var result = XString.Format($"{1234:N0}");
                Assert.That(result, Is.EqualTo("1,234"));
            });
        }

        [Test]
        public void Format_Interpolated_NumberFormat_RespectsCurrentCulture_jaJP()
        {
            RunUnderCulture("ja-JP", () =>
            {
                var result = XString.Format($"{1234:N0}");
                Assert.That(result, Is.EqualTo("1,234"));
            });
        }

        [Test]
        public void Format_Interpolated_NumberToString_RespectsCurrentCulture_DecimalSeparator()
        {
            RunUnderCulture("de-DE", () =>
            {
                // double.ToString() uses CurrentCulture (e.g. comma as decimal separator in de-DE)
                var result = XString.Format($"value={1.5}");
                Assert.That(result, Is.EqualTo("value=1,5"));
            });
        }

        [Test]
        public void Format_Interpolated_NumberToString_RespectsCurrentCulture_Invariant()
        {
            RunUnderCulture("en-US", () =>
            {
                var result = XString.Format($"value={1.5}");
                Assert.That(result, Is.EqualTo("value=1.5"));
            });
        }

        [Test]
        public void Format_FormatString_OneArg_AppliesFormat()
        {
            var result = XString.Format("{0}", 42);
            Assert.That(result, Is.EqualTo("42"));
        }

        [Test]
        public void Format_FormatString_TwoArgs_AppliesFormat()
        {
            var result = XString.Format("{0} + {1}", "a", "b");
            Assert.That(result, Is.EqualTo("a + b"));
        }

        [Test]
        public void Format_FormatString_ThreeArgs_AppliesFormat()
        {
            var result = XString.Format("{0},{1},{2}", 1, 2, 3);
            Assert.That(result, Is.EqualTo("1,2,3"));
        }

        [Test]
        public void Format_FormatString_FourArgs_AppliesFormat()
        {
            var result = XString.Format("{0}-{1}-{2}-{3}", "A", "B", "C", "D");
            Assert.That(result, Is.EqualTo("A-B-C-D"));
        }

        [Test]
        public void Join_CharSeparator_JoinsValues()
        {
            var result = XString.Join(',', "a", "b", "c");
            Assert.That(result, Is.EqualTo("a,b,c"));
        }

        [Test]
        public void Join_StringSeparator_JoinsValues()
        {
            var result = XString.Join(" | ", "x", "y", "z");
            Assert.That(result, Is.EqualTo("x | y | z"));
        }

        [Test]
        public void Join_IntArray_JoinsAsStrings()
        {
            var result = XString.Join(",", 1, 2, 3);
            Assert.That(result, Is.EqualTo("1,2,3"));
        }

        [Test]
        public void Concat_Values_Concatenates()
        {
            var result = XString.Concat("a", "b", "c");
            Assert.That(result, Is.EqualTo("abc"));
        }

        [Test]
        public void Concat_IntValues_ConcatenatesToStrings()
        {
            var result = XString.Concat(1, 2, 3);
            Assert.That(result, Is.EqualTo("123"));
        }

        [Test]
        public void CreateStringBuilder_AppendAndToString_ReturnsContent()
        {
            using var sb = XString.CreateStringBuilder();
            sb.Append("Hello");
            sb.Append("World");
            Assert.That(sb.ToString(), Is.EqualTo("HelloWorld"));
        }

        [Test]
        public void CreateUtf8StringBuilder_AppendAndToString_ReturnsContent()
        {
            using var sb = XString.CreateUtf8StringBuilder();
            sb.Append("Test");
            Assert.That(sb.ToString(), Is.EqualTo("Test"));
        }

        /// <summary>Helper that throws from ToString() to verify exception propagation in the interpolated handler.</summary>
        private class ThrowsInToString
        {
            public override string ToString() => throw new InvalidOperationException("ToString failed.");
        }

        [Test]
        public void Format_Interpolated_WhenAppendThrows_GetStringRethrows()
        {
            var bad = new ThrowsInToString();
            var ex = Assert.Throws<InvalidOperationException>(() => XString.Format($"x={bad}"));
            Assert.That(ex.Message, Is.EqualTo("ToString failed."));
        }

        // --- AppendFormatted<T>(T, int alignment): Alignment only ---

        [Test]
        public void Format_Interpolated_Int_RightAlign_PadsLeft()
        {
            var result = XString.Format($"{42,10}");
            Assert.That(result, Is.EqualTo("        42"));
        }

        [Test]
        public void Format_Interpolated_Int_LeftAlign_PadsRight()
        {
            var result = XString.Format($"{42,-10}");
            Assert.That(result, Is.EqualTo("42        "));
        }

        [Test]
        public void Format_Interpolated_Int_AlignmentZero_NoPadding()
        {
            var result = XString.Format($"{42,0}");
            Assert.That(result, Is.EqualTo("42"));
        }

        [Test]
        public void Format_Interpolated_Int_AlignmentSmallerThanValue_NoPadding()
        {
            var result = XString.Format($"{12345,3}");
            Assert.That(result, Is.EqualTo("12345"));
        }

        [Test]
        public void Format_Interpolated_Int_AlignmentExactWidth_NoPadding()
        {
            var result = XString.Format($"{42,2}");
            Assert.That(result, Is.EqualTo("42"));
        }

        [Test]
        public void Format_Interpolated_Double_RightAlign()
        {
            var result = XString.Format($"{3.14,10}");
            Assert.That(result, Is.EqualTo(string.Format("{0,10}", 3.14)));
        }

        [Test]
        public void Format_Interpolated_Double_LeftAlign()
        {
            var result = XString.Format($"{3.14,-10}");
            Assert.That(result, Is.EqualTo(string.Format("{0,-10}", 3.14)));
        }

        // --- AppendFormatted<T>(T, int alignment, string format): Alignment + Format ---

        [Test]
        public void Format_Interpolated_Int_RightAlign_WithFormat()
        {
            RunUnderCulture("en-US", () =>
            {
                var result = XString.Format($"{1234,15:N0}");
                Assert.That(result, Is.EqualTo(string.Format("{0,15:N0}", 1234)));
            });
        }

        [Test]
        public void Format_Interpolated_Int_LeftAlign_WithFormat()
        {
            RunUnderCulture("en-US", () =>
            {
                var result = XString.Format($"{1234,-15:N0}");
                Assert.That(result, Is.EqualTo(string.Format("{0,-15:N0}", 1234)));
            });
        }

        [Test]
        public void Format_Interpolated_Double_RightAlign_WithFormat()
        {
            RunUnderCulture("en-US", () =>
            {
                var result = XString.Format($"{1234.5678,20:F2}");
                Assert.That(result, Is.EqualTo(string.Format("{0,20:F2}", 1234.5678)));
            });
        }

        [Test]
        public void Format_Interpolated_Double_LeftAlign_WithFormat()
        {
            RunUnderCulture("en-US", () =>
            {
                var result = XString.Format($"{1234.5678,-20:F2}");
                Assert.That(result, Is.EqualTo(string.Format("{0,-20:F2}", 1234.5678)));
            });
        }

        [Test]
        public void Format_Interpolated_Int_Hex_RightAlign()
        {
            var result = XString.Format($"{255,10:X}");
            Assert.That(result, Is.EqualTo(string.Format("{0,10:X}", 255)));
        }

        [Test]
        public void Format_Interpolated_Int_Hex_LeftAlign()
        {
            var result = XString.Format($"{255,-10:X}");
            Assert.That(result, Is.EqualTo(string.Format("{0,-10:X}", 255)));
        }

        [Test]
        public void Format_Interpolated_DateTime_WithAlignmentAndFormat()
        {
            var dt = new DateTime(2025, 1, 15, 10, 30, 0);
            var result = XString.Format($"{dt,30:yyyy-MM-dd HH:mm}");
            Assert.That(result, Is.EqualTo(string.Format("{0,30:yyyy-MM-dd HH:mm}", dt)));
        }

        // --- AppendFormatted(string, int, string): String with alignment ---

        [Test]
        public void Format_Interpolated_String_RightAlign()
        {
            var result = XString.Format($"{"hello",10}");
            Assert.That(result, Is.EqualTo("     hello"));
        }

        [Test]
        public void Format_Interpolated_String_LeftAlign()
        {
            var result = XString.Format($"{"hello",-10}");
            Assert.That(result, Is.EqualTo("hello     "));
        }

        [Test]
        public void Format_Interpolated_String_AlignmentSmallerThanValue_NoPadding()
        {
            var result = XString.Format($"{"hello",3}");
            Assert.That(result, Is.EqualTo("hello"));
        }

        [Test]
        public void Format_Interpolated_String_NullWithAlignment_PadsEmpty()
        {
            string nil = null;
            var result = XString.Format($"[{nil,5}]");
            Assert.That(result, Is.EqualTo("[     ]"));
        }

        [Test]
        public void Format_Interpolated_StringVar_RightAlign()
        {
            string s = "abc";
            var result = XString.Format($"{s,8}");
            Assert.That(result, Is.EqualTo("     abc"));
        }

        [Test]
        public void Format_Interpolated_StringVar_LeftAlign()
        {
            string s = "abc";
            var result = XString.Format($"{s,-8}");
            Assert.That(result, Is.EqualTo("abc     "));
        }

        // --- AppendFormatted(object, int, string): Object with alignment and format ---

        [Test]
        public void Format_Interpolated_Object_RightAlign()
        {
            object obj = 42;
            var result = XString.Format($"{obj,10}");
            Assert.That(result, Is.EqualTo("        42"));
        }

        [Test]
        public void Format_Interpolated_Object_LeftAlign()
        {
            object obj = 42;
            var result = XString.Format($"{obj,-10}");
            Assert.That(result, Is.EqualTo("42        "));
        }

        [Test]
        public void Format_Interpolated_Object_WithFormat_IFormattable()
        {
            RunUnderCulture("en-US", () =>
            {
                object obj = 1234;
                var result = XString.Format($"{obj,10:N0}");
                Assert.That(result, Is.EqualTo(string.Format("{0,10:N0}", obj)));
            });
        }

        [Test]
        public void Format_Interpolated_Object_WithFormat_NonFormattable()
        {
            object obj = "test";
            var result = XString.Format($"{obj,10}");
            Assert.That(result, Is.EqualTo("      test"));
        }

        [Test]
        public void Format_Interpolated_Object_Null_WithAlignment()
        {
            object obj = null;
            var result = XString.Format($"[{obj,5}]");
            Assert.That(result, Is.EqualTo("[     ]"));
        }

        // --- Null with alignment: generic types ---

        [Test]
        public void Format_Interpolated_NullableInt_Null_WithAlignment()
        {
            int? n = null;
            var result = XString.Format($"[{n,5}]");
            Assert.That(result, Is.EqualTo("[     ]"));
        }

        [Test]
        public void Format_Interpolated_NullableDouble_Null_WithAlignmentAndFormat()
        {
            double? d = null;
            var result = XString.Format($"[{d,10:F2}]");
            Assert.That(result, Is.EqualTo("[          ]"));
        }

        // --- Mixed: alignment in larger interpolated expressions ---

        [Test]
        public void Format_Interpolated_Mixed_MultipleAlignments()
        {
            var result = XString.Format($"|{"Name",-10}|{"Age",5}|");
            Assert.That(result, Is.EqualTo("|Name      |  Age|"));
        }

        [Test]
        public void Format_Interpolated_Mixed_AlignmentWithLiterals()
        {
            var result = XString.Format($"[{42,6}] = {"hello",-8}!");
            Assert.That(result, Is.EqualTo("[    42] = hello   !"));
        }

        [Test]
        public void Format_Interpolated_TableLike_AlignedColumns()
        {
            RunUnderCulture("en-US", () =>
            {
                var result = XString.Format($"|{"Item",-12}|{99.9,10:F2}|");
                Assert.That(result, Is.EqualTo("|Item        |     99.90|"));
            });
        }

        // --- Edge case tests: Brace escaping in format strings ---

        [Test]
        public void Format_Interpolated_SpecialChar_BraceInFormat_EscapedCorrectly()
        {
            // Escaped braces in interpolated string produce literal braces
            var result = XString.Format($"{{}}");
            Assert.That(result, Is.EqualTo("{}"));
        }

        [Test]
        public void Format_Interpolated_SpecialChar_MultipleBraces_EscapedCorrectly()
        {
            // Test with escaped braces surrounding an interpolation
            var result = XString.Format($"{{{456:x}}}");
            Assert.That(result, Is.EqualTo("{1c8}"));
        }

        [Test]
        public void Format_Interpolated_SpecialChar_OnlyBraces_EscapedCorrectly()
        {
            // Multiple escaped braces producing literal braces
            var result = XString.Format($"{{{{}}}}");
            Assert.That(result, Is.EqualTo("{{}}"));
        }

        [Test]
        public void Format_Interpolated_SpecialChar_MixedContent_EscapedCorrectly()
        {
            // Test that format composition with special characters doesn't cause issues
            var result = XString.Format($"a{{{1000}}}b");
            Assert.That(result, Is.EqualTo("a{1000}b"));
        }

        // --- Edge case tests: Long format strings and stackalloc buffer ---

        [Test]
        public void Format_Interpolated_LongFormat_StackallocEdgeCase_Success()
        {
            // Test with a long interpolated string to exercise buffer allocation.
            var longText = new string('a', 500);
            var result = XString.Format($"{longText} {42}");
            Assert.That(result, Is.EqualTo(longText + " 42"));
        }

        [Test]
        public void Format_Interpolated_LongFormat_AllBraces_MaxBufferSize_Success()
        {
            // Test with a long interpolated string containing escaped braces (worst case for buffer).
            var longText = new string('x', 1000);
            var result = XString.Format($"{{{longText}}}");
            Assert.That(result, Is.EqualTo("{" + longText + "}"));
        }

        // --- Edge case tests: Thread-safety and nested StringBuilder ---

        // --- Edge case: null + format (generic path) ---

        [Test]
        public void Format_Interpolated_NullableInt_Null_WithFormatOnly()
        {
            int? n = null;
            var result = XString.Format($"{n:N2}");
            Assert.That(result, Is.EqualTo(""));
        }

        [Test]
        public void Format_Interpolated_NullableDouble_Null_WithFormatOnly()
        {
            double? d = null;
            var result = XString.Format($"{d:F3}");
            Assert.That(result, Is.EqualTo(""));
        }

        // --- Edge case: null string + alignment + format ---

        [Test]
        public void Format_Interpolated_NullString_WithAlignmentAndFormat()
        {
            string s = null;
            var result = XString.Format($"[{s,10:X}]");
            Assert.That(result, Is.EqualTo("[          ]"));
        }

        [Test]
        public void Format_Interpolated_NullString_LeftAlignmentAndFormat()
        {
            string s = null;
            var result = XString.Format($"[{s,-10:X}]");
            Assert.That(result, Is.EqualTo("[          ]"));
        }

        // --- Edge case: null + negative alignment (generic) ---

        [Test]
        public void Format_Interpolated_NullableInt_Null_LeftAlign()
        {
            int? n = null;
            var result = XString.Format($"[{n,-5}]");
            Assert.That(result, Is.EqualTo("[     ]"));
        }

        [Test]
        public void Format_Interpolated_NullString_LeftAlign()
        {
            string s = null;
            var result = XString.Format($"[{s,-5}]");
            Assert.That(result, Is.EqualTo("[     ]"));
        }

        [Test]
        public void Format_Interpolated_NullableInt_Null_LeftAlignWithFormat()
        {
            int? n = null;
            var result = XString.Format($"[{n,-10:N0}]");
            Assert.That(result, Is.EqualTo("[          ]"));
        }

        // --- Edge case: non-null string + format only ---

        [Test]
        public void Format_Interpolated_StringVar_WithFormatOnly()
        {
            // When a string has format, it is passed through the AppendFormattedViaComposite path.
            string s = "hello";
            var result = XString.Format($"{s,10}");
            Assert.That(result, Is.EqualTo("     hello"));
        }

        // --- Edge case: negative value + alignment ---

        [Test]
        public void Format_Interpolated_NegativeInt_RightAlign()
        {
            var result = XString.Format($"{-42,10}");
            Assert.That(result, Is.EqualTo(string.Format("{0,10}", -42)));
        }

        [Test]
        public void Format_Interpolated_NegativeInt_LeftAlign()
        {
            var result = XString.Format($"{-42,-10}");
            Assert.That(result, Is.EqualTo(string.Format("{0,-10}", -42)));
        }

        [Test]
        public void Format_Interpolated_NegativeDouble_RightAlign_WithFormat()
        {
            RunUnderCulture("en-US", () =>
            {
                var result = XString.Format($"{-1234.56,15:N2}");
                Assert.That(result, Is.EqualTo(string.Format("{0,15:N2}", -1234.56)));
            });
        }

        // --- Edge case: very large alignment ---

        [Test]
        public void Format_Interpolated_LargeAlignment_RightAlign()
        {
            var result = XString.Format($"{42,100}");
            Assert.That(result, Is.EqualTo(string.Format("{0,100}", 42)));
        }

        [Test]
        public void Format_Interpolated_LargeAlignment_LeftAlign()
        {
            var result = XString.Format($"{42,-100}");
            Assert.That(result, Is.EqualTo(string.Format("{0,-100}", 42)));
        }

        // --- Edge case: bool type ---

        [Test]
        public void Format_Interpolated_Bool_True()
        {
            var result = XString.Format($"{true}");
            Assert.That(result, Is.EqualTo("True"));
        }

        [Test]
        public void Format_Interpolated_Bool_False()
        {
            var result = XString.Format($"{false}");
            Assert.That(result, Is.EqualTo("False"));
        }

        [Test]
        public void Format_Interpolated_Bool_WithAlignment()
        {
            var result = XString.Format($"{true,10}");
            Assert.That(result, Is.EqualTo(string.Format("{0,10}", true)));
        }

        [Test]
        public void Format_Interpolated_NullableBool_Null()
        {
            bool? b = null;
            var result = XString.Format($"[{b,8}]");
            Assert.That(result, Is.EqualTo("[        ]"));
        }

        [Test]
        public void Format_CreateStringBuilder_Nested_NotNestedFalse_AllowsNesting()
        {
            // Default behavior (notNested=false): allows nested StringBuilder creation
            var sb1 = XString.CreateStringBuilder(notNested: false);
            sb1.Append("outer");

            var sb2 = XString.CreateStringBuilder(notNested: false);
            sb2.Append("inner");
            var innerResult = sb2.ToString();
            sb2.Dispose();

            sb1.Append("-" + innerResult);
            var result = sb1.ToString();
            sb1.Dispose();

            Assert.That(result, Is.EqualTo("outer-inner"));
        }

        [Test]
        public void Format_CreateStringBuilder_Nested_NotNestedTrue_ThreadStatic_Works()
        {
            // When notNested=true, uses thread-static buffer (reused across non-nested calls on same thread)
            // This test verifies that sequential (non-nested) calls work correctly
            var sb1 = XString.CreateStringBuilder(notNested: true);
            sb1.Append("first");
            var result1 = sb1.ToString();
            sb1.Dispose();

            // After dispose, buffer can be reused for another non-nested call
            var sb2 = XString.CreateStringBuilder(notNested: true);
            sb2.Append("second");
            var result2 = sb2.ToString();
            sb2.Dispose();

            Assert.That(result1, Is.EqualTo("first"));
            Assert.That(result2, Is.EqualTo("second"));
        }

        // --- Concurrency Tests ---

        [Test]
        public void Format_Concurrent_MultipleThreads_IsolatedResults()
        {
            // Verify that concurrent XString.Format calls on different threads produce correct results
            var results = new ConcurrentDictionary<int, string>();
            var threads = new Thread[4];

            for (int t = 0; t < 4; t++)
            {
                int threadId = t;
                threads[t] = new Thread(() =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var result = XString.Format($"Thread-{threadId}-Iteration-{i}");
                        results.TryAdd(threadId * 1000 + i, result);
                    }
                });
            }

            foreach (var thread in threads)
                thread.Start();

            foreach (var thread in threads)
                thread.Join();

            // Verify all results were generated and are correct
            Assert.That(results.Count, Is.EqualTo(400));
            for (int t = 0; t < 4; t++)
            {
                for (int i = 0; i < 100; i++)
                {
                    var expected = $"Thread-{t}-Iteration-{i}";
                    var key = t * 1000 + i;
                    Assert.That(results.ContainsKey(key), $"Missing result for thread {t}, iteration {i}");
                    var actual = results[key];
                    Assert.That(actual, Is.EqualTo(expected), $"Mismatch at thread {t}, iteration {i}");
                }
            }
        }

        [Test]
        public void Format_Concurrent_WithComplexFormatting_MultipleThreads()
        {
            // Test concurrent formatting with alignment, format specifiers, and culture-specific formatting
            var culture = CultureInfo.InvariantCulture;
            var results = new ConcurrentDictionary<int, string>();
            var threads = new Thread[3];

            for (int t = 0; t < 3; t++)
            {
                int threadId = t;
                threads[t] = new Thread(() =>
                {
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                    for (int i = 0; i < 50; i++)
                    {
                        var value = i * 10.5m;
                        var result = XString.Format($"T{threadId}: {value,15:F2}");
                        results.TryAdd(threadId * 1000 + i, result);
                    }
                });
            }

            foreach (var thread in threads)
                thread.Start();

            foreach (var thread in threads)
                thread.Join();

            Assert.That(results.Count, Is.EqualTo(150));

            for (int t = 0; t < 3; t++)
            {
                for (int i = 0; i < 50; i++)
                {
                    var value = i * 10.5m;
                    var expected = $"T{t}: {value.ToString("F2", culture),15}";
                    Assert.That(results[t * 1000 + i], Is.EqualTo(expected),
                        $"Mismatch at thread={t}, i={i}");
                }
            }
        }

        [Test]
        public void CreateStringBuilder_Concurrent_NotNestedTrue_ThreadSafeReuse()
        {
            // Verify thread-static buffer reuse is safe: each thread should get its own buffer
            var results = new ConcurrentDictionary<int, List<string>>();
            var threads = new Thread[3];

            for (int t = 0; t < 3; t++)
            {
                int threadId = t;
                threads[t] = new Thread(() =>
                {
                    var threadResults = new List<string>();
                    for (int i = 0; i < 10; i++)
                    {
                        var sb = XString.CreateStringBuilder(notNested: true);
                        sb.Append($"T{threadId}-value{i}");
                        var str = sb.ToString();
                        threadResults.Add(str);
                        sb.Dispose();
                    }
                    results.TryAdd(threadId, threadResults);
                });
            }

            foreach (var thread in threads)
                thread.Start();

            foreach (var thread in threads)
                thread.Join();

            // Each thread should have generated 10 distinct results
            Assert.That(results.Count, Is.EqualTo(3));
            for (int t = 0; t < 3; t++)
            {
                Assert.That(results.ContainsKey(t), $"Missing results for thread {t}");
                Assert.That(results[t].Count, Is.EqualTo(10));
                for (int i = 0; i < 10; i++)
                {
                    var expected = $"T{t}-value{i}";
                    Assert.That(results[t][i], Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void Format_Concurrent_Join_Concat_Operations()
        {
            // Test concurrent calls to Join and Concat methods
            var results = new ConcurrentDictionary<int, string>();
            var tasks = new Task[6];

            tasks[0] = Task.Run(() =>
            {
                var res = XString.Join(",", new[] { "a", "b", "c" });
                results.TryAdd(0, res);
            });

            tasks[1] = Task.Run(() =>
            {
                var res = XString.Join("|", new[] { 1, 2, 3 });
                results.TryAdd(1, res);
            });

            tasks[2] = Task.Run(() =>
            {
                var res = XString.Concat(new[] { "x", "y", "z" });
                results.TryAdd(2, res);
            });

            tasks[3] = Task.Run(() =>
            {
                var res = XString.Format($"Value: {42}");
                results.TryAdd(3, res);
            });

            tasks[4] = Task.Run(() =>
            {
                var res = XString.Join("-", new[] { 10.5, 20.3, 30.1 });
                results.TryAdd(4, res);
            });

            tasks[5] = Task.Run(() =>
            {
                var res = XString.Concat(new object[] { true, false, null });
                results.TryAdd(5, res);
            });

            Task.WaitAll(tasks);

            Assert.That(results.Count, Is.EqualTo(6));
            Assert.That(results[0], Is.EqualTo("a,b,c"));
            Assert.That(results[1], Is.EqualTo("1|2|3"));
            Assert.That(results[2], Is.EqualTo("xyz"));
            Assert.That(results[3], Is.EqualTo("Value: 42"));
            // results[4] and results[5] depend on culture/formatting, just verify they exist
            Assert.That(results[4], Is.Not.Null);
            Assert.That(results[5], Is.Not.Null);
        }

        // --- Enum interpolation tests ---

        private enum Color { Red, Green, Blue }

        [Flags]
        private enum Permission { None = 0, Read = 1, Write = 2, Execute = 4 }

        [Test]
        public void Format_Interpolated_Enum_EmbedsName()
        {
            var result = XString.Format($"{Color.Red}");
            Assert.That(result, Is.EqualTo("Red"));
        }

        [Test]
        public void Format_Interpolated_Enum_WithLiteral()
        {
            var result = XString.Format($"color={Color.Blue}");
            Assert.That(result, Is.EqualTo("color=Blue"));
        }

        [Test]
        public void Format_Interpolated_Enum_Variable()
        {
            Color c = Color.Green;
            var result = XString.Format($"selected={c}");
            Assert.That(result, Is.EqualTo("selected=Green"));
        }

        [Test]
        public void Format_Interpolated_Enum_WithAlignment()
        {
            var result = XString.Format($"{Color.Red,10}");
            Assert.That(result, Is.EqualTo(string.Format("{0,10}", Color.Red)));
        }

        [Test]
        public void Format_Interpolated_Enum_LeftAlign()
        {
            var result = XString.Format($"{Color.Red,-10}");
            Assert.That(result, Is.EqualTo(string.Format("{0,-10}", Color.Red)));
        }

        [Test]
        public void Format_Interpolated_Enum_WithFormat_D()
        {
            var result = XString.Format($"{Color.Blue:D}");
            Assert.That(result, Is.EqualTo("2"));
        }

        [Test]
        public void Format_Interpolated_Enum_WithFormat_X()
        {
            var result = XString.Format($"{Color.Blue:X}");
            Assert.That(result, Is.EqualTo(Color.Blue.ToString("X")));
        }

        [Test]
        public void Format_Interpolated_Enum_WithAlignmentAndFormat()
        {
            var result = XString.Format($"{Color.Green,10:D}");
            Assert.That(result, Is.EqualTo(string.Format("{0,10:D}", Color.Green)));
        }

        [Test]
        public void Format_Interpolated_Enum_Flags()
        {
            var perm = Permission.Read | Permission.Write;
            var result = XString.Format($"perm={perm}");
            Assert.That(result, Is.EqualTo($"perm={perm.ToString()}"));
        }

        [Test]
        public void Format_Interpolated_Enum_Flags_WithFormat_D()
        {
            var perm = Permission.Read | Permission.Execute;
            var result = XString.Format($"{perm:D}");
            Assert.That(result, Is.EqualTo(perm.ToString("D")));
        }

        [Test]
        public void Format_Interpolated_NullableEnum_Null()
        {
            Color? c = null;
            var result = XString.Format($"color={c}");
            Assert.That(result, Is.EqualTo("color="));
        }

        [Test]
        public void Format_Interpolated_NullableEnum_HasValue()
        {
            Color? c = Color.Green;
            var result = XString.Format($"color={c}");
            Assert.That(result, Is.EqualTo("color=Green"));
        }

        [Test]
        public void Format_Interpolated_NullableEnum_Null_WithAlignment()
        {
            Color? c = null;
            var result = XString.Format($"[{c,8}]");
            Assert.That(result, Is.EqualTo("[        ]"));
        }

        [Test]
        public void Format_Interpolated_Enum_Multiple()
        {
            var result = XString.Format($"{Color.Red},{Color.Green},{Color.Blue}");
            Assert.That(result, Is.EqualTo("Red,Green,Blue"));
        }

        // --- Direct handler null tests ---

        [Test]
        public void Handler_AppendLiteral_Null_AppendsNothing()
        {
            var handler = new _XS(0, 0);
            handler.AppendLiteral("a");
            handler.AppendLiteral(null);
            handler.AppendLiteral("b");
            var result = handler.GetString();
            handler.Dispose();
            Assert.That(result, Is.EqualTo("ab"));
        }

        [Test]
        public void Handler_AppendFormatted_String_Null_AppendsNothing()
        {
            var handler = new _XS(0, 0);
            handler.AppendLiteral("x");
            handler.AppendFormatted((string)null);
            handler.AppendLiteral("y");
            var result = handler.GetString();
            handler.Dispose();
            Assert.That(result, Is.EqualTo("xy"));
        }

        [Test]
        public void Handler_AppendFormatted_GenericString_Null_AppendsNothing()
        {
            var handler = new _XS(0, 0);
            handler.AppendFormatted<string>(null);
            var result = handler.GetString();
            handler.Dispose();
            Assert.That(result, Is.EqualTo(""));
        }

        [Test]
        public void Handler_AppendFormatted_GenericObject_Null_AppendsNothing()
        {
            var handler = new _XS(0, 0);
            handler.AppendLiteral("[");
            handler.AppendFormatted<object>(null);
            handler.AppendLiteral("]");
            var result = handler.GetString();
            handler.Dispose();
            Assert.That(result, Is.EqualTo("[]"));
        }

        [Test]
        public void Handler_AppendFormatted_NullableInt_Null_AppendsNothing()
        {
            var handler = new _XS(0, 0);
            handler.AppendFormatted<int?>(null);
            var result = handler.GetString();
            handler.Dispose();
            Assert.That(result, Is.EqualTo(""));
        }

        [Test]
        public void Handler_AppendFormatted_GenericNull_WithFormat_AppendsNothing()
        {
            var handler = new _XS(0, 0);
            handler.AppendFormatted<double?>(null, "F2");
            var result = handler.GetString();
            handler.Dispose();
            Assert.That(result, Is.EqualTo(""));
        }

        [Test]
        public void Handler_AppendFormatted_GenericNull_WithAlignment_AppendsPadding()
        {
            var handler = new _XS(0, 0);
            handler.AppendFormatted<string>(null, 5);
            var result = handler.GetString();
            handler.Dispose();
            Assert.That(result, Is.EqualTo("     "));
        }

        [Test]
        public void Handler_AppendFormatted_GenericNull_WithAlignmentAndFormat_AppendsPadding()
        {
            var handler = new _XS(0, 0);
            handler.AppendFormatted<int?>(null, 8, "N0");
            var result = handler.GetString();
            handler.Dispose();
            Assert.That(result, Is.EqualTo("        "));
        }

        [Test]
        public void Handler_AppendFormatted_StringNull_WithAlignmentAndFormat_AppendsPadding()
        {
            var handler = new _XS(0, 0);
            handler.AppendFormatted((string)null, 6, "X");
            var result = handler.GetString();
            handler.Dispose();
            Assert.That(result, Is.EqualTo("      "));
        }

        // --- SetTextX tests ---

        [Test]
        public void SetTextX_Interpolated_SetsTextWithoutAllocation()
        {
            var go = new GameObject("TMP_Test");
            var tmp = go.AddComponent<TextMeshPro>();
            try
            {
                tmp.SetTextX($"HP: {100}");
                Assert.That(tmp.text, Is.EqualTo("HP: 100"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void SetTextX_Interpolated_WithFormat()
        {
            var go = new GameObject("TMP_Test");
            var tmp = go.AddComponent<TextMeshPro>();
            try
            {
                tmp.SetTextX($"Score: {1234.5:F1}");
                Assert.That(tmp.text, Is.EqualTo($"Score: {1234.5:F1}"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void SetTextX_Interpolated_MultipleValues()
        {
            var go = new GameObject("TMP_Test");
            var tmp = go.AddComponent<TextMeshPro>();
            try
            {
                tmp.SetTextX($"Lv.{10} ATK:{250} DEF:{180}");
                Assert.That(tmp.text, Is.EqualTo("Lv.10 ATK:250 DEF:180"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void SetTextX_Interpolated_EmptyString()
        {
            var go = new GameObject("TMP_Test");
            var tmp = go.AddComponent<TextMeshPro>();
            try
            {
                tmp.SetTextX($"");
                Assert.That(tmp.text, Is.EqualTo(""));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void SetTextX_NullLabel_DoesNotThrow()
        {
            TMP_Text label = null;
            Assert.DoesNotThrow(() => label.SetTextX($"test {42}"));
        }
    }
}
