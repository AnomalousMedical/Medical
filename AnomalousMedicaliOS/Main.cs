using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace System.IO
{
	class FileSystemEventArgs
	{

	}
}

namespace AnomalousMedicaliOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			Medical.Main.Run();
		}

		void GlNoOp ()
		{
			OpenTK.Graphics.ES20.GL.IsEnabled (OpenTK.Graphics.ES20.EnableCap.Blend);
		}
	}
}
