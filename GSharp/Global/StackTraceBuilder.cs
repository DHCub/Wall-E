using System.Collections.Generic;

namespace GSharp;

public static class StackTraceBuilder
{
    public static List<char> GetStackTrace(Stack<string> stackNames)
    {
        var answ = new List<char>();
        if (stackNames != null && stackNames.Count > 0)
        {
            bool first = true;
            int i = 0;
            foreach(var stack in stackNames)
            {
                if (++i == 10) break;
                if (first)
                {
                    answ.AddRange("\nMost Recent calls: \n");
                    answ.AddRange(stack);
                    first = false;
                }
                else
                {
                    answ.AddRange("\n");
                    answ.AddRange(stack); 
                }
            }
        }

        return answ;
    }

    public static string GetStackTraceString(Stack<string> files) 
        => new string(GetStackTrace(files).ToArray());
}