using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Jsxbeautifier
{
    internal sealed class Beautifier
    {
        public BeautifierOptions Opts { get; set; }

        public BeautifierFlags Flags { get; set; }

        private List<BeautifierFlags> FlagStore { get; set; }

        private bool WantedNewline { get; set; }

        private bool JustAddedNewline { get; set; }

        private bool DoBlockJustClosed { get; set; }

        private string IndentString { get; set; }

        private string PreindentString { get; set; }

        private string LastWord { get; set; }

        private string LastType { get; set; }

        private string LastText { get; set; }

        private string LastLastText { get; set; }

        private string Input { get; set; }

        private List<string> Output { get; set; }

        private char[] Whitespace { get; set; }

        private string Wordchar { get; set; }

        private string Digits { get; set; }

        private string[] Punct { get; set; }

        private string[] LineStarters { get; set; }

        private int ParserPos { get; set; }

        private int NNewlines { get; set; }

        public Beautifier() : this(new BeautifierOptions())
        {

        }

        public Beautifier(BeautifierOptions opts)
        {
            Opts = opts;
            BlankState();
        }

        private void BlankState()
        {
            Flags = new BeautifierFlags("BLOCK");
            FlagStore = new List<BeautifierFlags>();
            WantedNewline = false;
            JustAddedNewline = false;
            DoBlockJustClosed = false;
            if (Opts.IndentWithTabs)
            {
                IndentString = "\t";
            }
            else
            {
                IndentString = new string(Opts.IndentChar, (int)Opts.IndentSize);
            }
            PreindentString = "";
            LastWord = "";
            LastType = "TK_START_EXPR";
            LastText = "";
            LastLastText = "";
            Input = null;
            Output = new List<string>();
            Whitespace = new char[4] { '\n', '\r', '\t', ' ' };
            Wordchar = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_$";
            Digits = "0123456789";
            Punct = "+ - * / % & ++ -- = += -= *= /= %= == === != !== > < >= <= >> << >>> >>>= >>= <<= && &= | || ! !! , : ? ^ ^= |= :: <?= <? ?> <%= <% %>".Split(' ');
            LineStarters = "continue,try,throw,return,var,if,switch,case,default,for,while,break,function".Split(',');
            SetMode("BLOCK");
            ParserPos = 0;
        }

        private void SetMode(string mode)
        {
            BeautifierFlags beautifierFlags = new BeautifierFlags("BLOCK");
            if (Flags != null)
            {
                FlagStore.Add(Flags);
                beautifierFlags = Flags;
            }
            Flags = new BeautifierFlags(mode);
            if (FlagStore.Count == 1)
            {
                Flags.IndentationLevel = 0;
            }
            else
            {
                Flags.IndentationLevel = beautifierFlags.IndentationLevel;
                if (beautifierFlags.VarLine && beautifierFlags.VarLineReindented)
                {
                    Flags.IndentationLevel += 1;
                }
            }
            Flags.PreviousMode = beautifierFlags.Mode;
        }

        public string Beautify(string s, BeautifierOptions opts = null)
        {
            if (opts != null)
            {
                Opts = opts;
            }
            BlankState();
            while (s.Length != 0 && (s[0] == ' ' || s[0] == '\t'))
            {
                PreindentString += s[0];
                s = s.Remove(0, 1);
            }
            Input = s;
            ParserPos = 0;
            while (true)
            {
                Tuple<string, string> nextToken = GetNextToken();
                string item = nextToken.Item1;
                string item2 = nextToken.Item2;
                if (item2 == "TK_EOF")
                {
                    break;
                }
                Dictionary<string, Action<string>> dictionary = new Dictionary<string, Action<string>>();
                dictionary.Add("TK_START_EXPR", HandleStartExpr);
                dictionary.Add("TK_END_EXPR", HandleEndExpr);
                dictionary.Add("TK_START_BLOCK", HandleStartBlock);
                dictionary.Add("TK_END_BLOCK", HandleEndBlock);
                dictionary.Add("TK_WORD", HandleWord);
                dictionary.Add("TK_SEMICOLON", HandleSemicolon);
                dictionary.Add("TK_STRING", HandleString);
                dictionary.Add("TK_EQUALS", HandleEquals);
                dictionary.Add("TK_OPERATOR", HandleOperator);
                dictionary.Add("TK_COMMA", HandleComma);
                dictionary.Add("TK_BLOCK_COMMENT", HandleBlockComment);
                dictionary.Add("TK_INLINE_COMMENT", HandleInlineComment);
                dictionary.Add("TK_COMMENT", HandleComment);
                dictionary.Add("TK_DOT", HandleDot);
                dictionary.Add("TK_UNKNOWN", HandleUnknown);
                dictionary[item2](item);
                LastLastText = LastText;
                LastType = item2;
                LastText = item;
            }
            Regex regex = new Regex("[\\n ]+$");
            return PreindentString + regex.Replace(string.Concat(Output), "", 1);
        }

        private void TrimOutput(bool eatNewlines = false)
        {
            while (Output.Count != 0 && (Output[Output.Count - 1] == " " || Output[Output.Count - 1] == IndentString || Output[Output.Count - 1] == PreindentString || (eatNewlines && (Output[Output.Count - 1] == "\n" || Output[Output.Count - 1] == "\r"))))
            {
                Output.RemoveAt(Output.Count - 1);
            }
        }

        private bool IsSpecialWord(string s)
        {
            switch (s)
            {
                default:
                    return s == "else";
                case "case":
                case "return":
                case "do":
                case "if":
                case "throw":
                    return true;
            }
        }

        private bool IsArray(string mode)
        {
            if (!(mode == "[EXPRESSION]"))
            {
                return mode == "[INDENTED-EXPRESSION]";
            }
            return true;
        }

        private bool IsExpression(string mode)
        {
            switch (mode)
            {
                default:
                    return mode == "(COND-EXPRESSION)";
                case "[EXPRESSION]":
                case "[INDENTED-EXPRESSION]":
                case "(EXPRESSION)":
                case "(FOR-EXPRESSION)":
                    return true;
            }
        }

        private void AppendNewlineForced()
        {
            bool keepArrayIndentation = Opts.KeepArrayIndentation;
            Opts.KeepArrayIndentation = false;
            AppendNewline();
            Opts.KeepArrayIndentation = keepArrayIndentation;
        }

        private void AppendNewline(bool ignoreRepeated = true, bool resetStatementFlags = true)
        {
            Flags.EatNextSpace = false;
            if (Opts.KeepArrayIndentation && IsArray(Flags.Mode))
            {
                return;
            }
            if (resetStatementFlags)
            {
                Flags.IfLine = false;
                Flags.ChainExtraIndentation = 0;
            }
            TrimOutput();
            if (Output.Count == 0)
            {
                return;
            }
            if (Output[Output.Count - 1] != "\n" || !ignoreRepeated)
            {
                JustAddedNewline = true;
                Output.Add("\n");
            }
            if (PreindentString != null && PreindentString.Length != 0)
            {
                Output.Add(PreindentString);
            }
            foreach (int item in Enumerable.Range(0, Flags.IndentationLevel + Flags.ChainExtraIndentation))
            {
                _ = item;
                Output.Add(IndentString);
            }
            if (Flags.VarLine && Flags.VarLineReindented)
            {
                Output.Add(IndentString);
            }
        }

        private void Append(string s)
        {
            if (s == " ")
            {
                if (LastType == "TK_COMMENT")
                {
                    AppendNewline();
                }
                else if (Flags.EatNextSpace)
                {
                    Flags.EatNextSpace = false;
                }
                else if (Output.Count != 0 && Output[Output.Count - 1] != " " && Output[Output.Count - 1] != "\n" && Output[Output.Count - 1] != IndentString)
                {
                    Output.Add(" ");
                }
            }
            else
            {
                JustAddedNewline = false;
                Flags.EatNextSpace = false;
                Output.Add(s);
            }
        }

        private void Indent()
        {
            Flags.IndentationLevel += 1;
        }

        private void RemoveIndent()
        {
            if (Output.Count != 0 && (Output[Output.Count - 1] == IndentString || Output[Output.Count - 1] == PreindentString))
            {
                Output.RemoveAt(Output.Count - 1);
            }
        }

        private void RestoreMode()
        {
            DoBlockJustClosed = Flags.Mode == "DO_BLOCK";
            if (FlagStore.Count > 0)
            {
                string mode = Flags.Mode;
                Flags = FlagStore[FlagStore.Count - 1];
                FlagStore.RemoveAt(FlagStore.Count - 1);
                Flags.PreviousMode = mode;
            }
        }

        private Tuple<string, string> GetNextToken()
        {
            NNewlines = 0;
            if (ParserPos >= Input.Length)
            {
                return new Tuple<string, string>("", "TK_EOF");
            }
            WantedNewline = false;
            char c = Input[ParserPos];
            ParserPos++;
            if (Opts.KeepArrayIndentation && IsArray(Flags.Mode))
            {
                int num = 0;
                while (Whitespace.Contains(c))
                {
                    switch (c)
                    {
                        case '\n':
                            TrimOutput();
                            Output.Add("\n");
                            JustAddedNewline = true;
                            num = 0;
                            break;
                        case '\t':
                            num += 4;
                            break;
                        default:
                            num++;
                            break;
                        case '\r':
                            break;
                    }
                    if (ParserPos >= Input.Length)
                    {
                        return new Tuple<string, string>("", "TK_EOF");
                    }
                    c = Input[ParserPos];
                    ParserPos++;
                }
                if (JustAddedNewline)
                {
                    foreach (int item2 in Enumerable.Range(0, num))
                    {
                        _ = item2;
                        Output.Add(" ");
                    }
                }
            }
            else
            {
                while (Whitespace.Contains(c))
                {
                    if (c == '\n' && (Opts.MaxPreserveNewlines == 0f || Opts.MaxPreserveNewlines > (float)NNewlines))
                    {
                        NNewlines++;
                    }
                    if (ParserPos >= Input.Length)
                    {
                        return new Tuple<string, string>("", "TK_EOF");
                    }
                    c = Input[ParserPos];
                    ParserPos++;
                }
                if (Opts.PreserveNewlines && NNewlines > 1)
                {
                    foreach (int item3 in Enumerable.Range(0, NNewlines))
                    {
                        AppendNewline(item3 == 0);
                        JustAddedNewline = true;
                    }
                }
                WantedNewline = NNewlines > 0;
            }
            string text = c.ToString();
            if (Wordchar.Contains(c))
            {
                if (ParserPos < Input.Length)
                {
                    text = c.ToString();
                    while (Wordchar.Contains(Input[ParserPos]))
                    {
                        text += Input[ParserPos];
                        ParserPos++;
                        if (ParserPos == Input.Length)
                        {
                            break;
                        }
                    }
                }
                if (ParserPos != Input.Length && "+-".Contains(Input[ParserPos]) && Regex.IsMatch(text, "^[0-9]+[Ee]$"))
                {
                    char c2 = Input[ParserPos];
                    ParserPos++;
                    Tuple<string, string> nextToken = GetNextToken();
                    text = text + c2 + nextToken.Item1;
                    return new Tuple<string, string>(text, "TK_WORD");
                }
                if (text == "in")
                {
                    return new Tuple<string, string>(text, "TK_OPERATOR");
                }
                if (WantedNewline && (LastText == "++" || LastText == "--" || LastType != "TK_OPERATOR") && LastType != "TK_EQUALS" && !Flags.IfLine && (Opts.PreserveNewlines || LastText != "var"))
                {
                    AppendNewline();
                }
                return new Tuple<string, string>(text, "TK_WORD");
            }
            if ("([".Contains(c))
            {
                return new Tuple<string, string>(c.ToString(), "TK_START_EXPR");
            }
            if (")]".Contains(c))
            {
                return new Tuple<string, string>(c.ToString(), "TK_END_EXPR");
            }
            switch (c)
            {
                case '{':
                    return new Tuple<string, string>(c.ToString(), "TK_START_BLOCK");
                case '}':
                    return new Tuple<string, string>(c.ToString(), "TK_END_BLOCK");
                case ';':
                    return new Tuple<string, string>(c.ToString(), "TK_SEMICOLON");
                case '/':
                    {
                        string text2 = "";
                        string item = "TK_INLINE_COMMENT";
                        if (Input[ParserPos] == '*')
                        {
                            ParserPos++;
                            if (ParserPos < Input.Length)
                            {
                                while ((Input[ParserPos] != '*' || ParserPos + 1 >= Input.Length || Input[ParserPos + 1] != '/') && ParserPos < Input.Length)
                                {
                                    c = Input[ParserPos];
                                    text2 += c;
                                    if ("\r\n".Contains(c))
                                    {
                                        item = "TK_BLOCK_COMMENT";
                                    }
                                    ParserPos++;
                                    if (ParserPos >= Input.Length)
                                    {
                                        break;
                                    }
                                }
                            }
                            ParserPos += 2;
                            return new Tuple<string, string>("/*" + text2 + "*/", item);
                        }
                        if (Input[ParserPos] != '/')
                        {
                            break;
                        }
                        text2 = c.ToString();
                        while (!"\r\n".Contains(Input[ParserPos]))
                        {
                            text2 += Input[ParserPos];
                            ParserPos++;
                            if (ParserPos >= Input.Length)
                            {
                                break;
                            }
                        }
                        if (WantedNewline)
                        {
                            AppendNewline();
                        }
                        return new Tuple<string, string>(text2, "TK_COMMENT");
                    }
            }
            if (c == '\'' || c == '"' || (c == '/' && ((LastType == "TK_WORD" && IsSpecialWord(LastText)) || (LastType == "TK_END_EXPR" && (Flags.PreviousMode == "(FOR-EXPRESSION)" || Flags.PreviousMode == "(COND-EXPRESSION)")) || new string[9] { "TK_COMMENT", "TK_START_EXPR", "TK_START_BLOCK", "TK_END_BLOCK", "TK_OPERATOR", "TK_EQUALS", "TK_EOF", "TK_SEMICOLON", "TK_COMMA" }.Contains(LastType))))
            {
                char c3 = c;
                bool flag = false;
                int result = 0;
                int num2 = 0;
                string text3 = c.ToString();
                bool flag2 = false;
                if (ParserPos < Input.Length)
                {
                    if (c3 == '/')
                    {
                        flag2 = false;
                        while (flag || flag2 || Input[ParserPos] != c3)
                        {
                            text3 += Input[ParserPos];
                            if (!flag)
                            {
                                flag = Input[ParserPos] == '\\';
                                if (Input[ParserPos] == '[')
                                {
                                    flag2 = true;
                                }
                                else if (Input[ParserPos] == ']')
                                {
                                    flag2 = false;
                                }
                            }
                            else
                            {
                                flag = false;
                            }
                            ParserPos++;
                            if (ParserPos >= Input.Length)
                            {
                                return new Tuple<string, string>(text3, "TK_STRING");
                            }
                        }
                    }
                    else
                    {
                        while (flag || Input[ParserPos] != c3)
                        {
                            text3 += Input[ParserPos];
                            if (result != 0 && result >= num2)
                            {
                                if (!int.TryParse(new string(text3.Skip(Math.Max(0, text3.Count() - num2)).Take(num2).ToArray()), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out result))
                                {
                                    result = 0;
                                }
                                if (result != 0 && result >= 32 && result <= 126)
                                {
                                    text3 = new string(text3.Take(2 + num2).ToArray());
                                    if ((ushort)result == c3 || (ushort)result == 92)
                                    {
                                        text3 += "\\";
                                    }
                                    text3 += (char)result/*cast due to constrained. prefix*/;
                                }
                                result = 0;
                            }
                            if (result == 0)
                            {
                                flag = !flag && Input[ParserPos] == '\\';
                            }
                            else
                            {
                                result++;
                            }
                            ParserPos++;
                            if (ParserPos >= Input.Length)
                            {
                                return new Tuple<string, string>(text3, "TK_STRING");
                            }
                        }
                    }
                }
                ParserPos++;
                text3 += c3;
                if (c3 == '/')
                {
                    while (ParserPos < Input.Length && Wordchar.Contains(Input[ParserPos]))
                    {
                        text3 += Input[ParserPos];
                        ParserPos++;
                    }
                }
                return new Tuple<string, string>(text3, "TK_STRING");
            }
            switch (c)
            {
                case '#':
                    {
                        string text5 = "";
                        if (Output.Count == 0 && Input.Length > 1 && Input[ParserPos] == '!')
                        {
                            text5 = c.ToString();
                            while (ParserPos < Input.Length && c != '\n')
                            {
                                c = Input[ParserPos];
                                text5 += c;
                                ParserPos++;
                            }
                            Output.Add(text5.Trim() + "\n");
                            AppendNewline();
                            return GetNextToken();
                        }
                        string text6 = "#";
                        if (ParserPos < Input.Length && Digits.Contains(Input[ParserPos]))
                        {
                            do
                            {
                                c = Input[ParserPos];
                                text6 += c;
                                ParserPos++;
                            }
                            while (ParserPos < Input.Length && c != '#' && c != '=');
                        }
                        if (c != '#' && ParserPos < Input.Length)
                        {
                            if (Input[ParserPos] == '[' && Input[ParserPos + 1] == ']')
                            {
                                text6 += "[]";
                                ParserPos += 2;
                            }
                            else if (Input[ParserPos] == '{' && Input[ParserPos + 1] == '}')
                            {
                                text6 += "{}";
                                ParserPos += 2;
                            }
                        }
                        return new Tuple<string, string>(text6, "TK_WORD");
                    }
                case '<':
                    if (Input.Substring(ParserPos - 1, Math.Min(4, Input.Length - ParserPos + 1)) == "<!--")
                    {
                        ParserPos += 3;
                        string text4 = "<!--";
                        while (ParserPos < Input.Length && Input[ParserPos] != '\n')
                        {
                            text4 += Input[ParserPos];
                            ParserPos++;
                        }
                        Flags.InHtmlComment = true;
                        return new Tuple<string, string>(text4, "TK_COMMENT");
                    }
                    break;
            }
            if (c == '-' && Flags.InHtmlComment && Input.Substring(ParserPos - 1, 3) == "-->")
            {
                Flags.InHtmlComment = false;
                ParserPos += 2;
                if (WantedNewline)
                {
                    AppendNewline();
                }
                return new Tuple<string, string>("-->", "TK_COMMENT");
            }
            if (c == '.')
            {
                return new Tuple<string, string>(".", "TK_DOT");
            }
            if (Punct.Contains(c.ToString()))
            {
                string text7 = c.ToString();
                while (ParserPos < Input.Length && Punct.Contains(text7 + Input[ParserPos]))
                {
                    text7 += Input[ParserPos];
                    ParserPos++;
                    if (ParserPos >= Input.Length)
                    {
                        break;
                    }
                }
                if (text7 == "=")
                {
                    return new Tuple<string, string>("=", "TK_EQUALS");
                }
                if (text7 == ",")
                {
                    return new Tuple<string, string>(",", "TK_COMMA");
                }
                return new Tuple<string, string>(text7, "TK_OPERATOR");
            }
            return new Tuple<string, string>(c.ToString(), "TK_UNKNOWN");
        }

        private void HandleStartExpr(string tokenText)
        {
            if (tokenText == "[")
            {
                if (LastType == "TK_WORD" || LastText == ")")
                {
                    if (LineStarters.Contains(LastText))
                    {
                        Append(" ");
                    }
                    SetMode("(EXPRESSION)");
                    Append(tokenText);
                    return;
                }
                if (Flags.Mode == "[EXPRESSION]" || Flags.Mode == "[INDENTED-EXPRESSION]")
                {
                    if (LastLastText == "]" && LastText == ",")
                    {
                        if (Flags.Mode == "[EXPRESSION]")
                        {
                            Flags.Mode = "[INDENTED-EXPRESSION]";
                            if (!Opts.KeepArrayIndentation)
                            {
                                Indent();
                            }
                        }
                        SetMode("[EXPRESSION]");
                        if (!Opts.KeepArrayIndentation)
                        {
                            AppendNewline();
                        }
                    }
                    else if (LastText == "[")
                    {
                        if (Flags.Mode == "[EXPRESSION]")
                        {
                            Flags.Mode = "[INDENTED-EXPRESSION]";
                            if (!Opts.KeepArrayIndentation)
                            {
                                Indent();
                            }
                        }
                        SetMode("[EXPRESSION]");
                        if (!Opts.KeepArrayIndentation)
                        {
                            AppendNewline();
                        }
                    }
                    else
                    {
                        SetMode("[EXPRESSION]");
                    }
                }
                else
                {
                    SetMode("[EXPRESSION]");
                }
            }
            else if (LastText == "for")
            {
                SetMode("(FOR-EXPRESSION)");
            }
            else if (LastText == "if" || LastText == "while")
            {
                SetMode("(COND-EXPRESSION)");
            }
            else
            {
                SetMode("(EXPRESSION)");
            }
            if (LastText == ";" || LastType == "TK_START_BLOCK")
            {
                AppendNewline();
            }
            else if (LastType == "TK_END_EXPR" || LastType == "TK_START_EXPR" || LastType == "TK_END_BLOCK" || LastText == ".")
            {
                if (WantedNewline)
                {
                    AppendNewline();
                }
            }
            else if (LastType != "TK_WORD" && LastType != "TK_OPERATOR")
            {
                Append(" ");
            }
            else if (LastWord == "function" || LastWord == "typeof")
            {
                if (Opts.JslintHappy)
                {
                    Append(" ");
                }
            }
            else if (LineStarters.Contains(LastText) || LastText == "catch")
            {
                Append(" ");
            }
            Append(tokenText);
        }

        private void HandleEndExpr(string tokenText)
        {
            if (tokenText == "]")
            {
                if (Opts.KeepArrayIndentation)
                {
                    if (LastText == "}")
                    {
                        RemoveIndent();
                        Append(tokenText);
                        RestoreMode();
                        return;
                    }
                }
                else if (Flags.Mode == "[INDENTED-EXPRESSION]" && LastText == "]")
                {
                    RestoreMode();
                    AppendNewline();
                    Append(tokenText);
                    return;
                }
            }
            RestoreMode();
            Append(tokenText);
        }

        private void HandleStartBlock(string tokenText)
        {
            if (LastWord == "do")
            {
                SetMode("DO_BLOCK");
            }
            else
            {
                SetMode("BLOCK");
            }
            if (Opts.BraceStyle == BraceStyle.Expand)
            {
                if (LastType != "TK_OPERATOR")
                {
                    if (LastText == "=" || (IsSpecialWord(LastText) && LastText != "else"))
                    {
                        Append(" ");
                    }
                    else
                    {
                        AppendNewline();
                    }
                }
                Append(tokenText);
                Indent();
                return;
            }
            if (LastType != "TK_OPERATOR" && LastType != "TK_START_EXPR")
            {
                if (LastType == "TK_START_BLOCK")
                {
                    AppendNewline();
                }
                else
                {
                    Append(" ");
                }
            }
            else if (IsArray(Flags.PreviousMode) && LastText == ",")
            {
                if (LastLastText == "}")
                {
                    Append(" ");
                }
                else
                {
                    AppendNewline();
                }
            }
            Indent();
            Append(tokenText);
        }

        private void HandleEndBlock(string tokenText)
        {
            RestoreMode();
            if (Opts.BraceStyle == BraceStyle.Expand)
            {
                if (LastText != "{")
                {
                    AppendNewline();
                }
            }
            else if (LastType == "TK_START_BLOCK")
            {
                if (JustAddedNewline)
                {
                    RemoveIndent();
                }
                else
                {
                    TrimOutput();
                }
            }
            else if (IsArray(Flags.Mode) && Opts.KeepArrayIndentation)
            {
                Opts.KeepArrayIndentation = false;
                AppendNewline();
                Opts.KeepArrayIndentation = true;
            }
            else
            {
                AppendNewline();
            }
            Append(tokenText);
        }

        private void HandleWord(string tokenText)
        {
            if (DoBlockJustClosed)
            {
                Append(" ");
                Append(tokenText);
                Append(" ");
                DoBlockJustClosed = false;
                return;
            }
            switch (tokenText)
            {
                case "function":
                    if (Flags.VarLine && LastText != "=")
                    {
                        Flags.VarLineReindented = !Opts.KeepFunctionIndentation;
                    }
                    if ((JustAddedNewline || LastText == ";") && LastText != "{")
                    {
                        int num = NNewlines;
                        if (!JustAddedNewline)
                        {
                            num = 0;
                        }
                        if (!Opts.PreserveNewlines)
                        {
                            num = 1;
                        }
                        foreach (int item in Enumerable.Range(0, 2 - num))
                        {
                            _ = item;
                            AppendNewline(ignoreRepeated: false);
                        }
                    }
                    if (LastText == "get" || LastText == "set" || LastText == "new" || LastType == "TK_WORD")
                    {
                        Append(" ");
                    }
                    if (LastType == "TK_WORD")
                    {
                        if (LastText == "get" || LastText == "set" || LastText == "new" || LastText == "return")
                        {
                            Append(" ");
                        }
                        else
                        {
                            AppendNewline();
                        }
                    }
                    else if (LastType == "TK_OPERATOR" || LastText == "=")
                    {
                        Append(" ");
                    }
                    else if (!IsExpression(Flags.Mode))
                    {
                        AppendNewline();
                    }
                    Append("function");
                    LastWord = "function";
                    return;
                case "default":
                    if (!Flags.InCaseStatement)
                    {
                        break;
                    }
                    goto case "case";
                case "case":
                    AppendNewline();
                    if (Flags.CaseBody)
                    {
                        RemoveIndent();
                        Flags.CaseBody = false;
                        Flags.IndentationLevel--;
                    }
                    Append(tokenText);
                    Flags.InCase = true;
                    Flags.InCaseStatement = true;
                    return;
            }
            string text = "NONE";
            if (LastType == "TK_END_BLOCK")
            {
                if (tokenText != "else" && tokenText != "catch" && tokenText != "finally")
                {
                    text = "NEWLINE";
                }
                else if (Opts.BraceStyle == BraceStyle.Expand || Opts.BraceStyle == BraceStyle.EndExpand)
                {
                    text = "NEWLINE";
                }
                else
                {
                    text = "SPACE";
                    Append(" ");
                }
            }
            else if (LastType == "TK_SEMICOLON" && (Flags.Mode == "BLOCK" || Flags.Mode == "DO_BLOCK"))
            {
                text = "NEWLINE";
            }
            else if (LastType == "TK_SEMICOLON" && IsExpression(Flags.Mode))
            {
                text = "SPACE";
            }
            else if (LastType == "TK_STRING")
            {
                text = "NEWLINE";
            }
            else if (LastType == "TK_WORD")
            {
                if (LastText == "else")
                {
                    TrimOutput(eatNewlines: true);
                }
                text = "SPACE";
            }
            else if (LastType == "TK_START_BLOCK")
            {
                text = "NEWLINE";
            }
            else if (LastType == "TK_END_EXPR")
            {
                Append(" ");
                text = "NEWLINE";
            }
            if (Flags.IfLine && LastType == "TK_END_EXPR")
            {
                Flags.IfLine = false;
            }
            if (LineStarters.Contains(tokenText))
            {
                text = ((!(LastText == "else")) ? "NEWLINE" : "SPACE");
            }
            switch (tokenText)
            {
                case "else":
                case "catch":
                case "finally":
                    if (LastType != "TK_END_BLOCK" || Opts.BraceStyle == BraceStyle.Expand || Opts.BraceStyle == BraceStyle.EndExpand)
                    {
                        AppendNewline();
                        break;
                    }
                    TrimOutput(eatNewlines: true);
                    Append(" ");
                    break;
                default:
                    if (text == "NEWLINE")
                    {
                        if (IsSpecialWord(LastText))
                        {
                            Append(" ");
                        }
                        else if (LastType != "TK_END_EXPR")
                        {
                            if ((LastType != "TK_START_EXPR" || tokenText != "var") && LastText != ":")
                            {
                                if (tokenText == "if" && LastWord == "else" && LastText != "{")
                                {
                                    Append(" ");
                                    break;
                                }
                                Flags.VarLine = false;
                                Flags.VarLineReindented = false;
                                AppendNewline();
                            }
                        }
                        else if (LineStarters.Contains(tokenText) && LastText != ")")
                        {
                            Flags.VarLine = false;
                            Flags.VarLineReindented = false;
                            AppendNewline();
                        }
                    }
                    else if (IsArray(Flags.Mode) && LastText == "," && LastLastText == "}")
                    {
                        AppendNewline();
                    }
                    else if (text == "SPACE")
                    {
                        Append(" ");
                    }
                    break;
            }
            Append(tokenText);
            LastWord = tokenText;
            if (tokenText == "var")
            {
                Flags.VarLine = true;
                Flags.VarLineReindented = false;
                Flags.VarLineTainted = false;
            }
            if (tokenText == "if")
            {
                Flags.IfLine = true;
            }
            if (tokenText == "else")
            {
                Flags.IfLine = false;
            }
        }

        private void HandleSemicolon(string tokenText)
        {
            Append(tokenText);
            Flags.VarLine = false;
            Flags.VarLineReindented = false;
            if (Flags.Mode == "OBJECT")
            {
                Flags.Mode = "BLOCK";
            }
        }

        private void HandleString(string tokenText)
        {
            if (LastType == "TK_END_EXPR" && (Flags.PreviousMode == "(COND-EXPRESSION)" || Flags.PreviousMode == "(FOR-EXPRESSION)"))
            {
                Append(" ");
            }
            if (LastType == "TK_COMMENT" || LastType == "TK_STRING" || LastType == "TK_START_BLOCK" || LastType == "TK_END_BLOCK" || LastType == "TK_SEMICOLON")
            {
                AppendNewline();
            }
            else if (LastType == "TK_WORD")
            {
                Append(" ");
            }
            else if (Opts.PreserveNewlines && WantedNewline && Flags.Mode != "OBJECT")
            {
                AppendNewline();
                Append(IndentString);
            }
            Append(tokenText);
        }

        private void HandleEquals(string tokenText)
        {
            if (Flags.VarLine)
            {
                Flags.VarLineTainted = true;
            }
            Append(" ");
            Append(tokenText);
            Append(" ");
        }

        private void HandleComma(string tokenText)
        {
            if (LastType == "TK_COMMENT")
            {
                AppendNewline();
            }
            if (Flags.VarLine)
            {
                if (IsExpression(Flags.Mode) || LastType == "TK_END_BLOCK")
                {
                    Flags.VarLineTainted = false;
                }
                if (Flags.VarLineTainted)
                {
                    Append(tokenText);
                    Flags.VarLineReindented = true;
                    Flags.VarLineTainted = false;
                    AppendNewline();
                }
                else
                {
                    Flags.VarLineTainted = false;
                    Append(tokenText);
                    Append(" ");
                }
            }
            else if (LastType == "TK_END_BLOCK" && Flags.Mode != "(EXPRESSION)")
            {
                Append(tokenText);
                if (Flags.Mode == "OBJECT" && LastText == "}")
                {
                    AppendNewline();
                }
                else
                {
                    Append(" ");
                }
            }
            else if (Flags.Mode == "OBJECT")
            {
                Append(tokenText);
                AppendNewline();
            }
            else
            {
                Append(tokenText);
                Append(" ");
            }
        }

        private void HandleOperator(string tokenText)
        {
            bool flag = true;
            bool flag2 = true;
            if (IsSpecialWord(LastText))
            {
                Append(" ");
                Append(tokenText);
                return;
            }
            if (tokenText == "*" && LastType == "TK_DOT" && !LastLastText.All(char.IsDigit))
            {
                Append(tokenText);
                return;
            }
            if (tokenText == ":" && Flags.InCase)
            {
                Flags.CaseBody = true;
                Indent();
                Append(tokenText);
                AppendNewline();
                Flags.InCase = true;
                return;
            }
            switch (tokenText)
            {
                case "::":
                    Append(tokenText);
                    return;
                case "+":
                case "-":
                    if (LastType == "TK_START_BLOCK" || LastType == "TK_START_EXPR" || LastType == "TK_EQUALS" || LastType == "TK_OPERATOR" || LineStarters.Contains(LastText) || LastText == ",")
                    {
                        goto case "++";
                    }
                    goto default;
                case "++":
                case "--":
                case "!":
                    flag = false;
                    flag2 = false;
                    if (LastText == ";" && IsExpression(Flags.Mode))
                    {
                        flag = true;
                    }
                    if (LastType == "TK_WORD" && LineStarters.Contains(LastText))
                    {
                        flag = true;
                    }
                    if (Flags.Mode == "BLOCK" && (LastText == ";" || LastText == "{"))
                    {
                        AppendNewline();
                    }
                    break;
                default:
                    if (tokenText == ":")
                    {
                        if (Flags.TernaryDepth == 0)
                        {
                            if (Flags.Mode == "BLOCK")
                            {
                                Flags.Mode = "OBJECT";
                            }
                            flag = false;
                        }
                        else
                        {
                            Flags.TernaryDepth--;
                        }
                    }
                    else if (tokenText == "?")
                    {
                        Flags.TernaryDepth++;
                    }
                    break;
            }
            if (flag)
            {
                Append(" ");
            }
            Append(tokenText);
            if (flag2)
            {
                Append(" ");
            }
        }

        private void HandleBlockComment(string tokenText)
        {
            string[] array = tokenText.Replace("\r", "").Split('\n');
            if (!(from x in array.Skip(1)
                  where x.Trim() == "" || x.TrimStart()[0] != '*'
                  select x).Any((string l) => !string.IsNullOrEmpty(l)))
            {
                AppendNewline();
                Append(array[0]);
                foreach (string item in array.Skip(1))
                {
                    AppendNewline();
                    Append(" " + item.Trim());
                }
            }
            else
            {
                if (array.Length > 1)
                {
                    AppendNewline();
                }
                else
                {
                    Append(" ");
                }
                string[] array2 = array;
                foreach (string s in array2)
                {
                    Append(s);
                    Append("\n");
                }
            }
            AppendNewline();
        }

        private void HandleInlineComment(string tokenText)
        {
            Append(" ");
            Append(tokenText);
            if (IsExpression(Flags.Mode))
            {
                Append(" ");
            }
            else
            {
                AppendNewlineForced();
            }
        }

        private void HandleComment(string tokenText)
        {
            if (LastText == "," && !WantedNewline)
            {
                TrimOutput(eatNewlines: true);
            }
            if (LastType != "TK_COMMENT")
            {
                if (WantedNewline)
                {
                    AppendNewline();
                }
                else
                {
                    Append(" ");
                }
            }
            Append(tokenText);
            AppendNewline();
        }

        private void HandleDot(string tokenText)
        {
            if (IsSpecialWord(LastText))
            {
                Append(" ");
            }
            else if (LastText == ")" && (Opts.BreakChainedMethods || WantedNewline))
            {
                Flags.ChainExtraIndentation = 1;
                AppendNewline(ignoreRepeated: true, resetStatementFlags: false);
            }
            Append(tokenText);
        }

        private void HandleUnknown(string tokenText)
        {
            Append(tokenText);
        }
    }
}