﻿using Microsoft.EntityFrameworkCore;
using Profissional.Dominio.Entidades;
using Profissional.Dominio.Interfaces;
using Profissional.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profissional.Repositorio
{
    public class AvaliacaoRep : BaseRep<Avaliacao>, IAvaliacao
    {
        private Contexto db = new Contexto();

        public async Task<List<Avaliacao>> GetByAvaliado(int idAvaliado, int idCliente)
        {
            return await db.Avaliacao.Where(c => c.UsuarioAvaliadoId == idAvaliado 
                && c.Ativo == true && c.IdCliente.Equals(idCliente)).ToListAsync();
        }

        public async Task<List<Avaliacao>> GetByAvaliadorAsync(int idAvaliador, int idCliente)
        {
            return await db.Avaliacao.Where(c => c.UsuarioAvaliadorId == idAvaliador 
            && c.Ativo == true && c.IdCliente.Equals(idCliente)).ToListAsync();
        }

        public async Task<List<Avaliacao>> GetByCodigoExterno(int codigoExterno, int idCliente)
        {
            return await db.Avaliacao.Where(c => c.CodigoExterno.Equals(codigoExterno.ToString())
            && c.Ativo == true && c.IdCliente.Equals(idCliente)).ToListAsync();
        }

        public async Task<Avaliacao> GetById(int id, int idCliente)
        {
            return await db.Avaliacao.FirstAsync(c => c.ID == id 
            && c.Ativo == true && c.IdCliente.Equals(idCliente));
        }

        public async Task<List<Avaliacao>> GetByOportunidade(int idOportunidade, int idCliente)
        {
            var r = await db.Avaliacao.Where(c => c.OportunidadeId.Equals(idOportunidade)
            && c.Ativo && c.IdCliente.Equals(idCliente)).ToListAsync();

            return r;
        }
    }
}
