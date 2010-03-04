#include "stdafx.h"
#include "Version.h"

using namespace System;
using namespace System::Reflection;
using namespace System::Runtime::CompilerServices;
using namespace System::Runtime::InteropServices;
using namespace System::Security::Permissions;

//
// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly:AssemblyTitleAttribute(ASSEMBLY_TITLE)];
[assembly:AssemblyDescriptionAttribute(ASSEMBLY_DESCRIPTION)];
[assembly:AssemblyConfigurationAttribute(ASSEMBLY_CONFIGURATION)];
[assembly:AssemblyCompanyAttribute(ASSEMBLY_COMPANY)];
[assembly:AssemblyProductAttribute(ASSEMBLY_PRODUCT)];
[assembly:AssemblyCopyrightAttribute(ASSEMBLY_COPYRIGHT)];
[assembly:AssemblyTrademarkAttribute(ASSEMBLY_TRADEMARK)];
[assembly:AssemblyCultureAttribute(ASSEMBLY_CULTURE)];

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the value or you can default the Revision and Build Numbers
// by using the '*' as shown below:

[assembly:AssemblyVersionAttribute(ASSEMBLY_VERSION)];

[assembly:ComVisible(false)];

[assembly:CLSCompliantAttribute(true)];

[assembly:SecurityPermission(SecurityAction::RequestMinimum, UnmanagedCode = true)];
