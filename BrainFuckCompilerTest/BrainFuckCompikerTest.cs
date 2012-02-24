/*
 * Created by SharpDevelop.
 * User: Hiroshi
 * Date: 2012/02/12
 * Time: 15:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Linq;

using BrainFuckCompiler;
using NUnit.Framework;
using System.Reactive;
using System.Reactive.Linq;

namespace BrainFuckCompilerTest
{
	[TestFixture]
	public class BrainFuckCompikerTest
	{
		[Test]
		public void ポインタインクリメントテスト()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler(">");
			compiler.Run();
			compiler.ProgramPointer.Is(1);
		}
		
		[Test]
		public void ポインタデクリメントテスト()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler(">><");
			compiler.Run();
			compiler.ProgramPointer.Is(1);
		}
		
		[Test]
		public void ポインタデクリメント例外テスト()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler("<");
			Assert.Throws<IndexOutOfRangeException>(
				() => compiler.Run());
		}
		
		[Test]
		public void 値インクリメントテスト()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler("+");
			compiler.Run();
			compiler.CurrentValue.Is((byte)1);
		}
		
		[Test]
		public void 値インクリメントテスト_オーバフロー()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler(
				new String( Enumerable.Range(0,256).Select(_=> '+' ).ToArray())
			);
			compiler.Run();
			compiler.CurrentValue.Is(byte.MinValue);
		}
		
		[Test]
		public void ポインタインクリメント値インクリメントテスト()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler(">+");
			compiler.Run();
			compiler.CurrentValue.Is((byte)1);
		}
		
		[Test]
		public void 値デクリメントテスト_アンダーフロー()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler("-");
			compiler.Run();
			compiler.CurrentValue.Is(byte.MaxValue);
		}
		
		[Test]
		public void 値デクリメントテスト_通常()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler("++-");
			compiler.Run();
			compiler.CurrentValue.Is((byte)1);
		}
		
		[Test]
		public void ループテスト_通常()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler("+++[-]");
			compiler.Run();
			compiler.CurrentValue.Is((byte)0);
		}
		
		[Test]
		public void ループテスト_ネスト()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler("+++[[-]]");
			compiler.Run();
			compiler.CurrentValue.Is((byte)0);
		}
		
		[Test]
		public void ループテスト_ネスト_ループしない()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler("[[++++]]");
			compiler.Run();
			compiler.CurrentValue.Is((byte)0);
		}
		
		[Test]
		public void ループテスト_左括弧閉じられず()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler("++--[-");
			Assert.Throws<ApplicationException>(() => compiler.Run());
		}
		
		[Test]
		public void ループテスト_右括弧閉じられず()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler("+++-]-");
			Assert.Throws<ApplicationException>(() => compiler.Run());
		}

		[Test]
		public void 出力テスト()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler("+++.+.+.");
			var answerList = new System.Collections.Generic.List<byte>();
			compiler.OutputEvent += output => {
				answerList.Add(output);
			};
			compiler.Run();

			//Chainning Asserition最強説
			answerList.Is((byte)3,(byte)4,(byte)5);
		}
		
		[Test]
		public void 入力テスト()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler("+++,");
			compiler.InputEvent += () => 10;
			compiler.Run();
			compiler.CurrentValue.Is((byte)10);
		}
		
		[Test]
		public void ハローワールドテスト()
		{
			var compiler = new BrainFuckCompiler.BrainFuckCompiler("+++++++++[>++++++++>+++++++++++>+++++<<<-]>.>++.+++++++..+++.>-.------------.<++++++++.--------.+++.------.--------.>+.");
			var answerList = new System.Collections.Generic.List<byte>();
			compiler.OutputEvent += output => {
				answerList.Add(output);
			};
			compiler.Run();
			answerList.Is(
				"Hello, world!".Select(ch => (byte)ch));
		}

	}
}
