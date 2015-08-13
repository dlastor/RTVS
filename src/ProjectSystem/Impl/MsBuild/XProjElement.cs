﻿using System.Xml.Linq;
using static Microsoft.VisualStudio.ProjectSystem.FileSystemMirroring.MsBuild.XProjHelpers;

namespace Microsoft.VisualStudio.ProjectSystem.FileSystemMirroring.MsBuild
{
	public class XProjElement : XElement
	{
		public XProjElement(string name) : base(MsBuildNamespace + name)
		{
		}

		public XProjElement(string name, object content) : base(MsBuildNamespace + name, content)
		{
		}

		public XProjElement(string name, params object[] content) : base(MsBuildNamespace + name, content)
		{
		}
	}
}