using System;
using Gtk;
using System.IO;
using System.Text;

using configgen;


public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void generateButtonHander (object sender, EventArgs e)
	{
		string fileName = this.logFilePath.Entry.Text;
		var fi = new FileInfo (fileName);
		if (fi.Exists == false) {
			MessageDialog md1 = new MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "файл не существует на диске");
			md1.Run ();
			md1.Destroy();
		}
		var pf = new FileOfStrings (fi);
		pf.LoadAndPreparse ();

		var sp = new SortedPatterns ();
		sp.Process (pf);

		using (var ms = new MemoryStream ()) {
			using (var sw = new StreamWriter (ms)) {
				sw.Write (pf.TotalLines.ToString());
				sw.Write (Environment.NewLine);
			}
			var text = Encoding.Default.GetString (ms.ToArray ());
			this.textview1.Buffer.Text = text;
		}



		MessageDialog md = new MessageDialog (this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Процесс чтения завершен");
		md.Run ();
		md.Destroy();
	}

}
