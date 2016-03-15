using System.Windows.Forms;

namespace PDTUtils.MVVM
{
    class WpfMessageBoxService : IMessageBoxService
    {
        /*MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
                MessageBoxDefaultButton.Button1,  // specify "Yes" as the default
                (MessageBoxOptions)0x40000*/

        public bool ShowMessage(string text, string caption)
        {
            if (MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning, 
                                MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000) == DialogResult.OK)
                return true;
            else
                return false;
        }
    }
}
