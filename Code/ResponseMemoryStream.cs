using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;

namespace NTT.Minifier
{
    public class ResponseMemoryStream : MemoryStream
    {
        private Stream outputStream;
        private List<Expression> expressionList = new List<Expression>();

        public ResponseMemoryStream(Stream stream)
        {
            outputStream = stream;

            //tabs
            expressionList.Add(new Expression(new Regex("\t", RegexOptions.Compiled | RegexOptions.Multiline)));

            //space before tag
            expressionList.Add(new Expression(new Regex(@"\s+<", RegexOptions.Multiline | RegexOptions.Compiled), " <"));
            //space after tag
            expressionList.Add(new Expression(new Regex(@">\s+", RegexOptions.Compiled | RegexOptions.Multiline), "> "));

            //new line character
            //the new line replace causes certain browsers cause issues with asp.net client side form submit
            ////expressionList.Add(new Expression(new Regex(@"\r\n\s*|\n\s*", RegexOptions.Multiline | RegexOptions.Compiled)));

            //multiple spaces
            expressionList.Add(new Expression(new Regex(@"^\s+", RegexOptions.Multiline | RegexOptions.Compiled)));

            //comment tags excep IE if statements
            expressionList.Add(new Expression(new Regex(@"<!--(?!\[).*?(?!<\])-->", RegexOptions.Singleline | RegexOptions.Compiled), string.Empty));
            //<!--(?!\[if).*?-->
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            //convert byte array to string
            string bufferString = Encoding.Default.GetString(buffer);

            //loop all the expressions 
            foreach (Expression expr in expressionList)
            {
                bufferString = expr.RegularExpression.Replace(bufferString, expr.ReplaceWith);
            }

            //write the content with converting in bytes
            outputStream.Write(Encoding.Default.GetBytes(bufferString), offset, Encoding.Default.GetByteCount(bufferString));
        }
    }
}
