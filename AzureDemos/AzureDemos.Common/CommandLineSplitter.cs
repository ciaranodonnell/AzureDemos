using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AzureDemos.Common
{

    public static class CommandLineSplitter
    {

        public static string[] SplitCommandLineStyle(this string value, char delimeter = ' ', bool ignoreEmptyValues = false)
        {
            List<String> result = new List<String>();

            using (TextReader rdr = new StringReader(value))
            {

                bool IsInQuotes = false;
                StringBuilder sb = new StringBuilder();

                Int32 nc; Char c;
                while ((nc = rdr.Read()) != -1)
                {
                    c = (Char)nc;

                    if (!IsInQuotes)
                    {

                        if (c == '"')
                        {
                            IsInQuotes = true;
                        }
                        else if (c == delimeter)
                        {
                            if (sb.Length > 0 || !ignoreEmptyValues)
                                result.Add(sb.ToString());
                            sb.Length = 0;
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                    else
                    {

                        if (c == '"')
                        {
                            IsInQuotes = false;
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                } 
                if (sb.Length > 0 || !ignoreEmptyValues) result.Add(sb.ToString());
            }
            return result.ToArray(); 
        }
        

    }
}
