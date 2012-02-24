/*
 * Created by SharpDevelop.
 * User: Hiroshi
 * Date: 2012/02/10
 * Time: 23:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
namespace BrainFuckCompiler
{
	/// <summary>
	/// Description of Class1.
	/// </summary>
	public class BrainFuckCompiler
	{
		private List<byte> _memoryStream = new List<byte>(){0};
		
		private int _programPointer = 0;
		
		private string _input;
		
		private Dictionary<int,int> _brucketPairs  = new Dictionary<int, int>();
		
		public byte CurrentValue
		{
			get { return _memoryStream[_programPointer]; }
		}

		public int ProgramPointer
		{
			get { return _programPointer; }
		}
		
		
		public BrainFuckCompiler(string inputString)
		{
			//フィールド変数で定義
			_input = inputString;
		}
		
		public event Action<byte> OutputEvent;
		public event Func<byte> InputEvent;
		
		public void Run()
		{
			for(int i = 0; i < _input.Length; )
			{
				switch (_input[i])
				{
					case '>':
						_programPointer++;
						AllocateMemory();
						i++;
						break;
					case '<':
						if(_programPointer <= 0)
						{
							throw new IndexOutOfRangeException();
						}
						_programPointer--;
						i++;
						break;
					case '+':
						if(_memoryStream[_programPointer] == byte.MaxValue)
						{
							_memoryStream[_programPointer] = byte.MinValue;
						}
						else{
							_memoryStream[_programPointer]++;
						}
						i++;
						break;
					case '-':
						if(_memoryStream[_programPointer] == byte.MinValue)
						{
							_memoryStream[_programPointer] = byte.MaxValue;
						}
						else{
							_memoryStream[_programPointer]--;
						}
						i++;
						break;
					case '[':
						//0ならば
						if(_memoryStream[_programPointer] == 0)
						{
							//対応する括弧の次までポインタを進める
							i = GetIndexOfEndBrucket(i) + 1;
						}
						else{
							i++;
						}
						break;
					case ']':
						//対応する括弧のまでポインタを戻す
						i = GetIndexOfStartBrucket(i);
						break;
					case '.':
						if(OutputEvent != null)
						{
							OutputEvent(_memoryStream[_programPointer]);
						}
						i++;
						break;
					case ',':
						if(InputEvent != null)
						{
							_memoryStream[_programPointer] = InputEvent();
						}
						i++;
						break;
					default:
						break;
				}
			}
		}
		
		int GetIndexOfStartBrucket(int currentIndex)
		{
			if(!_brucketPairs.ContainsValue(currentIndex))
			{
				_brucketPairs.Add(FindIndexOfStartBrucket(currentIndex),currentIndex);
			}
			
			return _brucketPairs.First(dict => dict.Value == currentIndex).Key;
		}
		
		//入力を括弧閉じのインデックスとする
		int FindIndexOfStartBrucket(int currentIndex)
		{
			//初期化
			currentIndex--;
			int brucketCounter = 1;
			//前に戻りながら括弧を見つける
			while(brucketCounter > 0)
			{
				if(currentIndex < 0)
				{
					throw new ApplicationException("括弧が閉じられていません");
				}
				
				if(_input[currentIndex] == '[')
				{
					brucketCounter--;
				}
				else if(_input[currentIndex] == ']')
				{
					brucketCounter++;
				}
				currentIndex--;
			}
			//必ず行き過ぎてるので戻しておく
			return currentIndex + 1;
		}
		
		int GetIndexOfEndBrucket(int currentIndex)
		{
			if(!_brucketPairs.ContainsKey(currentIndex))
			{
				_brucketPairs.Add(currentIndex, FindIndexOfEndBrucket(currentIndex));
			}
			return _brucketPairs[currentIndex];
		}
		
		// 入力を括弧位置のインデックスとする
		int FindIndexOfEndBrucket(int index)
		{
			//まずひとつ進めて・・・
			index++;
			//括弧閉じのカウンタも１に初期化
			int brucketCounter = 1;
			//括弧閉じカウンタが0になるまで入力文字をパース
			while(brucketCounter > 0)
			{
				if(index >= _input.Length)
				{
					throw new ApplicationException("括弧が閉じられていません");
				}
				
				//開き括弧なら括弧カウンタを加算
				if(_input[index] == '[')
				{
					brucketCounter++;
				}
				//閉じ括弧なら括弧カウンタを減算
				else if(_input[index] == ']')
				{
					brucketCounter--;
				}
				index++;
			}
			//必ず一つ行き過ぎているので、一つ戻しておく
			return index - 1;
		}
		
		void AllocateMemory()
		{
			while (_memoryStream.Count - 1 < _programPointer) {
				_memoryStream.Add((byte)0);
			}
		}
	}
}
