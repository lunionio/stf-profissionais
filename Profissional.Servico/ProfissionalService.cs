﻿using Profissional.Dominio.Entidades;
using Profissional.Repositorio;
using Profissional.Servico.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Profissional.Servico
{
    public class ProfissionalService : IService<Dominio.Entidades.Profissional>
    {
        private readonly ProfissionalServicoRep _servicoRep;
        private readonly ProfissionalRepository _pfRepository;
        private readonly ServicoRep _sRep;
        private static readonly object _lock = new object();

        public ProfissionalService(ProfissionalRepository profissionalRepository, ProfissionalServicoRep pServicoRep, ServicoRep servicoRep)
        {
            _pfRepository = profissionalRepository;
            _servicoRep = pServicoRep;
            _sRep = servicoRep;
        }

        public async Task<Dominio.Entidades.Profissional> SaveAsync(Dominio.Entidades.Profissional entity, string token)
        {
            try
            {
                if (await SeguracaServ.ValidaTokenAsync(token))
                {
                    if (entity.ID == 0)
                    {
                        entity.DataCriacao = DateTime.UtcNow;
                        entity.DataEdicao = DateTime.UtcNow;
                        //entity.Ativo = true;
                        var pfId = _pfRepository.Add(entity);

                        if (entity.Endereco != null)
                        {
                            entity.Endereco.ProfissionalId = pfId;
                        }

                        if (entity.Telefone != null)
                        {
                            entity.Telefone.ProfissionalId = pfId;
                        }
                    }
                    else
                    {
                        entity = await UpdateAsync(entity, token);
                    }

                    return entity;
                }
                else
                {
                    throw new Exception("Token inválido!");
                }
            }
            catch(Exception e)
            {
                throw new Exception("Erro ao efetuar requisição!", e);
            }
        }

        public async Task<IEnumerable<Dominio.Entidades.Profissional>> GetAllAsync(int idCliente, string token)
        {
            try
            {
                if (await SeguracaServ.ValidaTokenAsync(token))
                {
                    var result = _pfRepository.GetList(p => p.IdCliente.Equals(idCliente));
                    return result;
                }
                else
                {
                    throw new Exception("Token inválido!");
                }
            }
            catch(Exception e)
            {
                throw new Exception("Erro ao efetuar requisição!", e);
            }
        }

        public async Task<IEnumerable<ProfissionalServico>> GetProfissionalServicos(int idCliente, string token)
        {
            try
            {
                if (await SeguracaServ.ValidaTokenAsync(token))
                {
                    lock (_lock)
                    {
                        var profissionais = _pfRepository.GetList(p => p.IdCliente.Equals(idCliente));
                        var pIds = new List<int>();

                        foreach (var item in profissionais)
                        {
                            pIds.Add(item.ID);
                        }

                        var pServicos = _servicoRep.GetList(x => pIds.Contains(Convert.ToInt32(x.CodigoExterno)));
                        var sIds = pServicos.Select(x => x.ServicoId);

                        var servicos = _sRep.GetList(s => sIds.Contains(s.ID));

                        foreach (var item in pServicos)
                        {
                            item.Profissional = profissionais.FirstOrDefault(p => p.ID.Equals(item.CodigoExterno));
                            item.Servico = servicos.FirstOrDefault(s => s.ID.Equals(item.ServicoId));
                        }

                        return pServicos;
                    }
                }
                else
                {
                    throw new Exception("Token inválido!");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao efetuar requisição!", e);
            }
        }

        public async Task<Dominio.Entidades.Profissional> GetByIdAsync(int entityId, int idCliente, string token)
        {
            try
            {
                if (await SeguracaServ.ValidaTokenAsync(token))
                {
                    var result = _pfRepository.GetList(p => p.IdCliente.Equals(idCliente) && p.ID.Equals(entityId)).SingleOrDefault();
                    return result;
                }
                else
                {
                    throw new Exception("Token inválido.");
                }
            }
            catch(Exception e)
            {
                throw new Exception("Erro ao efetuar requisição!", e);
            }
        }

        public async Task<ProfissionalServico> GetProfissionalServicoByIdAsync(int entityId, int idCliente, string token)
        {
            try
            {
                if (await SeguracaServ.ValidaTokenAsync(token))
                {
                    lock (_lock)
                    {
                        var result = _pfRepository.GetList(p => p.IdCliente.Equals(idCliente) && p.ID.Equals(entityId)).SingleOrDefault();
                        var pServico = _servicoRep.GetList(x => x.CodigoExterno.Equals(result.ID)).FirstOrDefault();

                        var servico = _sRep.GetList(s => s.ID.Equals(pServico.ServicoId)).FirstOrDefault();

                        pServico.Servico = servico;
                        pServico.Profissional = result;

                        return pServico;
                    }
                }
                else
                {
                    throw new Exception("Token inválido.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao efetuar requisição!", e);
            }
        }

        public async Task<Dominio.Entidades.Profissional> UpdateAsync(Dominio.Entidades.Profissional entity, string token)
        {
            try
            {
                if (await SeguracaServ.ValidaTokenAsync(token))
                {
                    entity.DataEdicao = DateTime.UtcNow;
                    _pfRepository.Update(entity);

                    return entity;
                }
                else
                {
                    throw new Exception("Token inválido.");
                }
            }
            catch(Exception e)
            {
                throw new Exception("Erro ao efetuar requisição!", e);
            }
        }

        public async Task RemoveAsync(Dominio.Entidades.Profissional entity, string token)
        {
            try
            {
                if (await SeguracaServ.ValidaTokenAsync(token))
                {
                    _pfRepository.Remove(entity);
                }
                else
                {
                    throw new Exception("Token inválido.");
                }
            }
            catch(Exception e)
            {
                throw new Exception("Erro ao efetuar requisição!", e);
            }
        }

        public async Task<IEnumerable<ProfissionalServico>> GetByIdListAsync(int idCliente, string token, IEnumerable<int> ids)
        {
            try
            {
                if (await SeguracaServ.ValidaTokenAsync(token))
                {
                    var result = _pfRepository.GetList(p => ids.Contains(p.ID));

                    var pIds = result.Select(x => x.ID);

                    var pServicos = _servicoRep.GetList(x => pIds.Contains(x.UsuarioId));
                    var sIds = pServicos.Select(x => x.ServicoId);

                    var servicos = _sRep.GetList(s => sIds.Contains(s.ID));

                    foreach (var item in pServicos)
                    {
                        item.Profissional = result.FirstOrDefault(p => p.ID.Equals(item.UsuarioId));
                        item.Servico = servicos.FirstOrDefault(s => s.ID.Equals(item.ServicoId));
                    }

                    return pServicos;
                }
                else
                {
                    throw new Exception("Token inválido.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao efetuar requisição!", e);
            }

        }

        public async Task<ProfissionalServico> GetByUserIdAsync(int idCliente, string token, int userId)
        {
            try
            {
                if (await SeguracaServ.ValidaTokenAsync(token))
                {
                    lock (_lock)
                    {
                        var result = _pfRepository.GetSingle(p => p.IdCliente.Equals(idCliente) && p.IdUsuario.Equals(userId));

                        var pServico = _servicoRep.GetSingle(x => x.CodigoExterno.Equals(result.ID));

                        var servico = _sRep.GetSingle(s => s.ID.Equals(pServico.ServicoId));

                        pServico.Servico = servico;
                        pServico.Profissional = result;

                        return pServico;
                    }
                }
                else
                {
                    throw new Exception("Token inválido.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao efetuar requisição!", e);
            }
        }

        public async Task<IEnumerable<ProfissionalServico>> GetByUserIdListAsync(int idCliente, string token, IEnumerable<int> ids)
        {
            try
            {
                if (await SeguracaServ.ValidaTokenAsync(token))
                {
                    lock (_lock)
                    {
                        var result = _pfRepository.GetList(p => ids.Contains(p.IdUsuario));

                        var pIds = result.Select(x => x.ID);

                        var pServicos = _servicoRep.GetList(x => pIds.Contains(Convert.ToInt32(x.CodigoExterno)));
                        var sIds = pServicos.Select(x => x.ServicoId);

                        var servicos = _sRep.GetList(s => sIds.Contains(s.ID));

                        foreach (var item in pServicos)
                        {
                            item.Profissional = result.FirstOrDefault(p => p.ID.Equals(item.CodigoExterno));
                            item.Servico = servicos.FirstOrDefault(s => s.ID.Equals(item.ServicoId));
                        }

                        return pServicos;
                    }
                }
                else
                {
                    throw new Exception("Token inválido.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao efetuar requisição!", e);
            }

        }
    }
}