using System;
using System.Collections.Generic;
using System.Text;

namespace EdiStudio.Models
{
    public class ArquivosRecentesData
    {
        public List<ArquivoEdiRecente> ArquivosEdiRecentes { get; set; } = [];
        public List<LayoutRecente> LayoutsRecentes { get; set; } = [];
    }
}
