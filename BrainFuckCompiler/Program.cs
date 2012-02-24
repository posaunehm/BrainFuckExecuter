/*
 * Created by SharpDevelop.
 * User: Hiroshi
 * Date: 2012/02/10
 * Time: 23:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BrainFuckCompiler
{
	class Program
	{
		public static void Main(string[] args)
		{
			
			var compiler = new BrainFuckCompiler("+++{-}");
			compiler.Run();
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}