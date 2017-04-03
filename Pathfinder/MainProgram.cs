﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using System.Threading;
using HacknetPathfinder.Pathfinder;
using Hacknet;

namespace Pathfinder
{
	public static class MainProgram
	{
		internal static void Main(string[] args)
		{
			// Opens Hacknet.exe's Assembly
			AssemblyDefinition ad = AssemblyDefinition.ReadAssembly("Hacknet.exe");
			ad.AddAssemblyAttribute<InternalsVisibleToAttribute>("HacknetPathfinder");
            ad.Write("PatchedHacknet.exe");

            Type.GetType("Hacknet.MainMenu, Hacknet").GetField("OSVersion", BindingFlags.Static | BindingFlags.Public).SetValue(null, "Pathfinder v0.1");

            Thread hacknetThread = new Thread(() => HacknetLauncher.runHacknet(args));
            hacknetThread.Start();

            Console.WriteLine("Pathfinder Initialized.");
        }

		internal static void AddAssemblyAttribute<T>(this AssemblyDefinition ad, params object[] attribArgs)
		{
			var paramTypes = attribArgs.Length > 0 ? new Type[attribArgs.Length] : Type.EmptyTypes;
			int index = 0;
			foreach (var param in attribArgs)
			{
				paramTypes[index] = param.GetType();
				index++;
			}
			var attribCtor = ad.MainModule.ImportReference(typeof(T).GetConstructor(paramTypes));
			var attrib = new CustomAttribute(attribCtor);
			foreach (var param in attribArgs)
			{
				attrib.ConstructorArguments.Add(
					new CustomAttributeArgument(ad.MainModule.ImportReference(param.GetType()), param)
				);
			}
			ad.CustomAttributes.Add(attrib);
		}
	}
}