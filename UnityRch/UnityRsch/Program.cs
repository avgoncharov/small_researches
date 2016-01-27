using System;
using System.IO;
using System.Linq;
using System.Reflection;
using InterfaceForUnity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace UnityRsch
{
	public class Program
	{
		public static void Main()
		{
			BuildUpTest();
			Console.WriteLine("----------------------------------------------");
			LoadFromConfigWithSubDirs();
			Console.WriteLine("----------------------------------------------");
			LoadFromConfigWithSubDirsWithCustomConfigSection();
			Console.WriteLine("----------------------------------------------");
			LoadDynamicly();
			Console.ReadLine();		

		}

		private static void LoadDynamicly()
		{
			var x = Assembly.LoadFrom(@"Outside\TypesForUnityDynamicLoading.dll");
			var expected = x.GetExportedTypes().FirstOrDefault(type => typeof(IForUnityTest).IsAssignableFrom(type));

			var c = new UnityContainer();
			c.RegisterType(typeof(IForUnityTest), expected);

			var resolve = c.Resolve<IForUnityTest>();
			Console.WriteLine("Resolve is not null: {0}.", resolve != null);
			if (resolve != null)
			{
				Console.WriteLine("CountOfStars: {0}.", resolve.CountOfStars());
			}
		}
		
		private static void LoadFromConfigWithSubDirsWithCustomConfigSection()
		{
			var mc = new ConfigSectionAdapter();
			mc.LoadFromString(File.ReadAllText("AddToContainer.xml"));
			var c = new UnityContainer();
			c.LoadConfiguration(mc);

			var i3 = c.Resolve<IForUnityTest>("q3");
			Console.WriteLine("i3: {0}", i3);

		}

		private static void LoadFromConfigWithSubDirs()
		{
			// To load types from dll which are in some dir (not in your bin) 
			// you have to use in config file tag <probing privatePath="subdir"/>
			IUnityContainer c = new UnityContainer();
			c.LoadConfiguration();

			var testInt32 = c.Resolve<ITest<int>>();
			Console.WriteLine("testInt32 is not null: {0}.", testInt32 != null);
			var testString = c.Resolve<ITest<string>>();
			Console.WriteLine("testString is not null: {0}.", testString != null);
		}

		private static void BuildUpTest()
		{
			var container = new UnityContainer();
			container.RegisterType<IResource, Resource>();
			
			var m = container.Resolve<BuildUpClass>();

			Console.WriteLine("Resource in BuildUpClass is not null: {0}.", m.Resource != null);
			if (m.Resource != null)
			{
				m.Resource.Print();
			}
		}
	}
}
