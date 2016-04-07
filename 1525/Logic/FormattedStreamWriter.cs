using System;
using System.IO;

namespace PDTUtils.Logic
{
    public class FormattingStreamWriter : StreamWriter
    {
        private readonly IFormatProvider formatProvider;

        public FormattingStreamWriter(string filename, IFormatProvider formatProvider, bool append)
            : base(filename, append)
        {
            this.formatProvider = formatProvider;
        }

        public FormattingStreamWriter(string path, IFormatProvider formatProvider)
            : base(path)
        {
            this.formatProvider = formatProvider;
        }

        public override IFormatProvider FormatProvider
        {
            get
            {
                return this.formatProvider;
            }
        }
    }
}
