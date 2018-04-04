using System;

public static class MessageAppender
{
    public static void FormatMessage (this System.Windows.Forms.RichTextBox box, Newtonsoft.Json.Linq.JObject msg)
    {
        box.SelectionStart = box.TextLength;
        box.SelectionLength = 0;

        box.SelectionColor = System.Drawing.SystemColors.AppWorkspace;
        box.AppendText(
            "[" + msg["Timestamp"].ToObject<DateTime>().ToShortDateString() + " " + msg["Timestamp"].ToObject<DateTime>().ToShortTimeString() + "] ");
        box.SelectionColor = System.Drawing.SystemColors.Highlight;
        box.AppendText(msg["User"].ToObject<string>() + ": ");
        box.SelectionColor = box.ForeColor;
        box.AppendText(msg["Content"].ToObject<string>() + Environment.NewLine);
    }
}