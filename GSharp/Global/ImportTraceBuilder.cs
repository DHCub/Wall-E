using System.Collections.Generic;

namespace GSharp;

public static class ImportTraceBuilder
{
    public static List<char> GetImportTrace(Stack<string> files)
    {
        var answ = new List<char>();
        if (files != null && files.Count > 0)
        {
            bool first = true;
            foreach(var file in files)
            {
                if (first)
                {
                answ.AddRange("\nAt file: \n");
                answ.AddRange(file);
                first = false;
                }
                else
                {
                answ.AddRange(", Imported from File:\n");
                answ.AddRange(file); 
                }
            }
        }

        return answ;
    }

    public static string GetImportTraceString(Stack<string> files) 
        => new string(GetImportTrace(files).ToArray());
}