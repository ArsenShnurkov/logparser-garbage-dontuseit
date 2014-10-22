using System;
using System.Diagnostics;
using Gdk;


namespace configgen
{
	public interface ICN2Mapper
	{
		int N { get; set; }
		Point GetPairByIndex (long index);
		long GetIndexByPair (Point cp);
	}
	public abstract class IndexToPairMapperBase : ICN2Mapper
	{
		private int n;
		public int N { get { return n; } set { n = value; } }
		protected int Negate (int i)
		{
			return (N - 1) - i;
		}
		public long GetTotalPairs ()
		{
			long res = (N * (long)(N - 1)) / 2;
			return res;
		}
		public abstract Point GetPairByIndex (long index);
		public abstract long GetIndexByPair (Point cp);
	}
	public class IndexToPairMapperY : IndexToPairMapperBase
	{
		public override Point GetPairByIndex(long index)
		{
			int row = (int)Math.Floor ( (Math.Sqrt (index * 8 + 1) - 1) / 2);
			long subTotal = (long)row * (row + 1) / 2;
			int x = (int)(index - subTotal);
			int y = row - x;
			Point cp;
			cp.X = x;
			cp.Y = Negate(y);
			return cp;
		}
		public override long GetIndexByPair(Point cp)
		{
			int x = cp.X;
			int y = Negate(cp.Y);
			int row = x + y;
			long subTotal = (long)row * (row + 1) / 2;
			long index = subTotal + cp.X;
			return index;
		}
	}
	public class IndexToPairMapperX : IndexToPairMapperBase
	{
		public override Point GetPairByIndex(long index)
		{
			int row = (int)Math.Floor ( (Math.Sqrt (index * 8 + 1) - 1) / 2);
			long subTotal = (long)row * (row + 1) / 2;
			int x = (int)(index - subTotal);
			int y = row - x;
			Point cp;
			cp.X = Negate(x);
			cp.Y = y;
			return cp;
		}
		public override long GetIndexByPair(Point cp)
		{
			int x = Negate(cp.X);
			int y = cp.Y;
			int row = x + y;
			long subTotal = (long)row * (row + 1) / 2;
			long index = subTotal + x;
			return index;
		}
	}
	public class IndexToPairMapperTX : IndexToPairMapperX
	{
		public override Point GetPairByIndex(long index)
		{
			Point cp  = base.GetPairByIndex(index);
			return new Point(cp.Y, cp.X);
		}
		public override long GetIndexByPair(Point cp)
		{
			long index = base.GetIndexByPair(new Point(cp.Y, cp.X));
			return index;
		}
	}
	public static class IndexToPairMapperExtension
	{
		static public void Process<T>(this T mapper, int N) where T : IndexToPairMapperBase
		{
			mapper.N = N;
			long total = mapper.GetTotalPairs();
			for (long index2 = 0; index2 < total; index2++) {
				long index = total - 1 - index2;
				Point cp = mapper.GetPairByIndex (index);
				Debug.WriteLine (cp.X + " " + cp.Y);
				long reindex = mapper.GetIndexByPair (cp);
				if (index != reindex) {
					Debug.WriteLine (index + " " + reindex);
				}
			}
		}
	}
}

