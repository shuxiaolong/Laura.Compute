using System;

namespace Laura.Compute
{
    [Serializable]
    public class FieldFormat
    {
        private string formatExpres;
        private string startExpres;
        private string endExpres;
        private bool isValid;

        public string FormatExpres
        {
            get { return formatExpres; }
            set
            {
                if (formatExpres == value) return;

                formatExpres = value ?? string.Empty;
                int length = 3;
                int index = formatExpres.IndexOf("{0}");
                if (index < 0)
                {
                    length = 7;
                    index = formatExpres.IndexOf("{Field}");
                }
                if (index >= 0)
                {
                    startExpres = formatExpres.Substring(0, index);
                    endExpres = formatExpres.Substring(index + length);
                    isValid = true;
                }
                else
                    isValid = false;
            }
        }
        public string StartExpres
        {
            get { return startExpres; }
        }
        public string EndExpres
        {
            get { return endExpres; }
        }
        public bool IsValid
        {
            get { return isValid; }
        }

        public FieldFormat() { }
        public FieldFormat(string format)
        {
            FormatExpres = format;
        }
    }
}
