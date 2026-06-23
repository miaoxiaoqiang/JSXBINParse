using System;

namespace Parser
{
    public enum JsxbinVersion : ushort
    {
        Invalid = 0xFFFF,
        v10     = 0x0100,
        v20     = 0x0200,
        v21     = 0x0201,
    }

    //public static class JsxerDecompiler
    //{
    //    private static readonly Jsxbeautifier.Beautifier beautifier;

    //    static JsxerDecompiler()
    //    {
    //        beautifier = new Jsxbeautifier.Beautifier();
    //    }

    //    public static Tuple<string, string> Decompile(string input, bool unblind = false, bool beautiful = false, bool printStructure = false)
    //    {
    //        Reader reader = new Reader(input, unblind);
    //        //reader.ClearTreeStructure();

    //        if (!reader.VerifySignature())
    //        {
    //            throw new Exception("[!]: The input file has an invalid signature.\r\nJSXBIN signature verification failed!");
    //        }

    //        Nodes.Program ast = new Nodes.Program(reader)
    //        {
    //            PrintStructure = printStructure
    //        };
    //        ast.Parse();

    //        string code = ast.GetString();
    //        string output = PrependHeader(code, reader.Version, unblind);

    //        if(beautiful)
    //        {
    //            output = beautifier.Beautify(output);
    //        }

    //        return Tuple.Create(output, printStructure ? reader.GetTreeStructure() : string.Empty);
    //    }

    //    private static string PrependHeader(string code, JsxbinVersion version, bool unblind)
    //    {
    //        string versionStr = version switch
    //        {
    //            JsxbinVersion.v10 => "1.0",
    //            JsxbinVersion.v20 => "2.0",
    //            JsxbinVersion.v21 => "2.1",
    //            _ => "VERSION UNKNOWN"
    //        };

    //        string header = "/*\n"
    //                      + "* Decompiled with JsxBinParse\n"
    //                      + $"* Time: {DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss")}\n"
    //                      + "* Version: 1.0.0.0\n"
    //                      + "* JSXBIN " + versionStr + "\n";

    //        if (unblind)
    //        {
    //            header += "* Jsxblind Deobfuscation Enabled (EXPERIMENTAL)\n";
    //        }

    //        header += "*/\n\n";

    //        return header + code;
    //    }
    //}
}