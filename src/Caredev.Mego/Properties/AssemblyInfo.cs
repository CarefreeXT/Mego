// Copyright (c) CarefreeXT and Caredev Studios. All rights reserved.
// Licensed under the GNU Lesser General Public License v3.0. 
// See License.txt in the project root for license information.
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
// 将 ComVisible 设置为 false 会使此程序集中的类型
//对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型
//请将此类型的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]
// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("f7cd97dc-8e1e-4dbc-b7b7-c2ff42dc6a8a")]

#if RELEASE
[assembly: AssemblyKeyName("VS_KEY_MEGO20150825")]
#else
[assembly: InternalsVisibleTo("Caredev.Mego.Tests")]
#endif