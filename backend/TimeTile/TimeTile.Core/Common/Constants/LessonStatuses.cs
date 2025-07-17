using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TimeTile.Core.Common.Constants
{
    public static class LessonStatuses
    {
        public const string Scheduled = "Scheduled";
        public const string Cancelled = "Cancelled";
        public const string Exam = "Exam";
        public const string Event = "Event";

        private static readonly Lazy<IReadOnlyList<string>> _allLessonStatuses = new(() =>
            typeof(LessonStatuses)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(fi => fi.GetRawConstantValue()?.ToString())
                .Where(value => !string.IsNullOrEmpty(value))
                .ToList()!);

        public static IReadOnlyList<string> All => _allLessonStatuses.Value;
        public static int Count => All.Count;
    }
}
