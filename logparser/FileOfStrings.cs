using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

namespace configgen
{
	public struct LineInfo
	{
		public int number;
		public long position;
		public int length;
		public WeakReference StringBodyRef;
		public string locker;
	}
	/*public struct StringBody
	{
		string Text;
	}*/
	public interface IListOfStrings
	{
		int TotalLines {get ;}
		string GetLine(int index);
	}

	public class MemoryBasedListOfStrings : IListOfStrings
	{
		List<string> lines;
		public MemoryBasedListOfStrings(List<string> lines)
		{
			this.lines = lines;
		}
		public string GetLine(int index)
		{
			return lines [index];
		}
		public int TotalLines {
			get {
				return lines.Count;
			}
		}
	}

	public class FileOfStrings : IListOfStrings
	{
		public int TotalLines {
			get {
				return lines.Count;
			}
		}

		FileInfo fileInfo;
		public List<LineInfo> lines = new List<LineInfo>();
		public FileOfStrings(FileInfo file)
		{
			this.fileInfo = file;
		}
		public string GetLine(int index)
		{
			return GetLine (lines [index]);
		}
		public void LoadAndPreparse()
		{
			string fileName = fileInfo.FullName;
			using (var fs = new FileStream(fileName, FileMode.Open)) {
				using (var tr = new TrackingTextReader(new StreamReader (fs))) {
					for (;;)
					{
						long pos1 = GetCharPos(tr);
						var line = tr.ReadLine ();
						if (line == null)
							break;
						long pos2 = GetCharPos(tr);
						ProcessLine (line, pos1, (int)(pos2 - pos1));
					}
				}
			}
		}

		long GetCharPos (TrackingTextReader tr)
		{
			return tr.Position;
		}

		int totalLines = 0;
		long totallength = 0;
		void ProcessLine (string line, long pos, int len)
		{
			totallength += line.Length;
			var si = new LineInfo ();
			si.number = totalLines;
			si.position = pos;
			si.length = len;
			si.StringBodyRef = new WeakReference (line);
			si.locker = line;
			lines.Add (si);
			totalLines++;
		}
		string GetLine(LineInfo info)
		{
			string s = info.StringBodyRef.Target as string;

			if (s != null) {
				return s;
			}

			string fileName = fileInfo.FullName;
			using (var fs = new FileStream (fileName, FileMode.Open)) {
				fs.Position = info.position;
				var bytes = new byte[info.length];
				fs.Read(bytes, 0, (int)info.length);
				var res = Encoding.Default.GetString (bytes);
				info.StringBodyRef.Target = res;
				return res;
			}
		}
	}
}
