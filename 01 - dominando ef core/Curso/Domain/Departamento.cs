using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;

namespace DominandoEFCore.Domain
{
    public class Departamento
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public Departamento()
        {

        }

        private Action<object, string> _lazyLoader { get; set; }
        private Departamento(Action<object, string> lazyloader)
        {
            _lazyLoader = lazyloader;
        }

        private List<Funcionario> _funcionarios;
        public List<Funcionario> Funcionarios
        {
            get
            {
                _lazyLoader?.Invoke(this, nameof(Funcionarios));

                return _funcionarios;
            }
            set => _funcionarios = value;
        }

        /*
        private ILazyLoader _lazyLoader { get; set; }
        private Departamento(ILazyLoader lazyloader)
        {
            _lazyLoader = lazyloader;
        }

        private List<Funcionario> _funcionarios;
        public List<Funcionario> Funcionarios
        { 
            get => _lazyLoader.Load(this, ref _funcionarios);
            set => _funcionarios = value; 
        }*/

    }
}
