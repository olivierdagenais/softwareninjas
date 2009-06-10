using System;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Enable C# 3.0 extensions without referencing System.Core.dll
    /// </summary>
    /// 
    /// <seealso href="http://www.danielmoth.com/Blog/2007/05/using-extension-methods-in-fx-20.html"/>
    /// <seealso href="http://social.msdn.microsoft.com/forums/en-US/vcsharp2008prerelease/thread/9d88f8b1-ff03-4ea6-bd41-8cca9f2cd485/"/>
    [AttributeUsage ( AttributeTargets.Method )]
    public sealed class ExtensionAttribute : Attribute { }
}
