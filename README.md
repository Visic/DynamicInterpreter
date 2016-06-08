# DynamicInterpreter
This is a parser/interpreter generator using a simple custom description language for defining languages

Example of the description language (This is actually the current description language written in itself!)--

\<whitespace\> = ' '|'\\t'|'\\r\\n'|'\\n'
\<escaped\> = '\\\\'-'\\\\'|'\\\\\\\\'\<escaped\>
\<all_escape_chars\> = '\\\\'|'\\\\'\<all_escape_chars\>
\<almost_all_escape_chars\> = '\\\\'--'\\\\'|'\\\\'\<almost_all_escape_chars\>
\<all_whitespace\> = \<whitespace\>|\<whitespace\>\<all_whitespace\>
\<ignore_all_whitespace\> = ''|\<all_whitespace\>
\<anychar\> = '*'

\<range\> = '[' ('\\\\]'|'\\\\-'|-('-'|']')*) '-' ('\\\\]'|'\\\\-'|-('-'|']')*) ']'

\<allchars_not_quote\> = -('\\''|\<escaped\>\<almost_all_escape_chars\>'\\\\\\'')(--\<escaped\>\<all_escape_chars\>*|-\<escaped\>\<all_escape_chars\>|*)(''|\<allchars_not_quote\>)
\<literal\> = '\\''(''|\<allchars_not_quote\>)'\\''

\<allchars_not_gt\> = -('\>'|\<escaped\>\<almost_all_escape_chars\>'\\\\\>')(--\<escaped\>\<all_escape_chars\>*|-\<escaped\>\<all_escape_chars\>|*)(''|\<allchars_not_gt\>)
\<symbol\> = '\<'\<allchars_not_gt\>'\>'

\<inorder_ele\> = \<ignore_all_whitespace\>(\<symbol\>\<ignore_all_whitespace\>-'='|\<literal\>|\<group\>|\<negation\>|\<anychar\>|\<range\>)
\<inorder\> = \<inorder_ele\>|\<inorder_ele\>\<inorder\>
\<all_inorder\> = \<inorder\>

\<any\> = \<all_inorder\>|\<all_inorder\>'|'\<any\>
\<all_any\> = \<any\>
\<group\> = '('\<all_any\>\<ignore_all_whitespace\>')'
\<negation\> = '-'(\<group\>|\<symbol\>|\<literal\>|\<negation\>|\<range\>|\<anychar\>)
\<assignment\> = \<ignore_all_whitespace\>'\<'\<allchars_not_gt\>'\>'\<ignore_all_whitespace\>'='\<all_any\>\<ignore_all_whitespace\>

\<fallback_point\> = 'f'\<assignment\>
f\<all_assignments\> = (\<assignment\>|\<fallback_point\>)(''|\<all_assignments\>)
\<all_all_assignments\> = \<all_assignments\>
\<EntryPoint\> = \<all_all_assignments\>

Note:: The "EntryPoint" symbol is required, without it you will probably crash..

How to use::
All features are available through the REPL, "MakeInterpreter" and "Language" are the two key commands.
The MakeInterpreter command takes a filepath to your description language, an output directory, the name of your parser, and generates three files to include in your project.
	{name}Parser_Edit.cs -- this is where you will put your symbol handlers (the code that will get called as a result of successfully parsing your language)
	{name}Parser_NoEdit.cs -- Generated Parser code
	InterpreterSupport_NoEdit.cs -- Support code required by {name}Parser_NoEdit.cs but abstracted in order to de-duplicate (useful if you make more than one parser in a single project)

The Language command takes the filepath to a "Language" assembly (see TestLanguage or ParserGraphing for an example)
	This command will actually load the symbol handlers and language definition in the specified assembly, and allow you to use the REPL to run your own language (Note:: Hit {ESCAPE} in the REPL to change between prompts)


How it works::
The Parser itself is a set "Parse" functions which get combined to form the complete parsing routines for each symbol (Note:: This is based on Parser Combinators)
When the Parser asked to "Execute" some code, that code is run through the "EntryPoint" parser, and the result (if not an error) is a tree of parsed strings.
This tree is then passed to a function (Interpreter.RecursiveEval) along with your symbol handlers.
The RecursiveEval function passes the parsed data through your symbol handlers (as well as whatever you return from your handlers) until it completes, then it just returns whatever it has left (the result of your EntryPoint handler)
