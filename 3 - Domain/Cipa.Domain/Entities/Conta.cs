using Cipa.Domain.Enums;
using Cipa.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Cipa.Domain.Entities
{
    public class Conta : Entity<int>
    {
        public int? PlanoId { get; set; }
        public bool Ativa { get; set; }
        public int QtdaEstabelecimentos { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public DateTime DataCadastro { get; private set; }

        public virtual Plano Plano { get; private set; }
        public virtual ICollection<Usuario> Usuarios { get; } = new List<Usuario>();
        public virtual ICollection<Empresa> Empresas { get; } = new List<Empresa>();
        private List<EtapaPadraoConta> _etapasPadroes = new List<EtapaPadraoConta>();
        public virtual IReadOnlyCollection<EtapaPadraoConta> EtapasPadroes
        {
            get => new ReadOnlyCollection<EtapaPadraoConta>(_etapasPadroes.OrderBy(e => e.Ordem).ToList());
        }
        private List<TemplateEmail> _templatesEmails = new List<TemplateEmail>();
        public virtual IReadOnlyCollection<TemplateEmail> TemplatesEmails
        {
            get => new ReadOnlyCollection<TemplateEmail>(_templatesEmails.ToList());
        }

        public TemplateEmail BuscarTemplateEmail(ETipoTemplateEmail tipoTemplateEmail) =>
            TemplatesEmails.FirstOrDefault(t => t.TipoTemplateEmail == tipoTemplateEmail);

        public void AdicionarTempateEmail(TemplateEmail templateEmail) {
            _templatesEmails.Add(templateEmail);
        }

        public void AdicionarEtapaPadrao(EtapaPadraoConta etapaPadrao)
        {
            if (etapaPadrao.EtapaObrigatoriaId.HasValue)
            {
                var etapaDuplicada = EtapasPadroes.FirstOrDefault(e => e.EtapaObrigatoriaId == etapaPadrao.EtapaObrigatoriaId);
                if (etapaDuplicada != null)
                    throw new CustomException($"Já há uma etapa correspondente à etapa obrigatória '{etapaDuplicada.EtapaObrigatoria.Nome}'.");
            }
            // Shift
            var etapasPosteriores = EtapasPadroes.Where(e => e.Ordem >= etapaPadrao.Ordem);
            foreach (var etapa in etapasPosteriores)
                etapa.Ordem++;
            _etapasPadroes.Add(etapaPadrao);
        }

        public void AtualizarEtapaPadrao(EtapaPadraoConta etapaPadrao)
        {
            var etapaExistente = EtapasPadroes.FirstOrDefault(e => e == etapaPadrao);
            if (etapaExistente == null) throw new NotFoundException("Etapa não existente.");

            etapaExistente.Descricao = etapaPadrao.Descricao;
            etapaExistente.DuracaoPadrao = etapaPadrao.DuracaoPadrao;
            etapaExistente.Nome = etapaExistente.Nome;
        }

        public void RemoverEtapaPadrao(EtapaPadraoConta etapaPadrao)
        {
            var etapaExistente = EtapasPadroes.FirstOrDefault(e => e == etapaPadrao);
            if (etapaExistente == null) throw new NotFoundException("Etapa não existente.");

            if (etapaExistente.EtapaObrigatoriaId.HasValue)
                throw new CustomException("Não é permitido remover uma etapa obrigatória.");

            _etapasPadroes.Remove(etapaExistente);

            // Shift
            var etapasPosteriores = EtapasPadroes.Where(e => e.Ordem > etapaPadrao.Ordem);
            foreach (var etapa in etapasPosteriores)
                etapa.Ordem--;
        }

        public EtapaPadraoConta BuscarEtapaPadraoPeloId(int id) => EtapasPadroes.FirstOrDefault(e => e.Id == id);
    }
}
