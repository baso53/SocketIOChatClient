using System.Windows.Forms;

namespace SocketIOChatClient
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption, int maxLength, bool useMasked)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 60, Top = 69, Text = text };
            textLabel.ForeColor = System.Drawing.SystemColors.MenuBar;

            TextBox textBox = new TextBox() { Left = 160, Top = 66, Width = 172 };
            textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox.ForeColor = System.Drawing.SystemColors.MenuBar;
            textBox.MaxLength = maxLength;

            Button confirmation = new Button() { Text = "Ok", Left = 257, Width = 75, Top = 126, DialogResult = DialogResult.OK };
            confirmation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(27)))), ((int)(((byte)(27)))));
            confirmation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            confirmation.ForeColor = System.Drawing.SystemColors.MenuBar;
            confirmation.Click += (sender, e) => { prompt.Close(); };
            confirmation.FlatAppearance.BorderSize = 0;
            confirmation.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));

            prompt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            prompt.Icon = SocketIOChatClient.Properties.Resources.Icon;
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            prompt.MaximizeBox = false;

            if (useMasked)
            {
                textBox.PasswordChar = '*';
            }

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "=?*exitcode";
        }
    }
}
