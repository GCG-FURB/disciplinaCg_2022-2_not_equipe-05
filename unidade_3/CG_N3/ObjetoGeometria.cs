/**
  Autor: Dalton Solano dos Reis
**/

using System.Collections.Generic;
using CG_Biblioteca;
using System; 

namespace gcgcg
{
  internal abstract class ObjetoGeometria : Objeto
  {
    protected List<Ponto4D> pontosLista = new List<Ponto4D>();

    public ObjetoGeometria(char rotulo, Objeto paiRef) : base(rotulo, paiRef) { }

    private Scan_Line scan_Line = new Scan_Line();

    protected override void DesenharGeometria()
    {
      DesenharObjeto();
    }
    protected abstract void DesenharObjeto();
    public void PontosAdicionar(Ponto4D pto)
    {
      pontosLista.Add(pto);
      if (pontosLista.Count.Equals(1))
        base.BBox.Atribuir(pto);
      else
        base.BBox.Atualizar(pto);
      base.BBox.ProcessarCentro();
    }

    public void PontosRemoverUltimo()
    {
      pontosLista.RemoveAt(pontosLista.Count - 1);
    }

    protected void PontosRemoverTodos()
    {
      pontosLista.Clear();
    }

    public Ponto4D PontosUltimo()
    {
      return pontosLista[pontosLista.Count - 1];
    }

    public void PontosAlterar(Ponto4D pto, int posicao)
    {
      pontosLista[posicao] = pto;
    }

    public bool ValidaDentroObjeto(Ponto4D ponto) {
      if (BBox.validaDentro(ponto)) {
        return scan_Line.validaDentro(ponto, pontosLista);
      }
      return false;
    }

    public List<Ponto4D> getPontosLista(Ponto4D ponto) {
      double menorDistancia = Double.MaxValue;
      Ponto4D pontoMaisProximo = null;
      foreach (Ponto4D pontoLista in pontosLista)
      {
        double distancia = Math.Sqrt(Math.Pow(ponto.X - pontoLista.X, 2) + Math.Pow(ponto.Y - pontoLista.Y, 2));
        if ( distancia < menorDistancia ) {
          pontoMaisProximo = pontoLista;
        }
      }

      pontosLista.Remove(pontoMaisProximo);
      this.PontosAdicionar(pontoMaisProximo);
      return this.pontosLista;
    }

    public Ponto4D getPontoProximo(Ponto4D ponto) {
      double menorDistancia = Double.MaxValue;
      Ponto4D pontoMaisProximo = null;
      foreach (Ponto4D pontoLista in pontosLista)
      {
        double distancia = Math.Sqrt(Math.Pow(ponto.X - pontoLista.X, 2) + Math.Pow(ponto.Y - pontoLista.Y, 2));
        if ( distancia < menorDistancia ) {
          pontoMaisProximo = pontoLista;
        }
      }

      return pontoMaisProximo;
    }

    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto: " + base.rotulo + "\n";
      for (var i = 0; i < pontosLista.Count; i++)
      {
        retorno += "P" + i + "[" + pontosLista[i].X + "," + pontosLista[i].Y + "," + pontosLista[i].Z + "," + pontosLista[i].W + "]" + "\n";
      }
      return (retorno);
    }
  }
}