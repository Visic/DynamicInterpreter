﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace REPL.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("REPL.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;uppercase&gt; = &apos;A&apos;|&apos;B&apos;|&apos;C&apos;|&apos;D&apos;|&apos;E&apos;|&apos;F&apos;|&apos;G&apos;|&apos;H&apos;|&apos;I&apos;|&apos;J&apos;|&apos;K&apos;|&apos;L&apos;|&apos;M&apos;|&apos;N&apos;|&apos;O&apos;|&apos;P&apos;|&apos;Q&apos;|&apos;R&apos;|&apos;S&apos;|&apos;T&apos;|&apos;U&apos;|&apos;V&apos;|&apos;W&apos;|&apos;X&apos;|&apos;Y&apos;|&apos;Z&apos;
        ///&lt;lowercase&gt; = &apos;a&apos;|&apos;b&apos;|&apos;c&apos;|&apos;d&apos;|&apos;e&apos;|&apos;f&apos;|&apos;g&apos;|&apos;h&apos;|&apos;i&apos;|&apos;j&apos;|&apos;k&apos;|&apos;l&apos;|&apos;m&apos;|&apos;n&apos;|&apos;o&apos;|&apos;p&apos;|&apos;q&apos;|&apos;r&apos;|&apos;s&apos;|&apos;t&apos;|&apos;u&apos;|&apos;v&apos;|&apos;w&apos;|&apos;x&apos;|&apos;y&apos;|&apos;z&apos;
        ///&lt;digits&gt; = &apos;0&apos;|&apos;1&apos;|&apos;2&apos;|&apos;3&apos;|&apos;4&apos;|&apos;5&apos;|&apos;6&apos;|&apos;7&apos;|&apos;8&apos;|&apos;9&apos;
        ///&lt;whitespace&gt; = &apos; &apos;|&apos;\t&apos;|&apos;\r\n&apos;|&apos;\n&apos;
        ///&lt;specia....
        /// </summary>
        internal static string CommandlineGrammar {
            get {
                return ResourceManager.GetString("CommandlineGrammar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;whitespace&gt; = &apos; &apos;|&apos;\t&apos;|&apos;\r\n&apos;|&apos;\n&apos;
        ///&lt;escaped&gt; = &apos;\\&apos;-&apos;\\&apos;|&apos;\\\\&apos;&lt;escaped&gt;
        ///&lt;all_escape_chars&gt; = &apos;\\&apos;|&apos;\\&apos;&lt;all_escape_chars&gt;
        ///&lt;almost_all_escape_chars&gt; = &apos;\\&apos;--&apos;\\&apos;|&apos;\\&apos;&lt;almost_all_escape_chars&gt;
        ///&lt;all_whitespace&gt; = &lt;whitespace&gt;|&lt;whitespace&gt;&lt;all_whitespace&gt;
        ///&lt;ignore_all_whitespace&gt; = &apos;&apos;|&lt;all_whitespace&gt;
        ///&lt;anychar&gt; = &apos;*&apos;
        ///
        ///&lt;range&gt; = &apos;[&apos; (&apos;\\]&apos;|&apos;\\-&apos;|-(&apos;-&apos;|&apos;]&apos;)*) &apos;-&apos; (&apos;\\]&apos;|&apos;\\-&apos;|-(&apos;-&apos;|&apos;]&apos;)*) &apos;]&apos;
        ///
        ///&lt;allchars_not_quote&gt; = -(&apos;\&apos;&apos;|&lt;escaped&gt;&lt;almost_all_escape_chars&gt;&apos;\\\&apos;&apos;)(--&lt;escaped&gt;&lt;all_escape_chars&gt;*|-&lt;escaped&gt;&lt;all_es [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DescriptionLanguageGrammar {
            get {
                return ResourceManager.GetString("DescriptionLanguageGrammar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;uppercase&gt; = &apos;A&apos;|&apos;B&apos;|&apos;C&apos;|&apos;D&apos;|&apos;E&apos;|&apos;F&apos;|&apos;G&apos;|&apos;H&apos;|&apos;I&apos;|&apos;J&apos;|&apos;K&apos;|&apos;L&apos;|&apos;M&apos;|&apos;N&apos;|&apos;O&apos;|&apos;P&apos;|&apos;Q&apos;|&apos;R&apos;|&apos;S&apos;|&apos;T&apos;|&apos;U&apos;|&apos;V&apos;|&apos;W&apos;|&apos;X&apos;|&apos;Y&apos;|&apos;Z&apos;
        ///&lt;lowercase&gt; = &apos;a&apos;|&apos;b&apos;|&apos;c&apos;|&apos;d&apos;|&apos;e&apos;|&apos;f&apos;|&apos;g&apos;|&apos;h&apos;|&apos;i&apos;|&apos;j&apos;|&apos;k&apos;|&apos;l&apos;|&apos;m&apos;|&apos;n&apos;|&apos;o&apos;|&apos;p&apos;|&apos;q&apos;|&apos;r&apos;|&apos;s&apos;|&apos;t&apos;|&apos;u&apos;|&apos;v&apos;|&apos;w&apos;|&apos;x&apos;|&apos;y&apos;|&apos;z&apos;
        ///&lt;digit&gt; = &apos;0&apos;|&apos;1&apos;|&apos;2&apos;|&apos;3&apos;|&apos;4&apos;|&apos;5&apos;|&apos;6&apos;|&apos;7&apos;|&apos;8&apos;|&apos;9&apos;
        ///&lt;whitespace&gt; = &apos; &apos;|&apos;\t&apos;|&apos;\r\n&apos;|&apos;\n&apos;
        ///&lt;special....
        /// </summary>
        internal static string REPLCommandGrammar {
            get {
                return ResourceManager.GetString("REPLCommandGrammar", resourceCulture);
            }
        }
    }
}
