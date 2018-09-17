﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Profissional.Dominio.Entidades
{
    public class Endereco : Base
    {
        public string CEP { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Local { get; set; }
        public int NumeroLocal { get; set; }
        public string Complemento { get; set; }
        public int ProfissionalId { get; set; }
    }
}
