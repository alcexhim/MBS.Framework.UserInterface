using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MBS.Framework.UserInterface.Common
{
	public static class Reflection
	{
		private static Assembly[] mvarAvailableAssemblies = null;
		public static Assembly[] GetAvailableAssemblies()
		{
			if (mvarAvailableAssemblies == null)
			{
				List<Assembly> list = new List<Assembly>();
				
				string basePath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				Console.WriteLine("uwt: looking for engines in '{0}'", basePath);

				string[] fileNames = System.IO.Directory.GetFiles(basePath, "*.dll", System.IO.SearchOption.AllDirectories);
				foreach (string fileName in fileNames)
				{
					Console.Write("loading '{0}' ...", fileName);
					Assembly asm = null;
					try
					{
						asm = Assembly.LoadFile(fileName);
					}
					catch (Exception ex)
					{
						Console.WriteLine("failed! {0}", ex.Message);
					}

					if (asm == null) continue;

					Console.WriteLine(" [ OK ]");
					list.Add(asm);
				}
				mvarAvailableAssemblies = list.ToArray();
			}
			return mvarAvailableAssemblies;
		}

		private static Type[] mvarAvailableTypes = null;
		public static Type[] GetAvailableTypes(Type[] inheritsFromTypes = null)
		{
			if (mvarAvailableTypes == null)
			{
				List<Type> list = new List<Type>();
				Assembly[] asms = GetAvailableAssemblies();
				foreach (Assembly asm in asms)
				{
					Type[] types = null;
					try
					{
						types = asm.GetTypes();
					}
					catch (ReflectionTypeLoadException ex)
					{
						types = ex.Types;
					}
					foreach (Type type in types)
					{
						if (type == null) continue;
						list.Add(type);
					}
				}
				mvarAvailableTypes = list.ToArray();
			}

			if (inheritsFromTypes == null) return mvarAvailableTypes;

			List<Type> list2 = new List<Type>();
			foreach (Type type in mvarAvailableTypes)
			{
				if (type.IsAbstract) continue;
				foreach (Type inheritsFromType in inheritsFromTypes)
				{
					if (type.IsSubclassOf(inheritsFromType)) list2.Add(type);
				}
			}
			return list2.ToArray();
		}
	}
}
