// Run performance tests: Window > General > Test Runner (PlayMode).

using System;
using System.Globalization;
using System.Text;
using Cysharp.Text;
using NUnit.Framework;
using TMPro;
using Unity.PerformanceTesting;
using UnityEngine;
using xpTURN.Text;

namespace xpTURN.Text.Tests
{
    [TestFixture]
    public static class XStringBenchmarks
    {
        public enum ActionType
        {
            CreateRoom,
            JoinRoom,
            Ready,
            Playing,
        }

        const int WarmupCount = 5;
        const int MeasurementCount = 20;
        const int IterationsPerMeasurement = 100000;

        // Single test pattern: string + number (format) + date (format) + number (percent)
        const string Label = "Player_Alpha";
        const double Score = 12345.6789;
        static readonly DateTime At = new DateTime(2026, 2, 24, 14, 30, 0);
        const double Rate = 0.4567;

        const ActionType Type = ActionType.Playing;

        const string FormatTemplate = "User: {0} | Score: {1:N2} | At: {2:yyyy-MM-dd HH:mm} | Rate: {3:P1} | ActionType: {4}";
        const string EnumFormatTemplate = "Action: {0:D} | Hex: {0:X}";

        [SetUp]
        public static void SetCulture()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        }

        [Test, Performance]
        public static void XStringIFormat_FiveArgs()
        {
            Measure.Method(() =>
            {
                // One heap allocation (returned string)
                _ = XString.Format($"User: {Label} | Score: {Score:N2} | At: {At:yyyy-MM-dd HH:mm} | Rate: {Rate:P1} | ActionType: {Type}");
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void ZStringFormat_FiveArgs()
        {
            Measure.Method(() =>
            {
                // One heap allocation (returned string)
                _ = ZString.Format(FormatTemplate, Label, Score, At, Rate, Type);
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void StringFormat_FiveArgs()
        {
            Measure.Method(() =>
            {
                // Multiple temporary allocations inside
                _ = string.Format(FormatTemplate, Label, Score, At, Rate, Type);
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void StringIFormat_FiveArgs()
        {
            Measure.Method(() =>
            {
                // Multiple temporary allocations inside
                _ = $"User: {Label} | Score: {Score:N2} | At: {At:yyyy-MM-dd HH:mm} | Rate: {Rate:P1} | ActionType: {Type}";
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void StringBuilder_FiveArgs()
        {
            Measure.Method(() =>
            {
                var sb = new StringBuilder();
                sb.Append("User: ");
                sb.Append(Label);
                sb.Append(" | Score: ");
                sb.AppendFormat("{0:N2}", Score);
                sb.Append(" | At: ");
                sb.AppendFormat("{0:yyyy-MM-dd HH:mm}", At);
                sb.Append(" | Rate: ");
                sb.AppendFormat("{0:P1}", Rate);
                sb.Append(" | ActionType: ");
                sb.Append(Type);
                _ = sb.ToString();
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void StringBuilder128_FiveArgs()
        {
            Measure.Method(() =>
            {
                var sb = new StringBuilder(128);
                sb.Append("User: ");
                sb.Append(Label);
                sb.Append(" | Score: ");
                sb.AppendFormat("{0:N2}", Score);
                sb.Append(" | At: ");
                sb.AppendFormat("{0:yyyy-MM-dd HH:mm}", At);
                sb.Append(" | Rate: ");
                sb.AppendFormat("{0:P1}", Rate);
                sb.Append(" | ActionType: ");
                sb.Append(Type);
                _ = sb.ToString();
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void StringBuilderShare_FiveArgs()
        {
            // Reuse the same StringBuilder instance across iterations (created once)
            var sb = new StringBuilder(128);
            Measure.Method(() =>
            {
                sb.Clear();
                sb.Append("User: ");
                sb.Append(Label);
                sb.Append(" | Score: ");
                sb.AppendFormat("{0:N2}", Score);
                sb.Append(" | At: ");
                sb.AppendFormat("{0:yyyy-MM-dd HH:mm}", At);
                sb.Append(" | Rate: ");
                sb.AppendFormat("{0:P1}", Rate);
                sb.Append(" | ActionType: ");
                sb.Append(Type);
                _ = sb.ToString();
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void Utf16ValueStringBuilder_FiveArgs()
        {
            Measure.Method(() =>
            {
                // Utf16ValueStringBuilder (buffer reused); final ToString (returned string) => one heap allocation
                using var sb = ZString.CreateStringBuilder();
                sb.Append("User: ");
                sb.Append(Label);
                sb.Append(" | Score: ");
                sb.AppendFormat("{0:N2}", Score);
                sb.Append(" | At: ");
                sb.AppendFormat("{0:yyyy-MM-dd HH:mm}", At);
                sb.Append(" | Rate: ");
                sb.AppendFormat("{0:P1}", Rate);
                sb.Append(" | ActionType: ");
                sb.Append(Type);
                _ = sb.ToString();
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void XStringIFormat_EnumDX()
        {
            Measure.Method(() =>
            {
                _ = XString.Format($"Action: {Type:D} | Hex: {Type:X}");
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void ZStringFormat_EnumDX()
        {
            Measure.Method(() =>
            {
                _ = ZString.Format(EnumFormatTemplate, Type);
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void StringFormat_EnumDX()
        {
            Measure.Method(() =>
            {
                _ = string.Format(EnumFormatTemplate, Type);
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void StringIFormat_EnumDX()
        {
            Measure.Method(() =>
            {
                _ = $"Action: {Type:D} | Hex: {Type:X}";
            })
                .WarmupCount(WarmupCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .MeasurementCount(MeasurementCount)
                .GC()
                .Run();
        }

        [Test, Performance]
        public static void SetTextX_FiveArgs()
        {
            var go = new GameObject("TMP_Bench");
            var tmp = go.AddComponent<TextMeshPro>();
            try
            {
                Measure.Method(() =>
                {
                    // Zero heap allocation: char[] buffer passed directly via SetCharArray
                    tmp.SetTextX($"User: {Label} | Score: {Score:N2} | At: {At:yyyy-MM-dd HH:mm} | Rate: {Rate:P1} | ActionType: {Type}");
                })
                    .WarmupCount(WarmupCount)
                    .IterationsPerMeasurement(IterationsPerMeasurement)
                    .MeasurementCount(MeasurementCount)
                    .GC()
                    .Run();
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }

        [Test, Performance]
        public static void TMP_TextAssign_FiveArgs()
        {
            var go = new GameObject("TMP_Bench");
            var tmp = go.AddComponent<TextMeshPro>();
            try
            {
                Measure.Method(() =>
                {
                    // Standard interpolation: allocates string then assigns to TMP
                    tmp.text = $"User: {Label} | Score: {Score:N2} | At: {At:yyyy-MM-dd HH:mm} | Rate: {Rate:P1} | ActionType: {Type}";
                })
                    .WarmupCount(WarmupCount)
                    .IterationsPerMeasurement(IterationsPerMeasurement)
                    .MeasurementCount(MeasurementCount)
                    .GC()
                    .Run();
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }
    }
}
