using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kalea2.Models
{
    public class PdfCelda
    {
        public string texto { get; set; }
        public int HorizontalAlignment { get; set; }
        public int VerticalAlignment { get; set; }
        public int Rowspan { get; set; }
        public int Colspan { get; set; }
        public int Border { get; set; }
        public int PaddingBottom { get; set; }
        public int PaddingTop { get; set; }


        public PdfCelda()
        {
            this.texto                  = "";
            this.HorizontalAlignment    = 0;
            this.VerticalAlignment      = 0;
            this.Rowspan                = 0;
            this.Colspan                = 0;
            this.Border                 = 0;
            this.PaddingBottom          = 0;
            this.PaddingTop             = 0;
        }
    }
}