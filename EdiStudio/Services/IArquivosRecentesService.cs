using EdiStudio.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdiStudio.Services
{
    public interface IArquivosRecentesService
    {
        ArquivosRecentesData Carregar();
        void Salvar(ArquivosRecentesData data);
    }
}
