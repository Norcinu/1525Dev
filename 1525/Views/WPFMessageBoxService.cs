using System.Windows.Forms;

namespace PDTUtils.MVVM
{
    class WpfMessageBoxService : IMessageBoxService
    {
        public bool ShowMessage(string text, string caption)
        {
            if (MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
                return true;
            else
                return false;
        }
    }
}
