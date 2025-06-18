using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Tests.Utils
{
    public class Capturer
    {
        private readonly object _lock = new();
        private readonly List<string> _capturerValues = new ();

        public void Grab(object? value)
        {
            lock (_lock)
            {
                _capturerValues.Add(value?.ToString() ?? "null");
            }
        }

        public void Grab(string value)
        {
            lock (_lock)
            {
                _capturerValues.Add(value);
            }
        }

        public void Grab(string name, object? value)
        {
            Grab($"{name} : {value?.ToString() ?? "null"}");
        }

        public void Grab(string name, string value)
        {
            Grab($"{name} : {value}");
        }
        
        public string GetCaptured(string joinStr = "\r\n")
        {
            lock (_lock)
            {
                return string.Join(joinStr, _capturerValues);
            }
        }

        internal void AssertIsEqual(string expected)
        {
            string actual = GetCaptured().Trim();
            Assert.AreEqual(expected.Trim(), actual, $"Captured output does not match expected value.\n\nExpected :\n{expected}\n\nActual :\n{actual}");
        }
    }
}
