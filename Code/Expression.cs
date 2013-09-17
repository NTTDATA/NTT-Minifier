using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NTT.Minifier
{
    public class Expression
    {
        public Expression(Regex expr, string replaceWith = "")
        { RegularExpression = expr; ReplaceWith = replaceWith; }

        public Regex RegularExpression { get; private set; }
        public string ReplaceWith { get; private set; }
    }
}
