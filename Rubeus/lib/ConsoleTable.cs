using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

// Copyright (c) 2013 Khalid Abuhakmeh, The MIT License (MIT)
// Source: https://github.com/khalidabuhakmeh/ConsoleTables

// Changes:
//      Restricted values to strings
//      Removed divider from between each row
//      Removed additional formatting logic

namespace ConsoleTables
{
    public class ConsoleTable
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

        public IList<string> Columns { get; set; }
        public IList<string[]> Rows { get; protected set; }

        public ConsoleTableOptions Options { get; protected set; }

        public ConsoleTable(params string[] columns)
            :this(new ConsoleTableOptions { Columns = new List<string>(columns) })
        {          
        }

        public ConsoleTable(ConsoleTableOptions options)
        {
            Options = options;
            Rows = new List<string[]>();
            Columns = new List<string>(options.Columns);
        }

        public ConsoleTable AddColumn(IEnumerable<string> names)
        {
            foreach (var name in names)
                Columns.Add(name);
            return this;
        }

        public ConsoleTable AddRow(params string[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (!Columns.Any())
                throw new Exception(S(new byte[] { 80, 108, 101, 97, 115, 101, 32, 115, 101, 116, 32, 116, 104, 101, 32, 99, 111, 108, 117, 109, 110, 115, 32, 102, 105, 114, 115, 116 }));

            if (Columns.Count != values.Length)
                throw new Exception(
                    S(new byte[] { 84, 104, 101, 32, 110, 117, 109, 98, 101, 114, 32, 99, 111, 108, 117, 109, 110, 115, 32, 105, 110, 32, 116, 104, 101, 32, 114, 111, 119, 32, 40 }) + Columns.Count + S(new byte[] { 41, 32, 100, 111, 101, 115, 32, 110, 111, 116, 32, 109, 97, 116, 99, 104, 32, 116, 104, 101, 32, 118, 97, 108, 117, 101, 115, 32, 40 }) + values.Length + S(new byte[] { 41 }));

            Rows.Add(values);
            return this;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            // find the longest column by searching each row
            var columnLengths = ColumnLengths();

            // create the string format with padding
            var format = Enumerable.Range(0, Columns.Count)
                .Select(i => S(new byte[] { 32, 124, 32, 123 }) + i + S(new byte[] { 44, 45 }) + columnLengths[i] + S(new byte[] { 125 }) )
                .Aggregate((s, a) => s + a) + S(new byte[] { 32, 124 });

            // find the longest formatted line
            var maxRowLength = Math.Max(0, Rows.Any() ? Rows.Max(row => string.Format(format, row).Length) : 0);
            var columnHeaders = string.Format(format, Columns.ToArray());

            // longest line is greater of formatted columnHeader and longest row
            var longestLine = Math.Max(maxRowLength, columnHeaders.Length);

            // add each row
            var results = Rows.Select(row => string.Format(format, row)).ToList();

            // create the divider
            var divider = String.Format(S(new byte[] { 32, 123, 48, 125, 32 }), new String('-', longestLine - 1));

            builder.AppendLine(divider);
            builder.AppendLine(columnHeaders);
            builder.AppendLine(divider);

            foreach (var row in results)
            {
                builder.AppendLine(row);
            }

            builder.AppendLine(divider);

            return builder.ToString();
        }

        private string Format(List<int> columnLengths, char delimiter = '|')
        {
            var delimiterStr = delimiter == char.MinValue ? string.Empty : delimiter.ToString();
            var format = (Enumerable.Range(0, Columns.Count)
                .Select(i => S(new byte[] { 32 }) + delimiterStr + S(new byte[] { 32, 123 }) + i + S(new byte[] { 44, 45 }) + columnLengths[i] + S(new byte[] { 125 }) )
                .Aggregate((s, a) => s + a) + S(new byte[] { 32 }) + delimiterStr).Trim();
            return format;
        }

        private List<int> ColumnLengths()
        {
            var columnLengths = Columns
                .Select((t, i) => Rows.Select(x => x[i])
                    .Union(new[] { Columns[i] })
                    .Where(x => x != null)
                    .Select(x => x.ToString().Length).Max())
                .ToList();
            return columnLengths;
        }

        public void Write(Format format = ConsoleTables.Format.Default)
        {
            switch (format)
            {
                case ConsoleTables.Format.Default:
                    Console.WriteLine(ToString());
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        private static IEnumerable<string> GetColumns<T>()
        {  
            return typeof(T).GetProperties().Select(x => x.Name).ToArray();
        }

        private static object GetColumnValue<T>(object target, string column)
        {
            return typeof(T).GetProperty(column).GetValue(target, null);
        }
    }

    public class ConsoleTableOptions
    {
        private static string S(byte[] b) => System.Text.Encoding.UTF8.GetString(b);

        public IEnumerable<string> Columns { get; set; } = new List<string>();
        public bool EnableCount { get; set; } = true;
    }

    public enum Format
    {
        Default = 0,
        MarkDown = 1,
        Alternative = 2,
        Minimal = 3
    }
}
