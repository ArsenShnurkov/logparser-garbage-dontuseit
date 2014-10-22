using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Gdk;

namespace configgen
{
	public class CommonPart
	{
		public string text;
		public int freq;

		public CommonPart (string text, int freq)
		{
			this.text = text;
			this.freq = freq;
		}
	}

	public class OrderByFreq<T> : IComparer<T> where T : CommonPart {
		int IComparer<T>.Compare(T x, T y )  {
			if (y.freq != x.freq) {
				return y.freq - x.freq;
			}
			return String.Compare (y.text, x.text);
		}
	}
	public class SortedPatterns
	{
		SortedDictionary<string, CommonPart> dict = new SortedDictionary<string, CommonPart> ();
		SortedList<CommonPart, CommonPart> freq = new SortedList<CommonPart, CommonPart>(new OrderByFreq<CommonPart>());

		public void Process (IListOfStrings fs)
		{
			ProcessInternall (fs);
			/*			List<KeyValuePair<string, int>> myList = dict.ToList ();
			myList.Sort ((x, y) => y.Key.Length.CompareTo (x.Key.Length));
			foreach (var kvp in myList) {
				Debug.WriteLine (kvp.Value + "\t\"" + kvp.Key + "\"");
			}*/
		}

		public void ProcessInternall (IListOfStrings fs)
		{
			int totalLines = fs.TotalLines;
			var mapper = new IndexToPairMapperTX ();
			mapper.N = totalLines;
			long totalPairs = mapper.GetTotalPairs ();

			int old_dict_length = 0;
			//string uniquePart = String.Empty;
			for (long tp = 0; tp < totalPairs; tp++) {
				old_dict_length = dict.Count;

				long tprev = totalPairs - 1 - tp;
				Point pt = mapper.GetPairByIndex (tprev);
				int i = pt.X;
				int j = pt.Y;
				string line_i = fs.GetLine (i);
				if (String.IsNullOrEmpty (line_i)) {
					continue;
				}
				string line_j = fs.GetLine (j);
				if (String.IsNullOrEmpty (line_j)) {
					continue;
				}
				ProcessPair (line_i, line_j);

				if (old_dict_length != dict.Count && fs is FileOfStrings) {
					Debug.WriteLine (tp + "{" + (j - i) + "}" + "\t" + dict.Count);
					//RunThroughDictionary ();
				}
			}
		}

		void ProcessPair (string line_i, string line_j)
		{
			string[] s1, s2, s3;
			StringHelper.GetLongestCommonSubstring (line_i, line_j, out s1, out s2, out s3);
			ProcessParts (s1);
			//ProcessParts (s2);
			//ProcessParts (s3);
		}

		void ProcessParts (string[] parts)
		{
			for (int p = 0; p < parts.Length; ++p) {
				string part = parts [p];
				if (false == dict.ContainsKey (part)) {
					var cp = new CommonPart (part, 0);
					dict.Add (part, cp);
					freq.Add (cp, cp);
					Debug.WriteLine ("\"" + part + "\"");
					//RunThroughDictionary ();
				}
				dict [part].freq++;
			}
		}

		void RunThroughDictionary ()
		{
			var list = new List<string>(freq.Count);
			foreach (CommonPart li in freq.Keys) {
				//Debug.WriteLine (li.freq + " " + li.text);
				list.Add (li.text);
			}
			ProcessInternall (new MemoryBasedListOfStrings (list));
		}
	}
}

