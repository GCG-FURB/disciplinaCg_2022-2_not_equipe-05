using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System;
using System.Collections.Generic;

namespace gcgcg
{
  internal class Scan_Line
  {
    public Scan_Line() {

    }

    public bool validaDentro(Ponto4D ponto, List<Ponto4D> lista_ponto) {
      Ponto4D ultimoPonto = lista_ponto[0];
      int qntInterseccao = 0;
      for (int i = 1; i < lista_ponto.Count; i++)
      {
        Ponto4D pontoValidado = lista_ponto[i];
        double pontoIntersecao = ultimoPonto.X + (pontoValidado.X - ultimoPonto.X) *((ponto.Y - ultimoPonto.Y) / (pontoValidado.Y - ultimoPonto.Y));
        if(pontoIntersecao > ponto.X && ((ultimoPonto.X > pontoIntersecao && pontoValidado.X < pontoIntersecao) || (ultimoPonto.X < pontoIntersecao && pontoValidado.X > pontoIntersecao))) {
          Console.WriteLine("Ponto de Interseccao: " + pontoIntersecao);
          qntInterseccao++;
        }
       ultimoPonto = lista_ponto[i]; 
      }
      Ponto4D primeiroPonto = lista_ponto[0];
      double pontoInterseccao = ultimoPonto.X + (primeiroPonto.X - ultimoPonto.X) *((ponto.Y - ultimoPonto.Y) / (primeiroPonto.Y - ultimoPonto.Y));
      if(pontoInterseccao > ponto.X && ((ultimoPonto.X > pontoInterseccao && primeiroPonto.X < pontoInterseccao) || (ultimoPonto.X < pontoInterseccao && primeiroPonto.X > pontoInterseccao))) {
        Console.WriteLine("Ponto de Interseccao: " + pontoInterseccao);
          qntInterseccao++;
        }
      Console.WriteLine("quantidade de intersecção: "+ qntInterseccao);
      if (qntInterseccao%2 == 0) {
        return false;
      } else {
        return true;
      }
    }
  }

}