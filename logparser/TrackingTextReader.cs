using System;
using System.IO;

namespace configgen
{
	public class TrackingTextReader : TextReader
	{
		private TextReader _baseReader;
		private long _position;

		public TrackingTextReader(TextReader baseReader)
		{
			_baseReader = baseReader;
			_position = 0;
		}

		public override int Read()
		{
			_position++;
			return _baseReader.Read();
		}

		public override int Peek()
		{
			return _baseReader.Peek();
		}

		public long Position
		{
			get { return _position; }
		}
	}}

