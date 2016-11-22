namespace SoatChallenge
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>Format string to the appropriate culture</summary>
    public static class Write
    {
        /// <summary>flatten a collection to string</summary>
        /// <param name="inputs">collection to flat</param>
        /// <param name="separator">string.Join separator</param>
        /// <returns>collection flatten with string.join</returns>
        public static string Collection(IEnumerable<object> inputs, string separator)
        {
            if (inputs == null)
            {
                return "null";
            }
            else if (!inputs.Any())
            {
                return "empty";
            }
            else
            {
                return string.Join(separator, inputs);
            }
        }

        /// <summary>flatten a inputs to string</summary>
        /// <param name="inputs">inputs to flat</param>
        /// <returns>inputs flatten with string.join</returns>
        public static string Collection(IEnumerable<object> inputs)
        {
            return Write.Collection(inputs, ";");
        }

        /// <summary>Format string to invariant culture</summary>
        /// <param name="formattable">initial string as formattable</param>
        /// <returns>string formated</returns>
        public static string Current(FormattableString formattable)
        {
            return formattable?.ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>Format string to invariant culture</summary>
        /// <param name="formattable">initial string as formattable</param>
        /// <returns>string formated</returns>
        public static string Invariant(FormattableString formattable)
        {
            return formattable?.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>send string to console output, using invariant culture</summary>
        /// <param name="formattable">initial string as formattable</param>
        public static void Print(FormattableString formattable)
        {
            Console.WriteLine(Write.Invariant(formattable));

            Write.TraceVisible(formattable);
        }

        /// <summary>send message and collection flatten to string to debug output, using invariant culture</summary>
        /// <param name="message">message to print</param>
        /// <param name="collection">collection to print after message</param>
        /// <param name="separator">string.Join separator</param>
        public static void Trace(FormattableString message, IEnumerable<object> collection, string separator)
        {
            System.Diagnostics.Trace.WriteLine(Write.Invariant($"{message?.ToString()}{Write.Collection(collection, separator)}"));
        }

        /// <summary>send message and collection flatten to string to debug output, using invariant culture</summary>
        /// <param name="message">message to print</param>
        /// <param name="collection">collection to print after message</param>
        public static void Trace(FormattableString message, IEnumerable<object> collection)
        {
            Trace(message, collection, ";");
        }

        /// <summary>send string to debug output, using invariant culture</summary>
        /// <param name="formattable">initial string as formattable</param>
        public static void Trace(FormattableString formattable)
        {
            System.Diagnostics.Trace.WriteLine(Write.Invariant(formattable));
        }

        /// <summary>send string to debug output, using invariant culture</summary>
        /// <param name="message">initial string</param>
        public static void Trace(string message)
        {
            System.Diagnostics.Trace.WriteLine(message.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>send string to debug output, using invariant culture</summary>
        /// <param name="formattable">initial string as formattable</param>
        public static void TraceInline(FormattableString formattable)
        {
            System.Diagnostics.Trace.Write(Write.Invariant(formattable));
        }

        /// <summary>send string to debug output, using invariant culture</summary>
        /// <param name="formattable">initial string as formattable</param>
        public static void TraceVisible(FormattableString formattable)
        {
            System.Diagnostics.Trace.Write(Write.Invariant($"{Environment.NewLine}!! {formattable.ToString()}{Environment.NewLine}"));
        }
    }
}