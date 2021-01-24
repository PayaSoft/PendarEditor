// ReSharper disable UnreachableCode

using System.Reflection;
#if !AssemblyVersion
[assembly: AssemblyVersion(Internal.Version)]
#endif
[assembly: AssemblyFileVersion(Internal.Version)]
[assembly: AssemblyInformationalVersion(Internal.Version)]

// ReSharper disable once UnreachableCode
[assembly: AssemblyConfiguration(Internal.IsDebug ? "DEBUG" : "RELEASE")]
[assembly: AssemblyCompany(Internal.Company)]
[assembly: AssemblyProduct(@"پندار")]
[assembly: AssemblyCopyright(@"تمام حقوق این نرم‌افزار متعلق به شرکت پایا افزار می‌باشد")]
[assembly: AssemblyTrademark(@"Pendar")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: System.Runtime.InteropServices.ComVisible(false)]

[assembly: System.CLSCompliant(true)]

// ReSharper disable once PartialTypeWithSinglePart
// ReSharper disable once CheckNamespace
internal static partial class Internal
{
    internal const string Company = @"Paya";

    internal const string Version = @"2.9.5.13"; // also change the ~/new/js/common.js and ~/new/js/minified/common.js

#if DEBUGcommon
    internal const bool IsDebug = true;
#else
    internal const bool IsDebug = false;
#endif
}
